using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectingUsWebApp.Models
{
    public class Image
    {
        public int Id { get; set; }
        public int? IdService { get; set; }
        public string ImageString { get; set; }
    }
}