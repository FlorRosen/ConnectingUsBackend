using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using ConnectingUsWebApp.Models;

namespace ConnectingUsWebApp.Services
{
    public class ImageConverter
    {
        public string EncodeTo64(byte[] toEncode)
        {
          //  byte[] toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncode);
            return returnValue;
        }
        public byte[] DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);
            return encodedDataAsBytes;
        }
    }
}
