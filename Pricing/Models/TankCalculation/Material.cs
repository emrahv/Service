using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Data.Models;

namespace Pricing.Models.TankCalculation
{
    public class Material
    {
        public String id = "";
        public String name = "";

        public class Plate { public String thickness; public String materialId; }

        public Plate setPlateMaterial(String thickness, String materialId)
        {
            Plate result = new Plate();
            result.thickness = thickness;
            result.materialId = materialId;
            return result;
        }

        public static List<KeyValuePair<String, String>> getList(String productId)
        {
            List<KeyValuePair<String, String>> result = new List<KeyValuePair<string, string>>();

            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = productId;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetProductMaterial", parameterList);
            if (table.Rows.Count < 1) { return result; }

            foreach (DataRow row in table.Rows)
            {
                result.Add(new KeyValuePair<string, string>(row["id"].ToString(), row["materialName"].ToString()));
            }

            return result;
        }
    }
}