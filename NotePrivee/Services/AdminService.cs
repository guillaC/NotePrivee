using ChartJSCore.Helpers;
using ChartJSCore.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NotePrivee.Models;

namespace NotePrivee.Services
{
    public class AdminService
    {
        /// <summary>
        /// Permet de hasher une entrée avec l'algorithme SHA1, utilisée au moment de l'enregistrement/connexion/édition d'un utilisateur
        /// <param name="data">Str à hasher</param>
        /// </summary>
        public static string Hash(string data)
        {
            var sha1 = SHA1.Create();
            byte[] result = sha1.ComputeHash(Encoding.ASCII.GetBytes(data));
            return BitConverter.ToString(result).ToLower().Replace("-", "");
        }

        /// <summary>
        /// Retourne le graphique intégré à l'espace administrauter: nombre de nouvelles notes par mois
        /// </summary>
        /// <param name="nbMonths">Nombre de mois avant celui en cours</param>
        /// <param name="Notes">Liste de notes</param>
        public static Chart GenerateChartNotes(int nbMonths, List<Note> Notes)
        {
            List<string> monthLabels = new List<string>();
            List<double?> noteCounter = new List<double?>();
            DateTime dateVar = DateTime.Now;

            for (int i = 0; i < nbMonths; i++) // Création d'une liste de mois, contenant les n mois inférieurs à celui en cours
            {
                monthLabels.Add(dateVar.ToString("MM/yyyy"));
                dateVar = dateVar.AddMonths(-1);
            }
            monthLabels.Reverse();

            foreach (string monthY in monthLabels) // Création du set de données du graphique
            {
                int nbNotes = 0;
                foreach (Note note in Notes)
                {
                    if (note.DateCreation.Month.ToString("00") +"/" + note.DateCreation.Year.ToString("0000") == monthY) nbNotes++;
                }
                noteCounter.Add(nbNotes);
            }

            Chart chart = new Chart
            {
                Type = Enums.ChartType.Line
            };

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data
            {
                Labels = monthLabels
            };

            LineDataset dataset = new LineDataset() // Paramètrage du graphique
            {
                Label = "Créations",
                Data = noteCounter,
                Fill = "false",
                LineTension = 0.1,
                BackgroundColor = ChartColor.FromRgba(75, 192, 192, 0.4),
                BorderColor = ChartColor.FromRgb(75, 192, 192),
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                PointBackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ffffff") },
                PointBorderWidth = new List<int> { 1 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };

            data.Datasets = new List<Dataset>
            {
                dataset
            };

            chart.Data = data;
            return chart;
        }

    }
}
