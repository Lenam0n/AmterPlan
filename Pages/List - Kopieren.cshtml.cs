using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.IO;

namespace BautzPages.Pages
{
	public class ListModel1 : JsonPage
	{
        public List<Data> MyDataSet;
		public List<Data> MyData { get; set; }

        public ListModel1()
        {
            MyDataSet = jsonInitialize("data");
        }

		public void OnGet()
		{

		}
		public void OnPost() 
		{

		}
	}
}
