using System.Windows;
using System.Windows.Controls;

namespace Bubbelvrienden
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // =========================
        // LEDEN
        // =========================

        private void btnToevoegen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnWijzigen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnLeegmaken_Click(object sender, RoutedEventArgs e)
        {
            txtNaam.Text = "";
            txtVoornaam.Text = "";
            txtRijksregister.Text = "";
            txtStraat.Text = "";
            txtHuisnummer.Text = "";
            txtPostcode.Text = "";
            txtGemeente.Text = "";
            txtTelefoon.Text = "";
            txtEmail.Text = "";
            cmbSterren.SelectedIndex = -1;
        }

        private void btnZoekenLeden_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnResetLeden_Click(object sender, RoutedEventArgs e)
        {
            txtZoekenLeden.Text = "";
        }

        private void btnSterVerhogen_Click(object sender, RoutedEventArgs e)
        {
        }

        // =========================
        // TRAININGEN
        // =========================

        private void btnTrainingToevoegen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnTrainingWijzigen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnTrainingVerwijderen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnTrainingLeegmaken_Click(object sender, RoutedEventArgs e)
        {
            txtTrainingNaam.Text = "";
            txtTrainingTitel.Text = "";
            txtTags.Text = "";
            txtInhoud.Text = "";
            txtStad.Text = "";
            txtProvincie.Text = "";
            txtAantalPlaatsen.Text = "";
            cmbNiveau.SelectedIndex = -1;
            txtDiepte.Text = "";
            dpDatum.SelectedDate = null;
            txtStartuur.Text = "";
            txtEinduur.Text = "";
        }

        private void btnZoekenTrainingen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnResetTrainingen_Click(object sender, RoutedEventArgs e)
        {
            txtZoekenTrainingen.Text = "";
        }

        private void btnVolStatus_Click(object sender, RoutedEventArgs e)
        {
        }

        // =========================
        // TRAININGSSESSIES
        // =========================

        private void btnSessieMaken_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnSessieVerwijderen_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnControleerVoorwaarden_Click(object sender, RoutedEventArgs e)
        {
        }

        // =========================
        // IMPORT / EXPORT
        // =========================

        private void btnImporteerCsv_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnExporteerCsv_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}