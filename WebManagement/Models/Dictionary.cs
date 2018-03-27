using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;

namespace WebManagement.Models
{
    public class Dictionary
    {
        public int id = 0;
        public String language = System.Configuration.ConfigurationManager.AppSettings["defaultLanguage"].ToString();
        public String value = "";
   
        public static Dictionary get(String language, int id)
        {
            String whiteLabelApiuri = System.Configuration.ConfigurationManager.AppSettings["whiteLabelApiuri"].ToString();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(whiteLabelApiuri);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            String variables = "api/Dictionary/Get?";
            variables += "&id=" + id;
            variables += "&language=" + language;

            HttpResponseMessage response = client.GetAsync(variables).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<Dictionary>().Result;
            }
            else
            {
                return new Dictionary();
            }
        }

        public List<Dictionary> getLibrary(List<int> list, String language)
        {
            String whiteLabelApiuri = System.Configuration.ConfigurationManager.AppSettings["whiteLabelApiuri"].ToString();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(whiteLabelApiuri);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            String serialisedList = new JavaScriptSerializer().Serialize(list);
            
            String variables = "api/Dictionary/Library?";
            variables += "&list=" + serialisedList;
            variables += "&language=" + language;

            HttpResponseMessage response = client.GetAsync(variables).Result;

            if (response.IsSuccessStatusCode)
            {
                List<Dictionary> library = response.Content.ReadAsAsync<List<Dictionary>>().Result;

                return library;
            }
            else
            {
                return new List<Dictionary>();
            }

        }
    }
}