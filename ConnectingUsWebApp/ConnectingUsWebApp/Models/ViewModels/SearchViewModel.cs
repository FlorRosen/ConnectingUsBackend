
using System.Collections.Generic;

namespace ConnectingUsWebApp.Models.ViewModels
{
    public class SearchViewModel
    {
        public List<Category> Categories { get; set; }
        public string Text { get; set; }
        public int? IdCountry { get; set; }
        public int? IdCity { get; set; }
        public int? IdUser { get; set; }
        public bool? Active { get; set; }
    }
}
