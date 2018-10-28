using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectingUsWebApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryCode { get; set; }
        public string Description { get; set; }
    }
}