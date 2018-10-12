using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using ConnectingUsWebApp.Models;

namespace ConnectingUsWebApp.Repositories
{
    public class CitiesRepository
    {
        readonly SqlConnection connection;
        SqlCommand command;

        public CitiesRepository()
        {
            var constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }

        public City GetCity(int idCity, int idCountry)
        {
            var city = new City();

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "select * from cities where id_city = @id_city and id_country = @id_country"
            };
            command.Parameters.AddWithValue("@id_city", idCity);
            command.Parameters.AddWithValue("@id_country", idCountry);
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    city = MapCityFromDB(reader);

                }
            }
            connection.Close();

            return city;
        }

        public List<City> GetCountries()
        {
            List<City> countries = new List<City>();

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "select * from countries"
            };
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    countries.Add(MapCityFromDB(reader));

                }
            }
            connection.Close();

            return countries;
        }

        public City MapCityFromDB(SqlDataReader reader)
        {
            CountriesRepository countriesRepo = new CountriesRepository();

            var city = new City
            {
                Id = Int32.Parse(reader["id_city"].ToString()),
                Name = reader["name"].ToString(),
                Latitude = reader["latitude"].ToString(),
                Longitude = reader["longitude"].ToString(),
                Country = countriesRepo.GetCountry(Int32.Parse(reader["id_country"].ToString()))
            };

            return city;
        }
    }
}
