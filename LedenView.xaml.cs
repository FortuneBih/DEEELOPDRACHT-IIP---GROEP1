using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BubbelvriendWPF.Data;
using BubbelvriendWPF.Models;

namespace BubbelvriendWPF.Views
{
    public partial class LedenView : Page
    {
        private Person _geselecteerdLid = null;
        private bool _isWijzigenModus = false;

        private ObservableCollection<Person> _gefilterdeLeden;

        public LedenView()
        {
            InitializeComponent();
            _gefilterdeLeden = new ObservableCollection<Person>(AppData.Leden);
            DgLeden.ItemsSource = _gefilterdeLeden;
            PasAantalLedenBijwerken();
        }

        private void TxtZoeken_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasFilterToe();
        }

        private void CmbFilterSterren_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PasFilterToe();
        }

        private void PasFilterToe()
        {
            if (_gefilterdeLeden == null) return;

            string zoekterm = TxtZoeken?.Text?.ToLower() ?? "";
            int filterSterren = CmbFilterSterren?.SelectedIndex ?? 0;

            var gefilterd = AppData.Leden.Where(p =>
            {
                bool matchZoek = string.IsNullOrEmpty(zoekterm) ||
                                 p.Naam.ToLower().Contains(zoekterm) ||
                                 p.Voornaam.ToLower().Contains(zoekterm) ||
                                 p.Rijksregisternummer.Contains(zoekterm) ||
                                 p.Email.ToLower().Contains(zoekterm) ||
                                 p.Gemeente.ToLower().Contains(zoekterm);

                bool matchSterren = filterSterren == 0 || p.Sterren == filterSterren;

                return matchZoek && matchSterren;
            });

            _gefilterdeLeden.Clear();
            foreach (var lid in gefilterd)
                _gefilterdeLeden.Add(lid);

            PasAantalLedenBijwerken();
        }

        private void PasAantalLedenBijwerken()
        {
            if (TxtAantalLeden != null)
                TxtAantalLeden.Text = $"{_gefilterdeLeden.Count} / {AppData.Leden.Count} leden";
        }

        private void DgLeden_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _geselecteerdLid = DgLeden.SelectedItem as Person;
            bool heeftSelectie = _geselecteerdLid != null;

            BtnWijzigen.IsEnabled = heeftSelectie;
            BtnVerwijderen.IsEnabled = heeftSelectie;

            if (heeftSelectie)
                VulFormulierIn(_geselecteerdLid);
        }

        private void VulFormulierIn(Person p)
        {
            TxtNaam.Text = p.Naam;
            TxtVoornaam.Text = p.Voornaam;
            TxtRijksreg.Text = p.Rijksregisternummer;
            TxtStraat.Text = p.Straat;
            TxtHuisnummer.Text = p.Huisnummer;
            TxtPostcode.Text = p.Postcode;
            TxtGemeente.Text = p.Gemeente;
            TxtTelefoon.Text = p.Telefoonnummer;
            TxtEmail.Text = p.Email;
            CmbSterren.SelectedIndex = p.Sterren - 1;
            VerbergFout();
        }

        private Person LeesFormulier()
        {
            return new Person
            {
                Naam = TxtNaam.Text.Trim(),
                Voornaam = TxtVoornaam.Text.Trim(),
                Rijksregisternummer = TxtRijksreg.Text.Trim(),
                Straat = TxtStraat.Text.Trim(),
                Huisnummer = TxtHuisnummer.Text.Trim(),
                Postcode = TxtPostcode.Text.Trim(),
                Gemeente = TxtGemeente.Text.Trim(),
                Telefoonnummer = TxtTelefoon.Text.Trim(),
                Email = TxtEmail.Text.Trim(),
                Sterren = CmbSterren.SelectedIndex + 1
            };
        }

        private void LeegmakFormulier()
        {
            TxtNaam.Clear(); TxtVoornaam.Clear(); TxtRijksreg.Clear();
            TxtStraat.Clear(); TxtHuisnummer.Clear();
            TxtPostcode.Clear(); TxtGemeente.Clear();
            TxtTelefoon.Clear(); TxtEmail.Clear();
            CmbSterren.SelectedIndex = -1;
            _isWijzigenModus = false;
            _geselecteerdLid = null;
            DgLeden.SelectedItem = null;
            BtnWijzigen.IsEnabled = false;
            BtnVerwijderen.IsEnabled = false;
            TxtFormulierTitel.Text = "Nieuw lid toevoegen";
            VerbergFout();
        }

        private void ToonFout(string boodschap)
        {
            TxtFout.Text = boodschap;
            FoutPanel.Visibility = Visibility.Visible;
        }

        private void VerbergFout()
        {
            FoutPanel.Visibility = Visibility.Collapsed;
        }

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            Person nieuw = LeesFormulier();
            if (!nieuw.IsGeldig(out string fout))
            {
                ToonFout(fout);
                return;
            }

            nieuw.Id = AppData.NieuwLedenId();
            AppData.Leden.Add(nieuw);
            PasFilterToe();
            LeegmakFormulier();
        }

        private void BtnWijzigen_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerdLid == null) return;

            Person gewijzigd = LeesFormulier();
            if (!gewijzigd.IsGeldig(out string fout))
            {
                ToonFout(fout);
                return;
            }

            int index = AppData.Leden.IndexOf(_geselecteerdLid);
            if (index < 0) return;

            gewijzigd.Id = _geselecteerdLid.Id;
            AppData.Leden[index] = gewijzigd;

            PasFilterToe();
            LeegmakFormulier();
        }

        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerdLid == null) return;

            var keuze = MessageBox.Show(
                $"Ben je zeker dat je {_geselecteerdLid.VolledigeNaam} wil verwijderen?",
                "Bevestig verwijdering",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (keuze != MessageBoxResult.Yes) return;

            AppData.Leden.Remove(_geselecteerdLid);
            PasFilterToe();
            LeegmakFormulier();
        }

        private void BtnLeegmaken_Click(object sender, RoutedEventArgs e)
        {
            LeegmakFormulier();
        }

        private void BtnImporteer_Click(object sender, RoutedEventArgs e)
        {
            var importLeden = CsvHelper.ImporteerLeden();
            if (importLeden == null) return;

            foreach (var lid in importLeden)
            {
                lid.Id = AppData.NieuwLedenId();
                AppData.Leden.Add(lid);
            }
            PasFilterToe();
        }

        private void BtnExporteer_Click(object sender, RoutedEventArgs e)
        {
            CsvHelper.ExporteerLeden(AppData.Leden);
        }
    }
}