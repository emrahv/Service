using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Data.Models;
using WebManagement.Models;

namespace OrganisationManagement.Models.Behaviours
{
    public class OrganisationSection
    {
        public Section  section = new Section();
        public String location = "";

        public List<OrganisationSection> get(String organisationId, String language)
        {
            List<OrganisationSection> result = new List<OrganisationSection>();

            foreach (DataRow row in GetSectionsForOrganisation(organisationId).Rows)
            {
                OrganisationSection organisationSection = new OrganisationSection();

                organisationSection.section.getLocalisedSection(row["sectionId"].ToString(), language, new Section(), "");
                organisationSection.location = row["location"].ToString();

                result.Add(organisationSection);
            }

            return result;

        }

        private static DataTable GetSectionsForOrganisation(String id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetSectionsForOrganisation", parameterList);
            if (table.Rows.Count < 1) { return new DataTable(); }
            else { return table; }
        }
    }

}