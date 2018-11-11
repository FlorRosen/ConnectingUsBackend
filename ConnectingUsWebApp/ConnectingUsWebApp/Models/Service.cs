using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectingUsWebApp.Models
{
    public class Service
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public String Title { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public bool Active { get; set; }
        public Country Country { get; set; }
        public City City { get; set; }
      //  public String Image { get; set; }
        public List<Image> Images { get; set; }
    }
}