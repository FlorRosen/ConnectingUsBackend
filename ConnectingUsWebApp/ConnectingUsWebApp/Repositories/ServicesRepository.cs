using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using ConnectingUsWebApp.Models;
using System.Web.Http;
using ConnectingUsWebApp.Models.ViewModels;

namespace ConnectingUsWebApp.Repositories
{
    public class ServicesRepository
    {
        private SqlConnection connection;
        private SqlCommand command;

        public ServicesRepository()
        {
            string constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }

        //Public Methods
        //This method can bring all services, by user id or serivce id or category that are active
        public List<Service> GetServices(int? idService, int? idUser, int? idCategory)
        {
            List<Service> services = new List<Service>();

           
            String query = "select * from services_by_users where " +
           " ((@id_user IS NULL) OR (@id_user IS NOT NULL AND id_user = @id_user))" +
           " AND ((@id_service IS NULL) OR (@id_service IS NOT NULL AND id_service = @id_service)) " +
            "AND ((@id_category IS NULL) OR (@id_category IS NOT NULL AND id_category = @id_category)) ";
            command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id_service", idService ?? Convert.DBNull);
            command.Parameters.AddWithValue("@id_user", idUser ?? Convert.DBNull);
            command.Parameters.AddWithValue("@id_category", idCategory ?? Convert.DBNull);

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Service service = MapServiceFromDB(reader);
                    services.Add(service);
                }
            }
            connection.Close();
            return services;
        }

        //Public Methods
        //This method can bring all services, by user id or serivce id or category that are active
              public List<Service> Search(SearchViewModel search)
        {
            List<Service> services = new List<Service>();

            try
            {
                String query = "EXEC sp_SearchServices @id_user,@id_country,@id_city,@text,@id_categories,@active";

                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@active", search.Active ?? Convert.DBNull);
                command.Parameters.AddWithValue("@text", search.Text ?? Convert.DBNull);
                command.Parameters.AddWithValue("@id_user", search.IdUser ?? Convert.DBNull);
                command.Parameters.AddWithValue("@id_country", search.IdCountry ?? Convert.DBNull);
                command.Parameters.AddWithValue("@id_city", search.IdCity ?? Convert.DBNull);
                command.Parameters.AddWithValue("@id_categories", getCategoriesId(search.Categories) ?? Convert.DBNull);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Service service = MapServiceFromDB(reader);
                        services.Add(service);
                    }
                }
                connection.Close();
            }
            finally
            {
                connection.Close();

            }
            return services;
        }
        
        //Returns a string with all the categories id concatenated
        private String getCategoriesId(List<Category> categories){
            List<int> categoriesId = null;
            String categoriesQuery = "";
            if (categories != null) {
                categoriesId = new List<int>();
                foreach (Category element in categories)
                {
                    categoriesId.Add(element.Id);
                }
                categoriesQuery = "(" + string.Join(",", categoriesId) + ")";
            }
            return categoriesQuery;
        }
        
        public bool AddService(Service service)
        {
           
            var result = false;

            using (SqlCommand command_addService = new SqlCommand())
            {
                connection.Open();

                command_addService.Connection = connection;

                command_addService.CommandText = "INSERT INTO services_by_users (id_category, id_user,title, description,id_country,id_city,create_date) " +
                 "OUTPUT INSERTED.id_service VALUES (@id_category, @id_user,@title, @description, @id_country, @id_city,getdate())";

                command_addService.Parameters.AddWithValue("@id_category", service.Category.Id);
                command_addService.Parameters.AddWithValue("@id_user", service.UserId);
                command_addService.Parameters.AddWithValue("@title", service.Title);
                command_addService.Parameters.AddWithValue("@description", service.Description);
                command_addService.Parameters.AddWithValue("@id_country", service.Country.Id);
                command_addService.Parameters.AddWithValue("@id_city", service.City.Id);

                SqlParameter param = new SqlParameter("@id_service", SqlDbType.Int, 4)
                {
                    Direction = ParameterDirection.Output
                };
                command_addService.Parameters.Add(param);

                int Id = (int)command_addService.ExecuteScalar();

                //command_addService.ExecuteNonQuery();
                connection.Close();
            /*    if (service.Images != null)
                {
                    AddImages(service.Images, service.Id);
                }*/
                result = true;


            }
            return result;
        }

        //Update the service in the Database
        public bool UpdateService(Service service)
        {
            
            var result = false;
            try
            {
                using (SqlCommand command_UpdateService = new SqlCommand())
                {
                    connection.Open();

                    command_UpdateService.Connection = connection;

                    command_UpdateService.CommandText = "UPDATE services_by_users SET title = @title, description = @description , " +
                     "active = @active , id_category = @id_category , id_city = @id_city , id_country =  @id_country " +
                     "WHERE  id_service = @id_service";

                    command_UpdateService.Parameters.AddWithValue("@id_service", service.Id);
                    command_UpdateService.Parameters.AddWithValue("@description", service.Description);
                    command_UpdateService.Parameters.AddWithValue("@title", service.Title);
                    command_UpdateService.Parameters.AddWithValue("@id_category", service.Category.Id);
                    command_UpdateService.Parameters.AddWithValue("@id_country", service.Country.Id);
                    command_UpdateService.Parameters.AddWithValue("@id_city", service.City.Id);
                    //The active column is a BIT in the database
                    if (service.Active)
                    {
                        command_UpdateService.Parameters.AddWithValue("@active", 1);
                    }
                    else
                    {
                        command_UpdateService.Parameters.AddWithValue("@active", 0);
                    }

                    command_UpdateService.ExecuteNonQuery();
                    connection.Close();
                    result = true;

                }
            }
            finally
            {
                connection.Close();
            }
            return result;
        }


        //Private Methods
        private Service MapServiceFromDB(SqlDataReader reader)
        {

            UsersRepository usersRepository = new UsersRepository();
            CategoriesRepository categoriesRepository = new CategoriesRepository();
            CountriesRepository countriesRepository = new CountriesRepository();
            CitiesRepository citiesRepository = new CitiesRepository();
            ImagesRepository imagesRepository = new ImagesRepository();

            Service service = new Service
            {
                Id = Int32.Parse(reader["id_service"].ToString()),
                Title = reader["title"].ToString(),
                Description = reader["description"].ToString(),
                Active = Boolean.Parse(reader["active"].ToString()),
                UserId = Int32.Parse(reader["id_user"].ToString()),
                Category = categoriesRepository.GetCategory(Int32.Parse(reader["id_category"].ToString())),
                Country = countriesRepository.GetCountry(Int32.Parse(reader["id_country"].ToString())),
                City = citiesRepository.GetCity(Int32.Parse(reader["id_city"].ToString()), Int32.Parse(reader["id_country"].ToString())),
                Image = imagesRepository.GetImages(Int32.Parse(reader["id_service"].ToString())).ImageString,


            };

            return service;
        }
    }
}