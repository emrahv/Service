using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using Branding.Models;
using WebManagement.Models;
using Data.Models;

namespace OrganisationManagement.Models
{
    public class Organisation
    {
        public String id = "";
        public String name = "";
        public BrandingPackage brandingDetails = new BrandingPackage();
        public String owner = "";
        public String status = "";
        public String organisationDescription = "";
        public String analyticsID = "";
        public List<Address> addressList = new List<Address>();
        public List<Contact> contactList = new List<Contact>();
            
        public Organisation get(String id, String language)
        {
            DataRow row = getOrganisationDetails(id);

            if (row.IsNull("id")) { return this; }

            this.id = row["id"].ToString();
            this.name = row["name"].ToString();
            this.brandingDetails.get(row["brandDetails"].ToString());
            //this.owner = row["owner"].ToString();
            this.status = row["status"].ToString();
            this.organisationDescription = Dictionary.get(language, Int32.Parse(row["organisationDescription"].ToString())).value;
            this.analyticsID = row["analyticsID"].ToString();
            this.addressList = Address.getAddressList(this.id);
            this.contactList = Contact.getContactList(this.id);

            return this;
        }

        public Boolean isValid(String organisationId)
        {
            DataRow row = getOrganisationDetails(organisationId);
            
            if (!row["status"].ToString().Equals("Active") || row.Equals(null))
            { return false; }

            this.brandingDetails.get(row["brandDetails"].ToString());

            return true;

        }

        private DataRow getOrganisationDetails(String organisationId)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = organisationId;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.getOrganisation", parameterList);
            if (table.Rows.Count < 1) { return table.NewRow(); }
            else{ return table.Rows[0]; }
        }
    }
    
}