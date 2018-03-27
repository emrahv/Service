using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebManagement.Models;
using Data.Models;

namespace ProductManagement.Models
{
    public class Tree
    {
        public ShortProduct parent = new ShortProduct();
        public List<Tree> children = new List<Tree>();

        public class Relation
        {
            public String productId = "";
            public int productName = 0;
            public String parent = "";
            public int parentName = 0;
        }

        public List<Tree> get(String parentId, String language)
        {
            List<Tree> result = new List<Tree>();

            List<Relation> treeData = getProductTreeLibrary();
            List<Dictionary> library = getLibrary(language);

            if (parentId.Equals(""))
            {
                List<Relation> list = new List<Relation>();

                foreach (ShortProduct topLevelProduct in _getTopLevelProducts(language,treeData,library))
                {
                    Tree newItem = new Tree();
                    newItem.parent = topLevelProduct;

                    result.Add(newItem);
                }
            }
            else 
            {
                Tree newItem = new Tree();
                newItem.parent = ShortProduct.get(parentId, language);

                result.Add(newItem);
            }

            foreach (Tree parentItem in result)
            {
                List<ShortProduct> childrenList = _getChildren(parentItem.parent.id, language, treeData, library);

                foreach (ShortProduct child in childrenList)
                {
                    Tree childItem = new Tree();
                    childItem.parent = child;

                    parentItem.children.Add(childItem);
                }

            }
            
            return result;
        }

        private static List<Dictionary> getLibrary(String language)
        {
            List<int> libraryList = new List<int>();
            List<Relation> listItems = new List<Relation>();
            listItems = getProductTreeLibrary();

            foreach (Relation productTreeData in listItems)
            {
                libraryList.Add(productTreeData.productName);
                libraryList.Add(productTreeData.parentName);
            }

            Dictionary dictionary = new Dictionary();
            return dictionary.getLibrary(libraryList, language);
        }

        private ShortProduct getParent(String id, String language)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);
            
            DataTable table = DataOperation.select("serviceManager.GetParent", parameterList);
            if (table.Rows.Count < 1) { return new ShortProduct(); }

            return ShortProduct.get(table.Rows[0]["parentId"].ToString(), language);
        }

        public static List<ShortProduct> getParents(String id, String language)
        {
            return _getParents(id, language, new List<Relation>(), new List<Dictionary>());
        }

        private static List<ShortProduct> _getParents(String id, String language, List<Relation> treeLibrary, List<Dictionary> listDictionary)
        {
            List<ShortProduct> result = new List<ShortProduct>();
            
            List<Relation> productTreeLibrary = new List<Relation>();

            if (treeLibrary.Count > 0) { productTreeLibrary = treeLibrary; }
            else { productTreeLibrary = getProductTreeLibrary(); }

            String currentID = id;
            
            List<Dictionary> library = new List<Dictionary>();

            if (listDictionary.Count > 0)
            {
                library = listDictionary;
            }
            else { library = getLibrary(language); }
            

            for (int i=0; i < productTreeLibrary.Count; i++)
            {
                if (productTreeLibrary[i].productId.Equals(currentID))
                {
                    ShortProduct product = new ShortProduct();
                    product.id = productTreeLibrary[i].parent;

                    foreach (Dictionary dict in library)
                    {
                        if (dict.language.Equals(language) && productTreeLibrary[i].parentName.Equals(dict.id))
                        {
                            product.name = dict.value;
                        }
                    }

                    result.Add(product);  

                    currentID = productTreeLibrary[i].parent;

                    if (currentID.Equals(null)) { break; }
                    
                    i = 0;
                }
            }

            return result;
        }

        public List<ShortProduct> getChildren(String id, String language)
        {
            return _getChildren(id, language, new List<Relation>(), new List<Dictionary>());
        }

        public List<Product> getFullChildren(String id, String language)
        {
            List<Product> result = new List<Product>();

            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetChildren", parameterList);
            if (table.Rows.Count < 1) { return new List<Product>(); }

            foreach (DataRow row in table.Rows)
            {
                Product product = new Product();
                product.get(row["productId"].ToString(), language);
                result.Insert(0, product);
            }

            return result;
        }

        private List<ShortProduct> _getChildren(String id, String language, List<Relation> treeLibrary, List<Dictionary> listDictionary)
        {
            List<ShortProduct> result = new List<ShortProduct>();

            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetChildren", parameterList);
            if (table.Rows.Count < 1) { return new List<ShortProduct>(); }

            List<Dictionary> library = new List<Dictionary>();
            if (listDictionary.Count < 0){library = getLibrary(language);}
            else { library = listDictionary; }

            foreach (DataRow row in table.Rows)
            {
                ShortProduct product = new ShortProduct();
                product.id = row["productId"].ToString();

                foreach (Dictionary dict in library)
                {
                    if (dict.language.Equals(language) && Int32.Parse(row["name"].ToString())==dict.id)
                    {
                        product.name = dict.value;
                    }
                }

                result.Add(product);
            }

            return result;
        }

        public List<Product> getRelatedProducts(String id, String language)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetRelatedProducts", parameterList);

            if (table.Rows.Count < 1) { return new List<Product>(); }

            List<Product> result = new List<Product>();

            foreach (DataRow row in table.Rows)
            {
                Product product = new Product();
                product.get(row["relatedProductId"].ToString(), language);

                if (product.status)
                {
                    result.Add(product);
                }
            }

            return result;
        }

        public static List<ShortProduct> getTopLevelProducts(String language)
        {
            return _getTopLevelProducts(language, new List<Relation>(), new List<Dictionary>());
        }

        public static List<Product> getFullTopLevelProducts(String language)
        {
            List<Product> result = new List<Product>();

            DataTable table = DataOperation.select("serviceManager.GetTopLevelProducts", new List<SqlParameter>());

            if (table.Rows.Count < 1) { return new List<Product>(); }

            foreach (DataRow row in table.Rows)
            {
                Product product = new Product();
                product.get(row["productId"].ToString(), language);
                result.Add(product);
            }
            return result;
        }

        private static List<ShortProduct> _getTopLevelProducts(String language, List<Relation> treeLibrary, List<Dictionary> listDictionary)
        {
            List<ShortProduct> result = new List<ShortProduct>();

            DataTable table = DataOperation.select("serviceManager.GetTopLevelProducts", new List<SqlParameter>());
            
            if (table.Rows.Count < 1) { return new List<ShortProduct>(); }

            List<Dictionary> library = new List<Dictionary>();
            if (listDictionary.Count > 0) { library = listDictionary; }
            else { library = getLibrary(language); }

            foreach (DataRow row in table.Rows)
            {
                ShortProduct item = new ShortProduct();

                item.id = row["productId"].ToString();

                foreach(Dictionary dict in library)
                {
                    if (dict.language.Equals(language) && dict.id==Int32.Parse(row["name"].ToString()))
                    {
                        item.name = dict.value;
                    }
                }
                result.Add(item);
            }
            return result;

        }

        public static List<Relation> getProductTreeLibrary()
        {
            List<Relation> result = new List<Relation>();

            DataTable table = DataOperation.select("serviceManager.GetProductTreeLibrary", new List<SqlParameter>());

            if (table.Rows.Count < 1) { return new List<Relation>(); }

            foreach (DataRow row in table.Rows)
            {
                Relation newItem = new Relation();
                newItem.productId = row["productId"].ToString();
                newItem.productName = Int32.Parse(row["productName"].ToString());
                newItem.parent = row["parent"].ToString();
                newItem.parentName = Int32.Parse(row["parentName"].ToString());
                result.Add(newItem);
            }

            return result;
        }
    }
}