using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Branding.Models;
using WebManagement.Models;
using Data.Models;

namespace ProductManagement.Models
{
    public class Model
    {
        public String id = "";
        public String name;
        public BrandingPackage brandingPackage = new BrandingPackage();
        public Boolean status = false;

        public Model get(String id, String language)
        {
            DataRow row = getModelDetailsFromDatabase(id);

            if (row.Equals(null)) { return this; }

            this.id = row["id"].ToString();

            if (!Convert.ToBoolean(row["status"])) {return this; }

            this.name = Dictionary.get(language, Int32.Parse(row["name"].ToString())).value;
            this.brandingPackage.get(row["brandingPackageId"].ToString());
            this.status = Convert.ToBoolean(row["status"]);

            return this;
        }

        public Boolean isActive(String id)
        {
            return Convert.ToBoolean(getModelDetailsFromDatabase(id)["status"]);
        }

        private DataRow getModelDetailsFromDatabase(String id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetProductModel", parameterList);
            if (table.Rows.Count < 1) { return null; }
            else { return table.Rows[0]; }
        }
    }
}