using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using OrganisationManagement.Models;
using ProductManagement.Models;
using ProductManagement.Models.Relationships;

namespace Service.Controllers
{
    [RoutePrefix("api/Product")]
    public class ProductController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get(String organisationId, String id, String language)
        {
            Organisation organisation = new Organisation();

            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            Product product = new Product ();

            string serialisedProduct = new JavaScriptSerializer().Serialize(product.get(id, language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedProduct, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetFullProduct")]
        public HttpResponseMessage GetFullProduct(String organisationId, String id, String language)
        {
            Organisation organisation = new Organisation();

            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            FullProduct fullProduct = new FullProduct();

            string serialisedProduct = new JavaScriptSerializer().Serialize(fullProduct.get(id, language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedProduct, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetParents")]
        public HttpResponseMessage GetParents(String organisationId, String id, String language)
        {
            Organisation organisation = new Organisation();
            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            Product product = new Product();
            if (!product.isActive(id)) { return new HttpResponseMessage(); }
            
            string serialisedProduct = new JavaScriptSerializer().Serialize(Tree.getParents(id, language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedProduct, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetChildren")]
        public HttpResponseMessage GetChildren(String organisationId, String id, String language)
        {
            Organisation organisation = new Organisation();
            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            Product product = new Product();
            if (!product.isActive(id)) { return new HttpResponseMessage(); }

            string serialisedProduct = new JavaScriptSerializer().Serialize(Tree.getFullChildren(id, language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedProduct, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetProductTree")]
        public HttpResponseMessage GetProductTree(String organisationId, String language)
        {
            Organisation organisation = new Organisation();
            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            Tree productTree = new Tree();

            String serialisedProduct = new JavaScriptSerializer().Serialize(productTree.get("", language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedProduct, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetTopLevelProducts")]
        public HttpResponseMessage GetTopLevelProducts(String organisationId, String language)
        {
            Organisation organisation = new Organisation();
            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            String serialisedProduct = new JavaScriptSerializer().Serialize(Tree.getFullTopLevelProducts(language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedProduct, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetSections")]
        public HttpResponseMessage GetSections(String organisationId, String id, String language)
        {
            Organisation organisation = new Organisation();
            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            Product product = new Product();
            if (!product.isActive(id)) { return new HttpResponseMessage(); }

            string productSectionList = new JavaScriptSerializer().Serialize(ProductSection.get(id, language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(productSectionList, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetRelatedProducts")]
        public HttpResponseMessage GetRelatedProducts(String organisationId, String id, String language)
        {
            Organisation organisation = new Organisation();
            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            Product product = new Product();
            if (!product.isActive(id)) { return new HttpResponseMessage(); }

            string serialisedProduct = new JavaScriptSerializer().Serialize(Tree.getRelatedProducts(id, language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedProduct, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetModels")]
        public HttpResponseMessage GetModels(String organisationId, String id, String language)
        {
            Organisation organisation = new Organisation();
            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            Product product = new Product();
            if (!product.isActive(id)) { return new HttpResponseMessage(); }

            string serialisedProduct = new JavaScriptSerializer().Serialize(ProductModels.get(id, language));

            return new HttpResponseMessage()
            {
                Content = new StringContent(serialisedProduct, System.Text.Encoding.UTF8, "application/json")
            };
        }

        [HttpGet]
        [Route("GetModelSections")]
        public HttpResponseMessage GetModelSections(String organisationId, String id)
        {
            Organisation organisation = new Organisation();
            if (!organisation.isValid(organisationId)) { return new HttpResponseMessage(); }

            Model model = new Model();
            if (!model.isActive(id)) { return new HttpResponseMessage(); }

            ProductModelSection productSection = new ProductModelSection();

            string productSectionList = new JavaScriptSerializer().Serialize(productSection.get(id));

            return new HttpResponseMessage()
            {
                Content = new StringContent(productSectionList, System.Text.Encoding.UTF8, "application/json")
            };
        }


    }
}
