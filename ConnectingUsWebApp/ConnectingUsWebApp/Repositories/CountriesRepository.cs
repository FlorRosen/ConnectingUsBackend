using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using ConnectingUsWebApp.Models;

namespace ConnectingUsWebApp.Repositories
{
    public class CountriesRepository
    {
        readonly SqlConnection connection;
        SqlCommand command;

        public CountriesRepository()
        {
            var constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }

        public Country GetCountry(int id){
            var country = new Country();

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "select * from countries where id_country = @id_country"
            };
            command.Parameters.AddWithValue("@id_country", id);
            connection.Open();
            using(SqlDataReader reader = command.ExecuteReader()){
                while(reader.Read()){
                    country.Id = MapCountryFromDB(reader).Id;
                    country.Name = MapCountryFromDB(reader).Name;
                    country.CountryCode = MapCountryFromDB(reader).CountryCode;
                    country.Nationality = MapCountryFromDB(reader).Nationality;

                }
            }
            connection.Close();

            return country;
        }

        public List<Country> GetCountries(){
            List<Country> countries = new List<Country>();

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "select * from countries order by name"
            };
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    countries.Add(MapCountryFromDB(reader));

                }
            }
            connection.Close();

            return countries;
        }

        //Returns the countries where servieces are offer
        public List<Country> GetCountriesOfServices()
        {
            List<Country> countries = new List<Country>();

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "select distinct co.* from countries co " +
                "inner join services_by_users sbu on sbu.id_country = co.id_country "
            };
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    countries.Add(MapCountryFromDB(reader));

                }
            }
            connection.Close();

            return countries;
        }

        public Country MapCountryFromDB(SqlDataReader reader)
        {
            var country = new Country
            {
                Id = Int32.Parse(reader["id_country"].ToString()),
                Name = reader["name"].ToString(),
                Latitude = reader["latitude"].ToString(),
                Longitude = reader["longitude"].ToString(),
                CountryCode = reader["country_code"].ToString(),
                Nationality = reader["nationality"].ToString()
            };

            return country;
        }
    }
}
