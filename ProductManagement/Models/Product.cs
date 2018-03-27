using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebManagement.Models;
using Data.Models;
using Branding.Models;

namespace ProductManagement.Models
{
    public class ShortProduct
    {
        public String id = "";
        public String name = "";

        public static ShortProduct get(String id, String language)
        {
            ShortProduct result = new ShortProduct();

            List<SqlParameter> parameterList = new List<SqlParameter>();
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);
            DataRow row = DataOperation.select("serviceManager.GetProduct", parameterList).Rows[0];
            
            if (!Convert.ToBoolean(row["status"])) { return result; }

            result.id = row["id"].ToString();
            result.name = Dictionary.get(language, Int32.Parse(row["name"].ToString())).value;

            return result;
        }

    }

    public class Product
    {
        public String id = "";
        public String name;
        public String productId = "";
        public Boolean status = false;
        public BrandingPackage brandingPackage = new BrandingPackage();

        public Product get(String id, String language)
        {
            DataRow row = getProductDetailsFromDatabase(id);

            if (row.Equals(null)) { return new Product(); }

            this.id = row["id"].ToString();

            if (!Convert.ToBoolean(row["status"])) { return this; }

            this.name = Dictionary.get(language, Int32.Parse(row["name"].ToString())).value;
            this.productId = row["productId"].ToString();
            this.status = Convert.ToBoolean(row["status"].ToString());
            
            this.brandingPackage = this.brandingPackage.get(row["brandingPackageId"].ToString());

            return this;
        }

        public Boolean isActive(String id)
        {
            return Convert.ToBoolean(getProductDetailsFromDatabase(id)["status"]);

        }

        private static DataRow getProductDetailsFromDatabase(String id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetProduct", parameterList);
            if (table.Rows.Count < 1) { return null; }
            else { return table.Rows[0]; }
        }
    }

}