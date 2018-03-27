using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Data.Models;

namespace ProductManagement.Models.Behaviours
{
    public class ProductSection
    {
        public String section = "";
        public String location = "";

        public List<ProductSection> get(String productId)
        {
            List<ProductSection> result = new List<ProductSection>();

            foreach (DataRow row in getSectionListFromDatabase(productId).Rows)
            {

                ProductSection productSection = new ProductSection();

                productSection.section = row["sectionId"].ToString();
                productSection.location = row["location"].ToString();

                result.Add(productSection);
            }

            return result;
        }

        private static DataTable getSectionListFromDatabase(String id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetProductSections", parameterList);
            if (table.Rows.Count < 1) { return new DataTable(); }
            else { return table; }
        }

    }

}