using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.IO;
using System.Collections;
using NuGet.Packaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using BautzPages;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace BautzPages.Pages
{
    public class IndexModel : JsonPage
    {
        public List<Data> MyDataSet;
        public List<Data> MitarbeiterListe;
        public ObservableList<Data> MuellTeam;
        public ObservableList<Data> KucheTeam;
        public List<Data> Team;

        public List<SelectListItem> Types;
        [BindProperty]
        public int krankePerson {  get; set; }
        public IndexModel() {
            MyDataSet = jsonInitialize("data");
            MitarbeiterListe = new List<Data>();
            foreach (var item in MyDataSet)
            {
                item.currentAmt = "";
                MitarbeiterListe.Add(item);
            }

            Types = new List<SelectListItem>
            {
                new SelectListItem { Text = "Müll", Value = "Müll" },
                new SelectListItem { Text = "Küche", Value = "Müll" }
            };

            MuellTeam = new ObservableList<Data>();
            KucheTeam = new ObservableList<Data>();

            Team = new List<Data>();
            int calendarWeek = GetKW(DateTime.Now);

            if (!System.IO.File.Exists("TeamKW_" + calendarWeek + ".json"))
            {
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

                foreach (var item in MuellTeam)
                {
                    item.Muell += 1;
                    item.Active = true;
                    item.currentAmt = "mull";
                    item.MuellGemacht.Add(DateTime.Now);
                }            
                foreach (var item in KucheTeam)
                {
                    item.Kueche += 1;
                    item.Active = true;
                    item.currentAmt = "kuche";
                    item.KuecheGemacht.Add(DateTime.Now);
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
                jsonExporter(Team, MitarbeiterListe);
            }
            else
            {
                Team = jsonInitialize("TeamKW_" + GetKW(DateTime.Now));

                foreach(var item in Team)
                {
                    switch (item.currentAmt)
                    {
                        case "mull":
                            MuellTeam.Add(item);
                            break;
                        case "kuche":
                            KucheTeam.Add(item);
                            break;
                    }
                }
            }

        }


        public IActionResult OnPostKrankMelden()
        {
            var result = Team.FirstOrDefault(item => item.ID == krankePerson);
            result.Krank.Add(DateTime.Now);
            
            if(KucheTeam.Contains(result)) 
            {
                KucheTeam.Remove(result);
                result.Kueche -= 1;
                result.KuecheGemacht.Remove(result.KuecheGemacht[result.KuecheGemacht.Count -1]);
                KucheTeam.Add(MitarbeiterSelection("Küche"));
                if (KucheTeam[KucheTeam.Count - 1] == null)
                {
                    return RedirectToPage("./error");
                }
                MuellTeam[MuellTeam.Count - 1].Active = true;
                KucheTeam[KucheTeam.Count - 1].Kueche += 1;
                KucheTeam[KucheTeam.Count - 1].currentAmt = "mull";
                KucheTeam[KucheTeam.Count - 1].KuecheGemacht.Add(DateTime.Now);

            }
            if(MuellTeam.Contains(result))  
            {
                MuellTeam.Remove(result);
                result.Muell -= 1;
                result.MuellGemacht.Remove(result.MuellGemacht[result.MuellGemacht.Count -1]);
                MuellTeam.Add(MitarbeiterSelection("Müll"));
                if (MuellTeam[MuellTeam.Count - 1] == null)
                {
                    return RedirectToPage("./error");
                }
                MuellTeam[MuellTeam.Count - 1].Active = true;
                MuellTeam[MuellTeam.Count - 1].Muell += 1;
                MuellTeam[MuellTeam.Count - 1].currentAmt = "mull";
                MuellTeam[MuellTeam.Count - 1].MuellGemacht.Add(DateTime.Now);
            }
            result.Active = false;
            Team.Clear();
            Team.Add(MuellTeam[0]);
            Team.Add(MuellTeam[1]);
            Team.Add(KucheTeam[0]);
            Team.Add(KucheTeam[1]);

            if(Team.Count >= 4) { jsonExporter(Team, MitarbeiterListe, "y"); }
            
            

            return Page();
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

