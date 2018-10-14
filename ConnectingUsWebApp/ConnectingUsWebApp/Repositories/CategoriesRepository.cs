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
    public class CategoriesRepository
    {
        private SqlConnection connection;
        private SqlCommand command;

        private void CreateConnection()
        {
            string constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }

        //Public Methods
        public List<Category> GetCategories(int id_category) //This method can bring all categories,or one category
        {
            List<Category> categories = new List<Category>();

            CreateConnection();
            String query = "select * from categories where (@id_category IS NULL) OR id_category = @id_category";
            command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id_category", id_category);
     
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Category category = MapCategoryFromDB(reader);
                    categories.Add(category);
                }
            }
            connection.Close();
            return categories;
        }

        public Category GetCategory(int id)
        {
            Category category = new Category();

            CreateConnection();
            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "select * from service_category where id_category = @id_category"
            };
            command.Parameters.AddWithValue("@id_category", id);
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    category = MapCategoryFromDB(reader);
                }
            }

            connection.Close();
            return category;
        }
        //Private Methods
        private Category MapCategoryFromDB(SqlDataReader reader)
        {

            UsersRepository usersRepository = new UsersRepository();

            Category category = new Category
            {
                Id = Int32.Parse(reader["id_category"].ToString()),
                Description = reader["description"].ToString() 
            };

            return category;
        }
    }
}