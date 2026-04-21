using System;
using System.Text.RegularExpressions;

namespace BubbelvriendWPF.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Voornaam { get; set; }
        public string Rijksregisternummer { get; set; }
        public string Straat { get; set; }
        public string Huisnummer { get; set; }
        public string Postcode { get; set; }
        public string Gemeente { get; set; }
        public string Telefoonnummer { get; set; }
        public string Email { get; set; }
        public int Sterren { get; set; } // 1 t/m 5

        public string VolledigeNaam => $"{Voornaam} {Naam}";
        public string VolledigAdres => $"{Straat} {Huisnummer}, {Postcode} {Gemeente}";
        public string SterrenWeergave => new string('★', Sterren) + new string('☆', 5 - Sterren);

        // Valideer alle basisvelden
        public bool IsGeldig(out string foutmelding)
        {
            if (string.IsNullOrWhiteSpace(Naam))
            { foutmelding = "Naam is verplicht."; return false; }

            if (string.IsNullOrWhiteSpace(Voornaam))
            { foutmelding = "Voornaam is verplicht."; return false; }

            if (string.IsNullOrWhiteSpace(Rijksregisternummer))
            { foutmelding = "Rijksregisternummer is verplicht."; return false; }

            if (!IsGeldigRijksregisternummer(Rijksregisternummer, out string rrFout))
            { foutmelding = rrFout; return false; }

            if (string.IsNullOrWhiteSpace(Straat))
            { foutmelding = "Straat is verplicht."; return false; }

            if (string.IsNullOrWhiteSpace(Huisnummer))
            { foutmelding = "Huisnummer is verplicht."; return false; }

            if (string.IsNullOrWhiteSpace(Postcode))
            { foutmelding = "Postcode is verplicht."; return false; }

            if (!IsGeldigePostcode(Postcode))
            { foutmelding = "Postcode moet een geldig getal zijn van 4 cijfers."; return false; }

            if (string.IsNullOrWhiteSpace(Gemeente))
            { foutmelding = "Gemeente is verplicht."; return false; }

            if (string.IsNullOrWhiteSpace(Telefoonnummer))
            { foutmelding = "Telefoonnummer is verplicht."; return false; }

            if (string.IsNullOrWhiteSpace(Email))
            { foutmelding = "E-mailadres is verplicht."; return false; }

            if (!IsGeldigEmail(Email))
            { foutmelding = "E-mailadres heeft geen geldig formaat."; return false; }

            if (Sterren < 1 || Sterren > 5)
            { foutmelding = "Sterren moet tussen 1 en 5 liggen."; return false; }

            foutmelding = string.Empty;
            return true;
        }

        // Postcode = precies 4 cijfers
        public static bool IsGeldigePostcode(string postcode)
        {
            return Regex.IsMatch(postcode, @"^\d{4}$");
        }

        // Rijksregisternummer basischeck: 11 cijfers
        // Extra: controlegetals-validatie (uitbreiding)
        public static bool IsGeldigRijksregisternummer(string rr, out string fout)
        {
            string alleen = Regex.Replace(rr, @"\D", "");

            if (alleen.Length != 11)
            {
                fout = "Rijksregisternummer moet uit 11 cijfers bestaan.";
                return false;
            }

            // Controleer checksum (uitbreiding)
            if (!HeeftGeldigControlegetal(alleen))
            {
                fout = "Rijksregisternummer heeft een ongeldig controlegetal.";
                return false;
            }

            fout = string.Empty;
            return true;
        }

        // Berekening controlegetal rijksregisternummer
        // Zie: https://nl.wikipedia.org/wiki/Rijksregisternummer
        private static bool HeeftGeldigControlegetal(string rr)
        {
            try
            {
                long basis = long.Parse(rr.Substring(0, 9));
                int controle = int.Parse(rr.Substring(9, 2));

                // Probeer eerst zonder 2 (geboren vóór 2000)
                int rest = (int)(97 - (basis % 97));
                if (rest == controle) return true;

                // Probeer met 2 vooraan (geboren na 1999)
                long basis2000 = long.Parse("2" + rr.Substring(0, 9));
                int rest2000 = (int)(97 - (basis2000 % 97));
                return rest2000 == controle;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsGeldigEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // Omzetten naar CSV-rij
        public string NaarCsv()
        {
            return $"{Id},{EscapeCsv(Naam)},{EscapeCsv(Voornaam)},{EscapeCsv(Rijksregisternummer)}," +
                   $"{EscapeCsv(Straat)},{EscapeCsv(Huisnummer)},{EscapeCsv(Postcode)},{EscapeCsv(Gemeente)}," +
                   $"{EscapeCsv(Telefoonnummer)},{EscapeCsv(Email)},{Sterren}";
        }

        // Aanmaken vanuit CSV-rij
        public static Person VanCsv(string csvRegel)
        {
            string[] delen = csvRegel.Split(',');
            if (delen.Length < 11)
                throw new FormatException("Ongeldige CSV-rij voor persoon.");

            return new Person
            {
                Id = int.TryParse(delen[0], out int id) ? id : 0,
                Naam = delen[1].Trim('"'),
                Voornaam = delen[2].Trim('"'),
                Rijksregisternummer = delen[3].Trim('"'),
                Straat = delen[4].Trim('"'),
                Huisnummer = delen[5].Trim('"'),
                Postcode = delen[6].Trim('"'),
                Gemeente = delen[7].Trim('"'),
                Telefoonnummer = delen[8].Trim('"'),
                Email = delen[9].Trim('"'),
                Sterren = int.TryParse(delen[10], out int s) ? s : 1
            };
        }

        private static string EscapeCsv(string waarde)
        {
            if (string.IsNullOrEmpty(waarde)) return "";
            if (waarde.Contains(",") || waarde.Contains("\""))
                return $"\"{waarde.Replace("\"", "\"\"")}\"";
            return waarde;
        }

        public override string ToString() => VolledigeNaam;
    }
}
