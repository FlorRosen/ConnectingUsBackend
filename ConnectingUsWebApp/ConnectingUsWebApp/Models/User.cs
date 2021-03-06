﻿using System;

namespace ConnectingUsWebApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Country CountryOfResidence { get; set; }
        public Country CountryOfBirth { get; set; }
        public Account Account { get; set; }
        public DateTime CreateDate { get; set; }
        public string PhoneType { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneAreaCode { get; set; }
        public City CityOfResidence { get; set; }
        public Reputation Reputation { get; set; }

    }
}