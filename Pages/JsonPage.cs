using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Globalization;
using System.Text.Json;

namespace BautzPages.Pages
{
    public class JsonPage : PageModel
    {
        public JsonPage()
        {

        }
        public static List<Data>? jsonInitialize(string set)
        {
            //if (set == null) { var json = System.IO.File.ReadAllText(set + ".json"); }
            //else {
            var json = System.IO.File.ReadAllText(set + ".json");
            //}

            if (json != null) { return JsonSerializer.Deserialize<List<Data>>(json); }
            else { return null; }
        }
        public static void jsonExporter(List<Data> l_m, List<Data> alle, string p = "")
        {
            foreach (var item in alle)
            {
                item.Active = false;

                if (l_m.Count > 0)
                {
                    int calendarWeek = GetKW(DateTime.Now);

                    if (!System.IO.File.Exists("TeamKW_" + calendarWeek + ".json") || p.Equals("y"))
                    {
                        // Daten in JSON serialisieren
                        string json = JsonSerializer.Serialize(l_m);
                        string jsonA = JsonSerializer.Serialize(alle);

                        // JSON in eine Datei schreiben
                        System.IO.File.WriteAllText("TeamKW_" + calendarWeek + ".json", json);
                        System.IO.File.WriteAllText("data.json", jsonA);
                    }



                }
                else { return; }

            }
        }
            public static int GetKW(DateTime time)
            {
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                Calendar cal = dfi.Calendar;
                return cal.GetWeekOfYear(time, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            }
        }
    } 
