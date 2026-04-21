using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using BubbelvriendWPF.Models;
using Microsoft.Win32;

namespace BubbelvriendWPF.Data
{
    public static class CsvHelper
    {
        private const string CsvHeader = "Id,Naam,Voornaam,Rijksregisternummer,Straat,Huisnummer,Postcode,Gemeente,Telefoonnummer,Email,Sterren";

        // Exporteer leden naar een CSV-bestand via SaveFileDialog
        public static void ExporteerLeden(IEnumerable<Person> leden)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Leden exporteren naar CSV",
                Filter = "CSV-bestanden (*.csv)|*.csv|Alle bestanden (*.*)|*.*",
                DefaultExt = "csv",
                FileName = $"leden_{DateTime.Now:yyyyMMdd}"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                using (var writer = new StreamWriter(dialog.FileName, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine(CsvHeader);
                    foreach (var lid in leden)
                        writer.WriteLine(lid.NaarCsv());
                }
                MessageBox.Show($"Leden succesvol geëxporteerd naar:\n{dialog.FileName}",
                    "Export geslaagd", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij exporteren:\n{ex.Message}",
                    "Exportfout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Importeer leden vanuit een CSV-bestand via OpenFileDialog
        public static List<Person> ImporteerLeden()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Leden importeren vanuit CSV",
                Filter = "CSV-bestanden (*.csv)|*.csv|Alle bestanden (*.*)|*.*",
                DefaultExt = "csv"
            };

            if (dialog.ShowDialog() != true) return null;

            var resultaat = new List<Person>();
            int aantalFouten = 0;

            try
            {
                using (var reader = new StreamReader(dialog.FileName, System.Text.Encoding.UTF8))
                {
                    string eerste = reader.ReadLine(); // sla de headerrij over
                    if (eerste == null) return resultaat;

                    string regel;
                    int rijnummer = 2;
                    while ((regel = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(regel)) continue;
                        try
                        {
                            resultaat.Add(Person.VanCsv(regel));
                        }
                        catch
                        {
                            aantalFouten++;
                        }
                        rijnummer++;
                    }
                }

                string boodschap = $"{resultaat.Count} leden succesvol geïmporteerd.";
                if (aantalFouten > 0)
                    boodschap += $"\n{aantalFouten} rijen konden niet worden ingelezen (ongeldig formaat).";

                MessageBox.Show(boodschap, "Import voltooid", MessageBoxButton.OK,
                    aantalFouten > 0 ? MessageBoxImage.Warning : MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij importeren:\n{ex.Message}",
                    "Importfout", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return resultaat;
        }
    }
}
