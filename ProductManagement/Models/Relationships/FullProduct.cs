using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductManagement.Models.Relationships
{
    public class FullProduct
    {
        public Product product = new Product();
        public List<Model> models = new List<Model>();
        public List<Product> children = new List<Product>();
        public List<ShortProduct> parents = new List<ShortProduct>();
        public List<Product> relatedProducts = new List<Product>();
        public List<ProductSection> sections = new List<ProductSection>();

        public FullProduct get(String id, String language)
        {
            this.product.get(id, language);
            this.models = ProductModels.get(id, language);
            this.children = Tree.getFullChildren(id, language);
            this.parents = Tree.getParents(id, language);
            this.relatedProducts = Tree.getRelatedProducts(id, language);
            this.sections = ProductSection.get(id, language);

            return this;
        }
    }
}