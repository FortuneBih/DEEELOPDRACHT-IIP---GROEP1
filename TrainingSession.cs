using System;

namespace BubbelvriendWPF.Models
{
    public class TrainingSession
    {
        public int Id { get; set; }
        public Training Training { get; set; }
        public Person Duiker1 { get; set; }
        public Person Duiker2 { get; set; }

        public int TotaalSterren => (Duiker1?.Sterren ?? 0) + (Duiker2?.Sterren ?? 0);

        //  Om te controleren of de duo geldig is op basis van de sterrenregels
        // Regel: som >= 5 sterren, OF som = 4 als diepte <= 20m
        public bool IsGeldigTeam(out string foutmelding)
        {
            if (Duiker1 == null || Duiker2 == null)
            { foutmelding = "Selecteer twee duikers."; return false; }

            if (Duiker1.Id == Duiker2.Id)
            { foutmelding = "Beide duikers mogen niet dezelfde persoon zijn."; return false; }

            int som = TotaalSterren;
            int maxDiepte = Training?.MaxDiepte ?? 0;

            if (som >= 5)
            { foutmelding = string.Empty; return true; }

            if (som == 4 && maxDiepte <= 20)
            { foutmelding = string.Empty; return true; }

            if (som == 4)
                foutmelding = $"Een team van {Duiker1.Sterren}+{Duiker2.Sterren} sterren mag alleen bij een diepte van maximum 20 meter. " +
                              $"Deze training heeft een maximale diepte van {maxDiepte}m.";
            else
                foutmelding = $"Het team ({Duiker1.Sterren}★ + {Duiker2.Sterren}★ = {som} sterren) " +
                              $"voldoet niet aan de eis van minimum 5 sterren.";

            return false;
        }

        public string Weergave => $"{Duiker1?.VolledigeNaam ?? "?"} ({Duiker1?.Sterren}★) + " +
                                  $"{Duiker2?.VolledigeNaam ?? "?"} ({Duiker2?.Sterren}★) = {TotaalSterren}★";

        public override string ToString() => Weergave;
    }
}
