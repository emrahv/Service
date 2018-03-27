using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using Branding.Models;
using OrganisationManagement.Models;
using OrganisationManagement.Models.Behaviours;
using WebManagement.Models;

namespace Service.Controllers
{
    [RoutePrefix("api/Localisation")]
    public class LocalisationController : ApiController
    {
        [HttpGet]
        [Route("GetBrandingPackage")]
        public HttpResponseMessage GetBrandingPackage(String organisationId, String id)
        {
            Organisation organisation = new Organisation();

            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            BrandingPackage brandingPackage = new BrandingPackage();

            string serialisedProduct = new JavaScriptSerializer().Serialize(brandingPackage.get(id));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedProduct, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetBrandDetails")]
        public HttpResponseMessage GetBrandDetails(String organisationId)
        {
            Organisation organisation = new Organisation();

            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            string serialisedBrandingDetails = new JavaScriptSerializer().Serialize(organisation.brandingDetails);

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedBrandingDetails, System.Text.Encoding.UTF8, "application/json")
            };
        }
        
        [HttpGet]
        [Route("Organisation")]
        public HttpResponseMessage Organisation(String organisationId, String language)
        {
            Organisation organisation = new Organisation();

            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            string serialisedData= new JavaScriptSerializer().Serialize(organisation.get(organisationId, language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedData, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetOrganisationSections")]
        public HttpResponseMessage GetOrganisationSections(String organisationId, String language)
        {
            Organisation organisation = new Organisation();

            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            OrganisationSection orgSection = new OrganisationSection();

            string serialisedData = new JavaScriptSerializer().Serialize(orgSection.get(organisationId, language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedData, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        public HttpResponseMessage GetSection(String organisationId, String id, String language)
        {
            Organisation organisation = new Organisation();

            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            Section section = new Section();

            string serialisedSection = new JavaScriptSerializer().Serialize(section.getLocalisedSection(id, language, new Section(), ""));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedSection, System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
