using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Data.Models;

namespace ProductManagement.Models.Behaviours
{
    public class ProductModels
    {
        public List<Model> modelList = new List<Model>();

        public List<Model> getModels(String id, String language)
        {
            List<Model> result = new List<Model>();

            foreach (DataRow row in getModelListFromDatabase(id).Rows)
            {
                Model model = new Model();
                model.get(row["modelId"].ToString(), language);

                if (model.status)
                {
                    this.modelList.Add(model);
                }
            }

            return this.modelList;

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