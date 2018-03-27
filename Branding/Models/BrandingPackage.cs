using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Branding.Models
{
    public class BrandingPackage
    {
        public String id = "";
        public List<String> colours = new List<String>();
        public List<String> images = new List<String>();
        public String rootURL = "";

        public class BrandDetails
        {
            public List<String> logoList = new List<String>();
            public List<String> iconList = new List<String>();
            public List<String> colourList = new List<String>();
            public String rootURL = "";
        }

        private String whiteLabelApiuri = System.Configuration.ConfigurationManager.AppSettings["whiteLabelApiuri"].ToString();

        public BrandingPackage get(String id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(whiteLabelApiuri);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            String variables = "api/Branding/Get?";
            variables += "id=" + id;

            HttpResponseMessage response = client.GetAsync(variables).Result;

            if (response.IsSuccessStatusCode)
            {
                BrandingPackage brandingPackage = response.Content.ReadAsAsync<BrandingPackage>().Result;

                this.id = brandingPackage.id;
                this.images = brandingPackage.images;
                this.colours = brandingPackage.colours;
                this.rootURL = brandingPackage.rootURL;

                return this;
            }
            else
            {
                return new BrandingPackage();
            }

        }

        public BrandDetails getBrandDetails(String id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(whiteLabelApiuri);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            String variables = "api/Branding/GetBrandDetails?";
            variables += "id=" + id;

            HttpResponseMessage response = client.GetAsync(variables).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<BrandDetails>().Result;
            }
            else
            {
                return new BrandDetails();
            }

        }
    }
}