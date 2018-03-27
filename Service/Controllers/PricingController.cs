using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using OrganisationManagement.Models;
using Pricing.Models;

namespace Service.Controllers
{
    [RoutePrefix("api/Pricing")]
    public class PricingController : ApiController
    {
        [HttpGet]
        [Route("BespokeTank")]
        public HttpResponseMessage BespokeTank(String organisationId, 
            Double width, Double length, Double height, 
            String topModuleMaterialThickness, String topModuleMaterial,
            String bottomModuleMaterialThickness, String bottomModuleMaterial)
        {
            Organisation organisation = new Organisation();

            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            Pricing.Models.TankCalculation.PrismaticTank tank = new Pricing.Models.TankCalculation.PrismaticTank();
            
            string serialisedPrice = new JavaScriptSerializer().Serialize(
                tank.calculatePrice(
                                    width, length, height,
                                    topModuleMaterialThickness, topModuleMaterial,
                                    bottomModuleMaterialThickness, bottomModuleMaterial));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedPrice, System.Text.Encoding.UTF8, "application/json")
            };
        }

    }
}
