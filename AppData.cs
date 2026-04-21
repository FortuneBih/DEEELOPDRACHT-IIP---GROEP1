using System.Collections.Generic;
using System.Collections.ObjectModel;
using BubbelvriendWPF.Models;
using System;

namespace BubbelvriendWPF.Data
{
    // Centrale dataklas: alle schermen dezelfde klas
    public static class AppData
    {
        public static ObservableCollection<Person> Leden { get; set; } = new ObservableCollection<Person>();
        public static ObservableCollection<Training> Trainingen { get; set; } = new ObservableCollection<Training>();
        public static ObservableCollection<TrainingSession> Sessies { get; set; } = new ObservableCollection<TrainingSession>();

        private static int _volgendLedenId = 1;
        private static int _volgendTrainingId = 1;
        private static int _volgendSessieId = 1;

        public static int NieuwLedenId() => _volgendLedenId++;
        public static int NieuwTrainingId() => _volgendTrainingId++;
        public static int NieuwSessieId() => _volgendSessieId++;

        // Fictieve data laden om werking te zien
        public static void LaadVoorbeeldData()
        {
            // Fictieve leden
            var p1 = new Person { Id = NieuwLedenId(), Naam = "Janssen", Voornaam = "Pieter", Rijksregisternummer = "85073011171", Straat = "Gentseweg", Huisnummer = "12", Postcode = "9000", Gemeente = "Gent", Telefoonnummer = "0478 12 34 56", Email = "pieter.janssen@mail.be", Sterren = 3 };
            var p2 = new Person { Id = NieuwLedenId(), Naam = "De Smedt", Voornaam = "Lisa", Rijksregisternummer = "92010215689", Straat = "Antwerpsestraat", Huisnummer = "4", Postcode = "2000", Gemeente = "Antwerpen", Telefoonnummer = "0476 98 76 54", Email = "lisa.desmedt@gmail.com", Sterren = 2 };
            var p3 = new Person { Id = NieuwLedenId(), Naam = "Peeters", Voornaam = "Thomas", Rijksregisternummer = "78041521345", Straat = "Stationslaan", Huisnummer = "88B", Postcode = "3000", Gemeente = "Leuven", Telefoonnummer = "0469 11 22 33", Email = "thomas.peeters@hotmail.com", Sterren = 5 };
            var p4 = new Person { Id = NieuwLedenId(), Naam = "Claes", Voornaam = "Emma", Rijksregisternummer = "95120344512", Straat = "Molenstraat", Huisnummer = "7", Postcode = "9200", Gemeente = "Dendermonde", Telefoonnummer = "0491 44 55 66", Email = "emma.claes@outlook.com", Sterren = 4 };
            var p5 = new Person { Id = NieuwLedenId(), Naam = "Vermeersch", Voornaam = "Bram", Rijksregisternummer = "88062734521", Straat = "Brugsesteenweg", Huisnummer = "33", Postcode = "8800", Gemeente = "Roeselare", Telefoonnummer = "0483 77 88 99", Email = "bram.vermeersch@mail.be", Sterren = 1 };
            var p6 = new Person { Id = NieuwLedenId(), Naam = "Willems", Voornaam = "Sophie", Rijksregisternummer = "01052912345", Straat = "Leuvenseweg", Huisnummer = "22", Postcode = "3010", Gemeente = "Kessel-Lo", Telefoonnummer = "0475 33 44 55", Email = "sophie.willems@gmail.com", Sterren = 3 };

            Leden.Add(p1); Leden.Add(p2); Leden.Add(p3);
            Leden.Add(p4); Leden.Add(p5); Leden.Add(p6);

            // Fictieve trainingen
            var t1 = new Training
            {
                Id = NieuwTrainingId(),
                Naam = "Introductieduik Merelbeke",
                Titel = "Eerste stappen onder water",
                Tags = "beginner, zwembad, opleiding",
                Inhoud = "Kennismaking met duikmateriaal en basisvaardigheden in het zwembad.",
                Stad = "Merelbeke",
                Provincie = "Oost-Vlaanderen",
                AantalPlaatsen = 8,
                NiveauSterren = 1,
                MaxDiepte = 5,
                Datum = DateTime.Today.AddDays(7),
                Startuur = new TimeSpan(10, 0, 0),
                Einduur = new TimeSpan(12, 0, 0)
            };
            // Fictieve inschrijvingen via website
            t1.Inschrijvingen.AddRange(new[] { p2, p5 });

            var t2 = new Training
            {
                Id = NieuwTrainingId(),
                Naam = "Gevorderd open water – Donkmeer",
                Titel = "Navigatie en diepduiken",
                Tags = "gevorderd, open water, navigatie",
                Inhoud = "Navigatie onder water en verkenning van diepere zones in het Donkmeer.",
                Stad = "Berlare",
                Provincie = "Oost-Vlaanderen",
                AantalPlaatsen = 6,
                NiveauSterren = 3,
                MaxDiepte = 25,
                Datum = DateTime.Today.AddDays(14),
                Startuur = new TimeSpan(9, 0, 0),
                Einduur = new TimeSpan(13, 0, 0)
            };
            t2.Inschrijvingen.AddRange(new[] { p1, p3, p4 });

            var t3 = new Training
            {
                Id = NieuwTrainingId(),
                Naam = "Reddingstechnieken voor instructeurs",
                Titel = "Rescue & emergency procedures",
                Tags = "rescue, instructeur, noodprocedures",
                Inhoud = "Verdieping in reddingstechnieken en noodprocedures voor ervaren duikers.",
                Stad = "Gent",
                Provincie = "Oost-Vlaanderen",
                AantalPlaatsen = 4,
                NiveauSterren = 4,
                MaxDiepte = 40,
                Datum = DateTime.Today.AddDays(21),
                Startuur = new TimeSpan(8, 30, 0),
                Einduur = new TimeSpan(14, 0, 0)
            };
            t3.Inschrijvingen.AddRange(new[] { p3, p4 });
            // T3: accepteer p3 zodat we een voorbeeld van een algeaccepteerde hebben
            t3.Geaccepteerd.Add(p3);

            Trainingen.Add(t1); Trainingen.Add(t2); Trainingen.Add(t3);

            // Update de IDtellers
            _volgendLedenId = 7;
            _volgendTrainingId = 4;
            _volgendSessieId = 1;
        }
    }
}
