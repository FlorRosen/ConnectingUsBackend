using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using ConnectingUsWebApp.Models;
using System.Web.Http;
using ConnectingUsWebApp.Services;

namespace ConnectingUsWebApp.Repositories
{
    public class ImagesRepository
    {
        private SqlConnection connection;
        private SqlCommand command;

        public ImagesRepository()
        {
            var constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }
        //Public Methods
        //Brings all images for a service
        public Image GetImage(int idService)
        {
            Image image = new Image();


            String query = "select * " +
                           "from services_images where id_service = @id_service";
            command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id_service", idService);

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    image = MapImageFromDB(reader);
                    
                }
            }
            connection.Close();
            return image;
        }

        public bool AddImage(String imageString, int idService)
        {
            var result = false;
            
            try
            {

                using (SqlCommand command_addImage = new SqlCommand())
                {

                    connection.Open();

                    command_addImage.Connection = connection;

                    command_addImage.CommandText = "INSERT INTO services_images (id_service, image) " +
                     "VALUES (@id_service,@image)";

                    command_addImage.Parameters.AddWithValue("@id_service", idService);
                    command_addImage.Parameters.AddWithValue("@image", imageString ?? Convert.DBNull);

                    command_addImage.ExecuteNonQuery();
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


        public bool UpdateImage(String imageString, int idService)
        {
            var result = false;

            try
            {

                using (SqlCommand command_addImage = new SqlCommand())
                {

                    connection.Open();

                    command_addImage.Connection = connection;

                    command_addImage.CommandText = "UPDATE services_images SET image = @image " +
                        "WHERE id_service = @id_service";

                    command_addImage.Parameters.AddWithValue("@id_service", idService);
                    command_addImage.Parameters.AddWithValue("@image", imageString ?? Convert.DBNull);

                    command_addImage.ExecuteNonQuery();
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
        private Image MapImageFromDB(SqlDataReader reader)
        {

            Image image = new Image
            {
                Id = Int32.Parse(reader["id_image"].ToString()),
                IdService = Int32.Parse(reader["id_service"].ToString()),
                ImageString = reader["image"].ToString(),
                
            };

            return image;
        }
    }
}