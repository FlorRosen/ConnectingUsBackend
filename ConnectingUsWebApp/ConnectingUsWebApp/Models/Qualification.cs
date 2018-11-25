﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;


namespace ConnectingUsWebApp.Models
{
    public class Qualification
    {
        public int? IdChat { get; set; }
        public int? UserRequesterId { get; set; }
        public int? UserOfertorId { get; set; }
        public int QualificationNumber { get; set; }
        public String QualificationDate { get; set; }

    }
}