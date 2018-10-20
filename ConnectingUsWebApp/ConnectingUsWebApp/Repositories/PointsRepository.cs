using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using ConnectingUsWebApp.Models;
using System.Web.Http;
namespace ConnectingUsWebApp.Repositories
{
    public class PointsRepository
    {
        private SqlConnection connection;
        private SqlCommand command;

        private void CreateConnection()
        {
            string constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }

        //Public Methods
        //This method can bring all services, by user id or serivce id or category that are active
        public List<Service> GetServices(int? idService, int? idUser, int? idCategory)
        {
            List<Service> services = new List<Service>();

            CreateConnection();
            String query = "select * from services_by_users where " +
           " ((@id_user IS NULL) OR (@id_user IS NOT NULL AND id_user = @id_user))" +
           " AND ((@id_service IS NULL) OR (@id_service IS NOT NULL AND id_service = @id_service)) " +
            "AND ((@id_category IS NULL) OR (@id_category IS NOT NULL AND id_category = @id_category)) ";
            command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id_service", idService ?? Convert.DBNull);
            command.Parameters.AddWithValue("@id_user", idUser ?? Convert.DBNull);
            command.Parameters.AddWithValue("@id_category", idCategory ?? Convert.DBNull);
            //  command.Parameters.AddWithValue("@id_service", idService);
            //command.Parameters.AddWithValue("@id_user", idUser);
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Service service = MapPointFromDB(reader);
                    services.Add(service);
                }
            }
            connection.Close();
            return services;
        }

        //Public Methods
        //This method can bring all services, by user id or serivce id or category that are active
        public List<Service> Search(List<string> idCategories, string textForSearch, int? idCountry, int? idCity, int? idUser)
        {
            List<Service> services = new List<Service>();
            //string categories = string.Format(" AND sbu.id_category  in ({0})", string.Join(",", idCategories));


            CreateConnection();
            String query = "select sbu.* from services_by_users sbu INNER JOIN users us on" +
                " us.id_user = sbu.id_user" +
            " WHERE " +
            "   active = 1 " + //A
            " AND ((@id_user IS NULL) OR (@id_user IS NOT NULL AND sbu.id_user = @id_user))" +
            " AND ((@id_country IS NULL) OR (@id_country IS NOT NULL AND us.id_country = @id_country))" +
            " AND ((@id_city IS NULL) OR (@id_city IS NOT NULL AND us.id_city_residence = @id_city))" +
            " AND  ((@text IS NULL) OR (upper(sbu.description) LIKE '%' + @text + '%')) " +
            " @id_categories ";
            //" AND ((@id_category IS NULL) OR (@id_category IS NOT NULL AND id_category = @id_category)) ";


            command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@text", textForSearch.ToUpper() ?? Convert.DBNull);
            command.Parameters.AddWithValue("@id_user", idUser ?? Convert.DBNull);
            command.Parameters.AddWithValue("@id_country", idCountry ?? Convert.DBNull);
            command.Parameters.AddWithValue("@id_city", idCity ?? Convert.DBNull);
            command.Parameters.AddWithValue("@id_categories", string.Format(" AND sbu.id_category  in ({0})", string.Join(",", idCategories)));
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Service service = MapPointFromDB(reader);
                    services.Add(service);
                }
            }
            connection.Close();
            return services;
        }

        public bool AddService(Service service)
        {
            CreateConnection();
            var result = false;

            using (SqlCommand command_addService = new SqlCommand())
            {
                connection.Open();

                command_addService.Connection = connection;

                command_addService.CommandText = "INSERT INTO services_by_users (id_category, id_user, description, active) " +
                 "VALUES (@id_category, @id_user, @description, @active)";

                command_addService.Parameters.AddWithValue("@id_category", service.Category.Id);
                command_addService.Parameters.AddWithValue("@id_user", service.UserId);
                command_addService.Parameters.AddWithValue("@description", service.Description);

                //The active column is a BIT in the database
                if (service.Active)
                {
                    command_addService.Parameters.AddWithValue("@active", 1);
                }
                else
                {
                    command_addService.Parameters.AddWithValue("@active", 0);
                }
                command_addService.ExecuteNonQuery();
                connection.Close();
                result = true;


            }
            return result;
        }

        //Update the service in the Database
        public bool UpdateService(Service service)
        {
            CreateConnection();
            var result = false;
            using (SqlCommand command_UpdateService = new SqlCommand())
            {
                connection.Open();

                command_UpdateService.Connection = connection;

                command_UpdateService.CommandText = "UPDATE services_by_users SET description = @description , active = @active " +
                 "WHERE  id_service = @id_service";

                command_UpdateService.Parameters.AddWithValue("@id_service", service.Id);
                command_UpdateService.Parameters.AddWithValue("@description", service.Description);

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
            return result;
        }

        //Private Methods
        private Service MapPointFromDB(SqlDataReader reader)
        {

            UsersRepository usersRepository = new UsersRepository();
            ServicesRepository servicesRepository = new ServicesRepository();

            Service service = new Service
            {
                Id = Int32.Parse(reader["id_service"].ToString()),
                Description = reader["description"].ToString(),
                Active = Boolean.Parse(reader["active"].ToString()),
                UserId = Int32.Parse(reader["id_user"].ToString()),
                //Category = categoriesRepository.GetCategory(Int32.Parse(reader["id_category"].ToString()))


            };

            return service;
        }
    }
}