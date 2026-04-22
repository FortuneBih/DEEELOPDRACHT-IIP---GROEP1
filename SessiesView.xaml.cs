using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BubbelvriendWPF.Data;
using BubbelvriendWPF.Models;

namespace BubbelvriendWPF.Views
{
    public partial class SessiesView : Page
    {
        public SessiesView()
        {
            InitializeComponent();
            DgSessies.ItemsSource = AppData.Sessies;
            CmbTraining.ItemsSource = AppData.Trainingen;
        }

        // ─── TRAINING SELECTIE ─────────────────────────────────

        private void CmbTraining_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var training = CmbTraining.SelectedItem as Training;
            if (training == null)
            {
                TrainingDetailsPanel.Visibility = Visibility.Collapsed;
                CmbDuiker1.ItemsSource = null;
                CmbDuiker2.ItemsSource = null;
                return;
            }

            // Toon trainingsdetails
            TxtTrainingDetails.Text =
                $"📅 {training.DatumWeergave}   ⏰ {training.UrenWeergave}\n" +
                $"📍 {training.Stad} ({training.Provincie})   " +
                $"🌊 Max. {training.MaxDiepte}m   ⭐ Niveau: {training.NiveauSterren} sterren\n" +
                $"👥 Plaatsen: {training.Geaccepteerd.Count}/{training.AantalPlaatsen}";

            TrainingDetailsPanel.Visibility = Visibility.Visible;
            VolPanel.Visibility = training.IsVol ? Visibility.Visible : Visibility.Collapsed;

            // Vul de duikerlijsten: toon enkel beschikbare, geaccepteerde leden
            // Extra: filter leden die al gekoppeld zijn aan een sessie voor deze training
            var reedGekoppeld = training.Sessies
                .SelectMany(s => new[] { s.Duiker1?.Id, s.Duiker2?.Id })
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToHashSet();

            var bron = training.Geaccepteerd.Count > 0
    ? training.Geaccepteerd
    : AppData.Leden.ToList();

            var beschikbaren = bron
                .Where(p => !reedGekoppeld.Contains(p.Id))
                .ToList();

            CmbDuiker1.ItemsSource = beschikbaren;
            CmbDuiker2.ItemsSource = beschikbaren;
            CmbDuiker1.SelectedItem = null;
            CmbDuiker2.SelectedItem = null;
            TxtDuiker1Sterren.Text = "";
            TxtDuiker2Sterren.Text = "";
            SterrenPreview.Visibility = Visibility.Collapsed;
            VerbergFout();
        }

        // ─── DUIKER SELECTIE ──────────────────────────────────

        private void CmbDuiker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d1 = CmbDuiker1.SelectedItem as Person;
            var d2 = CmbDuiker2.SelectedItem as Person;

            TxtDuiker1Sterren.Text = d1 != null ? $"{d1.SterrenWeergave} ({d1.Sterren} sterren)" : "";
            TxtDuiker2Sterren.Text = d2 != null ? $"{d2.SterrenWeergave} ({d2.Sterren} sterren)" : "";

            if (d1 != null && d2 != null)
            {
                ToonSterrenPreview(d1, d2);
            }
            else
            {
                SterrenPreview.Visibility = Visibility.Collapsed;
            }
        }

        private void ToonSterrenPreview(Person d1, Person d2)
        {
            var training = CmbTraining.SelectedItem as Training;
            var sessie = new TrainingSession
            {
                Duiker1 = d1,
                Duiker2 = d2,
                Training = training
            };

            int som = sessie.TotaalSterren;
            bool geldig = sessie.IsGeldigTeam(out string fout);

            TxtSterrenPreview.Text =
                $"{d1.Voornaam} ({d1.Sterren}★) + {d2.Voornaam} ({d2.Sterren}★) = {som}★ totaal";

            if (geldig)
            {
                TxtGeldigheidPreview.Text = "✅ Geldig team!";
                TxtGeldigheidPreview.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(46, 125, 50));
            }
            else
            {
                TxtGeldigheidPreview.Text = $"❌ {fout}";
                TxtGeldigheidPreview.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 40, 40));
            }

            SterrenPreview.Visibility = Visibility.Visible;
        }

        // ─── KOPPELEN ─────────────────────────────────────────

        private void BtnKoppelen_Click(object sender, RoutedEventArgs e)
        {
            var training = CmbTraining.SelectedItem as Training;
            var d1 = CmbDuiker1.SelectedItem as Person;
            var d2 = CmbDuiker2.SelectedItem as Person;

            if (training == null)
            { ToonFout("Selecteer eerst een training."); return; }

            if (d1 == null || d2 == null)
            { ToonFout("Selecteer twee duikers."); return; }

            if (d1.Id == d2.Id)
            { ToonFout("Beide duikers mogen niet dezelfde persoon zijn."); return; }

            if (training.IsVol)
            { ToonFout("Deze training is vol."); return; }

            var sessie = new TrainingSession
            {
                Id = AppData.NieuwSessieId(),
                Training = training,
                Duiker1 = d1,
                Duiker2 = d2
            };

            if (!sessie.IsGeldigTeam(out string fout))
            { ToonFout(fout); return; }

            AppData.Sessies.Add(sessie);
            training.Sessies.Add(sessie);

            // Reset de keuzes voor het volgende duo
            CmbTraining_SelectionChanged(null, null);
            VerbergFout();

            MessageBox.Show(
                $"Duo succesvol gekoppeld:\n{sessie.Weergave}\nTraining: {training.Naam}",
                "Sessie aangemaakt",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            CmbTraining.SelectedItem = null;
            CmbDuiker1.ItemsSource = null;
            CmbDuiker2.ItemsSource = null;
            TxtDuiker1Sterren.Text = "";
            TxtDuiker2Sterren.Text = "";
            SterrenPreview.Visibility = Visibility.Collapsed;
            TrainingDetailsPanel.Visibility = Visibility.Collapsed;
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
    }
}
