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

        public List<City> GetCities(int idCountry)
        {
            List<City> cities = new List<City>();

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "select * from cities where id_country = @id_country order by name"
            };
            command.Parameters.AddWithValue("@id_country", idCountry);
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    cities.Add(MapCityFromDB(reader));

                }
            }
            connection.Close();

            return cities;
        }

        public City MapCityFromDB(SqlDataReader reader)
        {
            var city = new City
            {
                Id = Int32.Parse(reader["id_city"].ToString()),
                Name = reader["name"].ToString(),
                //Latitude = reader["latitude"].ToString(),
                //Longitude = reader["longitude"].ToString()
               
            };

            return city;
        }
    }
}
