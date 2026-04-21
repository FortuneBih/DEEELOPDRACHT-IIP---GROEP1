using System;
using System.Collections.Generic;
using System.Linq;

namespace BubbelvriendWPF.Models
{
    public class Training
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Titel { get; set; }
        public string Tags { get; set; }
        public string Inhoud { get; set; }
        public string Stad { get; set; }
        public string Provincie { get; set; }
        public int AantalPlaatsen { get; set; }
        public int NiveauSterren { get; set; } // vereist niveau
        public int MaxDiepte { get; set; }     // in meters
        public DateTime Datum { get; set; }
        public TimeSpan Startuur { get; set; }
        public TimeSpan Einduur { get; set; }

        // Lijst van ingeschreven personen 
        public List<Person> Inschrijvingen { get; set; } = new List<Person>();

        // Lijst van geaccepteerde deelnemers
        public List<Person> Geaccepteerd { get; set; } = new List<Person>();

        // Sessies (duo's) binnen deze training
        public List<TrainingSession> Sessies { get; set; } = new List<TrainingSession>();

        // Is de training vol?
        public bool IsVol => Geaccepteerd.Count >= AantalPlaatsen;

        // Hoeveel plaatsen zijn er nog vrij?
        public int VrijePlaatsen => Math.Max(0, AantalPlaatsen - Geaccepteerd.Count);

        public string DatumWeergave => Datum.ToString("dd/MM/yyyy");
        public string UrenWeergave => $"{Startuur:hh\\:mm} - {Einduur:hh\\:mm}";
        public string VolStatusWeergave => IsVol ? "VOL" : $"{VrijePlaatsen} plaatsen vrij";

        public bool IsGeldig(out string foutmelding)
        {
            if (string.IsNullOrWhiteSpace(Naam))
            { foutmelding = "Naam is verplicht."; return false; }

            if (string.IsNullOrWhiteSpace(Titel))
            { foutmelding = "Titel is verplicht."; return false; }

            if (string.IsNullOrWhiteSpace(Stad))
            { foutmelding = "Stad is verplicht."; return false; }

            if (string.IsNullOrWhiteSpace(Provincie))
            { foutmelding = "Provincie is verplicht."; return false; }

            if (AantalPlaatsen <= 0)
            { foutmelding = "Aantal plaatsen moet groter zijn dan 0."; return false; }

            if (NiveauSterren < 1 || NiveauSterren > 5)
            { foutmelding = "Niveau (sterren) moet tussen 1 en 5 liggen."; return false; }

            if (MaxDiepte <= 0)
            { foutmelding = "Maximale diepte moet groter zijn dan 0."; return false; }

            if (Startuur >= Einduur)
            { foutmelding = "Startuur moet vóór het einduur liggen."; return false; }

            foutmelding = string.Empty;
            return true;
        }

        // Geeft terug of een persoon al ingeschreven is
        public bool IsAlIngeschreven(Person p) =>
            Inschrijvingen.Any(x => x.Id == p.Id) || Geaccepteerd.Any(x => x.Id == p.Id);

        public override string ToString() => $"{Naam} – {DatumWeergave}";
    }
}
