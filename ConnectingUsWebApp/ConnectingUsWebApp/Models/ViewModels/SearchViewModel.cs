using System;
using System.Collections.Generic;

namespace ConnectingUsWebApp.Models.ViewModels
{
    public class SearchViewModel
    {
        public List<int> IdCategories { get; set; }
        public string TextForSearch { get; set; }
        public int IdCountry { get; set; }
        public int IdCity { get; set; }
        public int IdUser { get; set; }

    }
}
