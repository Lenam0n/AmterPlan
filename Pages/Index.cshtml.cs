using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.IO;
using System.Collections;
using NuGet.Packaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using BautzPages;

namespace BautzPages.Pages
{
    public class IndexModel : JsonPage
    {
        public List<Data> MyDataSet;
        public List<Data> MitarbeiterListe;
        public List<Data> MitarbeiterListe_Krank;
        public ObservableList<Data> MuellTeam;
        public ObservableList<Data> KucheTeam;
        public List<Data> Team;

        public List<SelectListItem> Types;
        [BindProperty]
        public int krankePerson {  get; set; }

        public IndexModel() {
            MyDataSet = jsonInitialize();
            MitarbeiterListe = new List<Data>();
            //krankePerson = 0;
            foreach (var item in MyDataSet)
            {
                MitarbeiterListe.Add(item);
            }

            
            MitarbeiterListe_Krank = new List<Data>();

            MuellTeam = new ObservableList<Data>();
            KucheTeam = new ObservableList<Data>();

            Team = new List<Data>();

            Types = new List<SelectListItem>
            {
                new SelectListItem { Text = "Müll", Value = "Müll" },
                new SelectListItem { Text = "Küche", Value = "Müll" }
            };

            if (MyDataSet.Count > 0)
            {
                List<Data> TempList = new List<Data>
                {
                    MitarbeiterSelection("Müll"),
                    MitarbeiterSelection("Müll")
                };
                MuellTeam.Add(TempList[0]);
                MuellTeam.Add(TempList[1]);
                TempList = new List<Data>
                {
                    MitarbeiterSelection("Küche"),
                    MitarbeiterSelection("Küche")
                };
                KucheTeam.Add(TempList[0]);
                KucheTeam.Add(TempList[1]);
                Team.Add(MuellTeam[0]);
                Team.Add(MuellTeam[1]);
                Team.Add(KucheTeam[0]);
                Team.Add(KucheTeam[1]);
            }

            KucheTeam.ListChanged += (sender, e) =>
            {
                // Handle the list change event by updating the Team list
                Team.Clear();
                Team.AddRange(KucheTeam);
                Team.AddRange(MuellTeam);
            };            
            MuellTeam.ListChanged += (sender, e) =>
            {
                // Handle the list change event by updating the Team list
                Team.Clear();
                Team.AddRange(KucheTeam);
                Team.AddRange(MuellTeam);
            };

        }


        public void OnPostKrankMelden()
        {
            Random random = new Random();
            krankePerson = int.Parse(Console.ReadLine());

            //if (krankePerson == 0) {  return RedirectToPage(); }
            var result = MyDataSet.FirstOrDefault(item => item.ID == krankePerson);
            //if (result == null ) { return RedirectToPage(); }
            result.Krank.Add(DateTime.Now);
            if(KucheTeam.Contains(result)) 
            {
                KucheTeam.Remove(result);
                result.Active = false;
                KucheTeam.Add(MitarbeiterSelection("Küche"));
            }
            if(MuellTeam.Contains(result))  {
                result.Active = false;
                MuellTeam.Remove(result);
                MuellTeam.Add(MitarbeiterSelection("Müll")); 
            }


            //return Page();
        }


        public Data MitarbeiterSelection(string type)
        {
            Data selectedPerson = null;

            switch (type)
            {
                case "Küche":
                    MitarbeiterListe.Sort((p1, p2) => p1.Kueche.CompareTo(p2.Kueche));
                    break;
                case "Müll":
                    MitarbeiterListe.Sort((p1, p2) => p1.Muell.CompareTo(p2.Muell));
                    break;
            }
            var currentDate = DateTime.Now;
            // Filtern der Personen, bei denen "Aktiv" nicht true ist
            var filteredPeople = MitarbeiterListe.Where(p => !p.Active 
                                                             && (p.Krank?.Count == 0
                                                             || p.Krank.Any(krankTag => (currentDate - krankTag).TotalDays > 6) )).ToList();

            if (filteredPeople.Count >= 1)
            {
                // Zufällige Auswahl von zwei Personen
                Random random = new Random();
                var selectedIndex = random.Next(0, filteredPeople.Count);
                selectedPerson = filteredPeople[selectedIndex];
                selectedPerson.Active = true;

            }
            return selectedPerson;
        }

    }





    


    /*
    public class MuellComparer : IEqualityComparer<Data>
    {
        public bool Equals(Data x, Data y)
        {
            return x.Muell == y.Muell;
        }

        public int GetHashCode(Data obj)
        {
            return obj.Muell.GetHashCode();
        }
    }

    public class KucheComparer : IEqualityComparer<Data>
    {
        public bool Equals(Data x, Data y)
        {
            return x.Kueche == y.Kueche;
        }

        public int GetHashCode(Data obj)
        {
            return obj.Muell.GetHashCode();
        }
    }*/







    /*{ }  

        static void NichtDa(Mitarbeiter m, string e)
        {
            switch (e)
            {
                case "Küche":
                    m.Kueche += 1;
                    m.Krank.Add(DateTime.Now);
                    break;
                case "Müll":
                    m.Muell += 1;
                    m.Krank.Add(DateTime.Now);
                    break;
            }
        }
        static void Gemacht(Mitarbeiter m, string e)
        {
            switch (e)
            {
                case "Küche":
                    m.Kueche -= 1;
                    m.KuecheGemacht.Add(DateTime.Now);
                    break;
                case "Müll":
                    m.MuellGemacht.Add(DateTime.Now);
                    m.Muell -= 1;
                    break;
            }
        }

    }
    }*/

}