using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text.Json;

namespace BautzPages.Pages
{
    public class JsonPage : PageModel
    {
        public JsonPage()
        {
            
        }
        public static List<Data>? jsonInitialize()
        {
            var json = System.IO.File.ReadAllText("data.json");
            if (json != null) { return JsonSerializer.Deserialize<List<Data>>(json); }
            else { return null; }
        }
    }
}
