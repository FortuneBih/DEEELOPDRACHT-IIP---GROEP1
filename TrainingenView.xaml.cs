using System;
using System.Windows;
using System.Windows.Controls;
using BubbelvriendWPF.Data;
using BubbelvriendWPF.Models;

namespace BubbelvriendWPF.Views
{
    public partial class TrainingenView : Page
    {
        private Training _geselecteerdeTraining = null;

        public TrainingenView()
        {
            InitializeComponent();
            DgTrainingen.ItemsSource = AppData.Trainingen;
        }

        // ─── SELECTIE ─────────────────────────────────────────

        private void DgTrainingen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _geselecteerdeTraining = DgTrainingen.SelectedItem as Training;
            BtnVerwijderTraining.IsEnabled = _geselecteerdeTraining != null;

            if (_geselecteerdeTraining != null)
            {
                VulFormulierIn(_geselecteerdeTraining);
                LaadInschrijvingen(_geselecteerdeTraining);
            }
        }

        private void LaadInschrijvingen(Training t)
        {
            // Toon enkel de nog niet geaccepteerde inschrijvingen
            var wachtenden = new System.Collections.ObjectModel.ObservableCollection<Person>();
            foreach (var p in t.Inschrijvingen)
            {
                bool alGeaccepteerd = t.Geaccepteerd.Exists(g => g.Id == p.Id);
                if (!alGeaccepteerd)
                    wachtenden.Add(p);
            }

            DgInschrijvingen.ItemsSource = wachtenden;
            TxtInschrijvingenTitel.Text = $"Inschrijvingen voor '{t.Naam}' " +
                $"({t.Geaccepteerd.Count}/{t.AantalPlaatsen} geaccepteerd" +
                (t.IsVol ? " – VOL" : "") + ")";

            BtnAccepteer.IsEnabled = !t.IsVol && wachtenden.Count > 0;
        }

        // ─── ACCEPTEREN ───────────────────────────────────────

        private void BtnAccepteer_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerdeTraining == null) return;
            var geselecteerdePersoon = DgInschrijvingen.SelectedItem as Person;

            if (geselecteerdePersoon == null)
            {
                MessageBox.Show("Selecteer eerst een ingeschreven persoon om te accepteren.",
                    "Geen selectie", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_geselecteerdeTraining.IsVol)
            {
                MessageBox.Show("Deze training is vol. Er kunnen geen deelnemers meer geaccepteerd worden.",
                    "Training vol", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _geselecteerdeTraining.Geaccepteerd.Add(geselecteerdePersoon);
            // Noot: sms en e-mail versturen hoeft niet geïmplementeerd te worden.

            LaadInschrijvingen(_geselecteerdeTraining);
            DgTrainingen.Items.Refresh(); // status bijwerken (VOL-indicator)

            MessageBox.Show(
                $"{geselecteerdePersoon.VolledigeNaam} is geaccepteerd voor '{_geselecteerdeTraining.Naam}'.\n" +
                $"(In de echte applicatie zou nu een sms en e-mail verstuurd worden.)",
                "Deelnemer geaccepteerd",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ─── FORMULIER ────────────────────────────────────────

        private void VulFormulierIn(Training t)
        {
            TxtNaam.Text          = t.Naam;
            TxtTitel.Text         = t.Titel;
            TxtTags.Text          = t.Tags;
            TxtInhoud.Text        = t.Inhoud;
            TxtStad.Text          = t.Stad;
            TxtProvincie.Text     = t.Provincie;
            TxtAantalPlaatsen.Text = t.AantalPlaatsen.ToString();
            TxtMaxDiepte.Text     = t.MaxDiepte.ToString();
            CmbNiveau.SelectedIndex = t.NiveauSterren - 1;
            DpDatum.SelectedDate  = t.Datum;
            TxtStartuur.Text      = t.Startuur.ToString(@"hh\:mm");
            TxtEinduur.Text       = t.Einduur.ToString(@"hh\:mm");
            VerbergFout();
        }

        private Training LeesFormulier()
        {
            int.TryParse(TxtAantalPlaatsen.Text, out int plaatsen);
            int.TryParse(TxtMaxDiepte.Text, out int diepte);
            TimeSpan.TryParse(TxtStartuur.Text, out TimeSpan start);
            TimeSpan.TryParse(TxtEinduur.Text, out TimeSpan eind);

            return new Training
            {
                Naam          = TxtNaam.Text.Trim(),
                Titel         = TxtTitel.Text.Trim(),
                Tags          = TxtTags.Text.Trim(),
                Inhoud        = TxtInhoud.Text.Trim(),
                Stad          = TxtStad.Text.Trim(),
                Provincie     = TxtProvincie.Text.Trim(),
                AantalPlaatsen = plaatsen,
                MaxDiepte     = diepte,
                NiveauSterren = (CmbNiveau.SelectedIndex >= 0) ? CmbNiveau.SelectedIndex + 1 : 0,
                Datum         = DpDatum.SelectedDate ?? DateTime.Today,
                Startuur      = start,
                Einduur       = eind
            };
        }

        private void LeegmakFormulier()
        {
            TxtNaam.Clear(); TxtTitel.Clear(); TxtTags.Clear();
            TxtInhoud.Clear(); TxtStad.Clear(); TxtProvincie.Clear();
            TxtAantalPlaatsen.Clear(); TxtMaxDiepte.Clear();
            TxtStartuur.Clear(); TxtEinduur.Clear();
            CmbNiveau.SelectedIndex = -1;
            DpDatum.SelectedDate = null;
            _geselecteerdeTraining = null;
            DgTrainingen.SelectedItem = null;
            BtnVerwijderTraining.IsEnabled = false;
            DgInschrijvingen.ItemsSource = null;
            TxtInschrijvingenTitel.Text = "Inschrijvingen (selecteer een training)";
            BtnAccepteer.IsEnabled = false;
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

        // ─── KNOPPEN ──────────────────────────────────────────

        private void BtnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            Training nieuw = LeesFormulier();
            if (!nieuw.IsGeldig(out string fout))
            {
                ToonFout(fout);
                return;
            }

            if (_geselecteerdeTraining != null)
            {
                // Wijzigen van bestaande training
                int index = AppData.Trainingen.IndexOf(_geselecteerdeTraining);
                if (index >= 0)
                {
                    nieuw.Id = _geselecteerdeTraining.Id;
                    nieuw.Inschrijvingen = _geselecteerdeTraining.Inschrijvingen;
                    nieuw.Geaccepteerd = _geselecteerdeTraining.Geaccepteerd;
                    nieuw.Sessies = _geselecteerdeTraining.Sessies;
                    AppData.Trainingen[index] = nieuw;
                }
            }
            else
            {
                // Nieuwe training toevoegen
                nieuw.Id = AppData.NieuwTrainingId();
                AppData.Trainingen.Add(nieuw);
            }

            LeegmakFormulier();
        }

        private void BtnVerwijderTraining_Click(object sender, RoutedEventArgs e)
        {
            if (_geselecteerdeTraining == null) return;

            var keuze = MessageBox.Show(
                $"Ben je zeker dat je de training '{_geselecteerdeTraining.Naam}' wil verwijderen?",
                "Bevestig verwijdering", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (keuze != MessageBoxResult.Yes) return;

            AppData.Trainingen.Remove(_geselecteerdeTraining);
            LeegmakFormulier();
        }

        private void BtnLeegmaken_Click(object sender, RoutedEventArgs e)
        {
            LeegmakFormulier();
        }
    }
}
