using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace BautzPages
{
    public class Data
    {
		public int ID { get; set; } 
		[Display(Name = "Name")]
        public string Name { get; set; }
		public int Muell { get; set; }
        public int Kueche { get; set; }
		[Display(Name = "Wann wurde Müll gemacht")]
        public List<DateTime>? MuellGemacht { get; set; }
		[Display(Name = "Wann wurde Küche gemacht")]
        public List<DateTime>? KuecheGemacht { get; set; }
		[Display(Name = "Krank Tag")]
        public List<DateTime>? Krank { get; set; }
        public bool Active { get; set; }
        public string currentAmt { get; set; }

    }
}
