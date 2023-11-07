using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.IO;

namespace BautzPages.Pages
{
	public class ListModel : JsonPage
    {
        public List<Data> MyDataSet;
		public Data MyData { get; set; }

        public ListModel()
        {
            MyDataSet = jsonInitialize();
        }



        /*public async Task<IActionResult> OnGetAsync(int? id,)
		{
			if (id == null)
			{
				return NotFound();
			}

			await foreach (var item in MyDataSet)
			{
				if (item.Id == id)
				{
					MyData = item;
					break;
				}
			}

			if (MyData == null)
			{
				return NotFound();
			}
			return Page();
		}*/
    }
}
