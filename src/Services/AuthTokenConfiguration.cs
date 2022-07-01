using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationAPI.Services
{
    public static class AuthTokenConfiguration
    {
        public static string Url = "https://fs.foxconn.cz/adfs/oauth2/token/";
        public static string ClientID { get; set; }
        public static string ClientSecret { get; set; }

        /// <summary>
        /// Get authorization token from ADFS because this is not API with user calling token
        /// </summary>
        /// <returns></returns>
        public static string GetAuthorizationToken()
        {
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(ClientID + ":" + ClientSecret);
            string encoded = Convert.ToBase64String(toEncodeAsBytes);
            request.Headers.Add("Authorization", "Basic " + encoded);

            using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string grant_type = "grant_type=client_credentials";
                streamWriter.Write(grant_type);
            }

            try
            {
                // make the web request and return the content
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader responseReader = new StreamReader(response.GetResponseStream());
                string sResponseHTML = responseReader.ReadToEnd();
                string[] splitResponse = sResponseHTML.Split(new char[] { '"', ',' });

                if (splitResponse != null && splitResponse.Length > 3)
                {
                    return splitResponse[3];
                }

                return string.Empty;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}