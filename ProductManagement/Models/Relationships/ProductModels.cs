using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Data.Models;

namespace ProductManagement.Models.Relationships
{
    public class ProductModels
    {
        public static List<Model> get(String id, String language)
        {
            List<Model> result = new List<Model>();

            foreach (DataRow row in getModelListFromDatabase(id).Rows)
            {
                Model model = new Model();
                model.get(row["modelId"].ToString(), language);

                if (model.status)
                {
                    result.Add(model);
                }
            }

            return result;

        }

        private static DataTable getModelListFromDatabase(String id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetModelsOfProduct", parameterList);
            if (table.Rows.Count < 1) { return new DataTable(); }
            else { return table; }
        }
        
    }

}