using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Data.Models;

namespace OrganisationManagement.Models
{
    public class Contact
    {
        public String id = "";
        public String contactType = "";
        public String contactInfo = "";
        public Boolean status = false;

        public static List<Contact> getContactList(String id)
        {
            return _getContactList(id);
        }

        public static List<Contact> _getContactList(String id)
        {
            List<Contact> result = new List<Contact>();
            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetContactList", parameterList);
            if (table.Rows.Count < 1) { return result ; }
            
            foreach (DataRow row in table.Rows)
            {
                Contact contact = new Contact();

                contact.id = id;
                contact.contactType = row["contactType"].ToString();
                contact.contactInfo = row["contactInfo"].ToString();
                contact.status = Convert.ToBoolean(row["status"]);

                result.Add(contact);
            }

            return result;
        }
    }
 
}