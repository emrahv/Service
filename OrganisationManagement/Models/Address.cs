using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Data.Models;

namespace OrganisationManagement.Models
{
    public class Address
    {
        public String id = "";
        public String addressType = "";
        public String addressLine1 = "";
        public String addressLine2 = "";
        public String addressLine3 = "";
        public String addressLine4 = "";
        public String addressLine5 = "";
        public String location = "";
        public Boolean status = false;

        public static List<Address> getAddressList(String id)
        {
            return _getAddressList(id);

        }

        private static List<Address> _getAddressList(String id)
        {
            List<Address> result = new List<Address>();

            List<SqlParameter> parameterList = new List<SqlParameter>();
            
            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetAddressList", parameterList);
            if (table.Rows.Count < 1) { return result; }

            foreach (DataRow row in table.Rows)
            {
                Address address = new Address();

                address.id = id;
                address.addressType = row["addressType"].ToString();
                address.addressLine1 = row["addressLine1"].ToString();
                address.addressLine2 = row["addressLine2"].ToString();
                address.addressLine3 = row["addressLine3"].ToString();
                address.addressLine4 = row["addressLine4"].ToString();
                address.addressLine5 = row["addressLine5"].ToString();
                address.location = row["location"].ToString();
                address.status = Convert.ToBoolean(row["status"]);

                result.Add(address);
            }

            return result;
        }
        
    }
}