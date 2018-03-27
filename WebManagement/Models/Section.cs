using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Branding.Models;
using Data.Models;
using System.Web.Script.Serialization;

namespace WebManagement.Models
{
    public class Section
    {
        public String id = "";
        public String templateName = "";
        public String dataModelType = "";
        public String dataModel = "";
        public BrandingPackage brandingPackage = new BrandingPackage();
        public List<SubSection> subSectionList = new List<SubSection>();

        public class SubSection
        {
            public Section section = new Section();
            public String width = "";
        }

        public class Variable
        {
            public String type;
            public String id;
            public String name;
            public List<KeyValuePair<String, String>> itemList = new List<KeyValuePair<String, String>>();

            public void getList(String language, String source, String productId)
            {
                switch (source)
                {
                    case "Material":
                        itemList = Pricing.Models.TankCalculation.Material.getList(productId);
                        break;

                    case "MaterialThickness":
                        //itemList = Pricing.Models.TankCalculation.Material.Thickness.getList();
                        break;

                    case "PrismaticTankPackage":
                        //itemList = Pricing.Models.TankCalculation.PrismaticTank.Package.getList();
                        break;
                }
                
            }
        }

        public Section get(String id)
        {
            this.id = id;

            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetSection", parameterList);
            if (table.Rows.Count < 1) { return this; }
           
            this.templateName = table.Rows[0]["templateName"].ToString();
            this.dataModelType = table.Rows[0]["dataModelType"].ToString();
            this.dataModel = getSectionDataModel(id);
            this.brandingPackage = getSectionBrandingPackage(id);
            this.subSectionList = getSubSectionList(id);

            return this;
        }

        public Section getLocalisedSection(String id, String language, Section existingSectionData, String productId)
        {
            Section section = existingSectionData;

            if (existingSectionData.id.Length <= 0)
            { section = get(id); }

            if (section.dataModelType.Equals("Dynamic"))
            {
                List<SqlParameter> parameterList = new List<SqlParameter>();

                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@id";
                parameter.Value = id;
                parameterList.Add(parameter);

                DataTable table = DataOperation.select("serviceManager.GetSectionDataSourceVariables", parameterList);
                if (table.Rows.Count < 1) { return this; }

                List<Variable> variables = new List<Variable>();
               
                foreach (DataRow data in table.Rows)
                {
                    Variable variable = new Variable();

                    variable.id = data["Variable"].ToString();
                    variable.name = Dictionary.get(language, Int32.Parse(data["Name"].ToString())).value;
                    variable.type = data["Type"].ToString();
                    variable.getList(language, data["Source"].ToString(), productId);

                    variables.Add(variable);
                }

                var jsonSerialiser = new JavaScriptSerializer();

                section.dataModel = jsonSerialiser.Serialize(variables);

                String a = "";

            }

            List<SubSection> subSections = new List<SubSection>();

            foreach (SubSection subSection in section.subSectionList)
            {
                SubSection sub = subSection;
                sub.section = subSection.section.getLocalisedSection("", language, sub.section, "");
                subSections.Add(sub);
            }

            section.subSectionList = subSections;

            int start = 0;
                
            while(section.dataModel.IndexOf("text\":", start) > 0)
            {
                start = section.dataModel.IndexOf("text\":", start);
                int end = section.dataModel.IndexOf("}", start + 6);

                int textID = int.Parse(section.dataModel.Substring(start + 6, end - start - 6));
                
                section.dataModel =
                    section.dataModel.Substring(0, start + 6) +
                    "\"" + Dictionary.get(language, textID).value + "\"" +
                    section.dataModel.Substring(end);

                start = end + 1;

                if (start >= section.dataModel.Length)
                {
                    break;
                }
            }

            start = 0;

            while (section.dataModel.IndexOf("name\":", start) > 0)
            {
                start = section.dataModel.IndexOf("name\":", start);
                int end = section.dataModel.IndexOf(",", start + 6);

                int textID = int.Parse(section.dataModel.Substring(start + 6, end - start - 6));

                section.dataModel =
                    section.dataModel.Substring(0, start + 6) +
                    "\"" + Dictionary.get(language, textID).value + "\"" +
                    section.dataModel.Substring(end);

                start = end + 1;

                if (start >= section.dataModel.Length)
                {
                    break;
                }
            }

            start = 0;

            while (section.dataModel.IndexOf("unit\":", start) > 0)
            {
                start = section.dataModel.IndexOf("unit\":", start);
                int end = section.dataModel.IndexOf("}", start + 6);

                if (section.dataModel.Substring(start + 5, end - start - 5).IndexOf("[") > 0)
                {
                    end = section.dataModel.IndexOf("]", start + 6);
                    String[] list = new String[] { };
                    list = section.dataModel.Substring(start + 7, end - start - 7).Split(',');
                    String result = "";

                    foreach (String item in list)
                    {
                        result += "\"" + Dictionary.get(language, Int32.Parse(item)).value + "\",";
                    }
                    result = result.Substring(0, result.Length - 1);//remove comma

                    section.dataModel =
                                    section.dataModel.Substring(0, start + 7) +
                                    result +
                                    section.dataModel.Substring(end);

                    start = end + 1;
                }
                else
                {
                    int textID = int.Parse(section.dataModel.Substring(start + 6, end - start - 6));

                    section.dataModel =
                        section.dataModel.Substring(0, start + 6) +
                        "\"" + Dictionary.get(language, textID).value + "\"" +
                        section.dataModel.Substring(end);

                    start = end + 1;
                }
                
                if (start >= section.dataModel.Length)
                {
                    break;
                }
            }

            start = 0;
            while (section.dataModel.IndexOf("title\":", start) > 0)
            {
                start = section.dataModel.IndexOf("title\":", start);
                int end = section.dataModel.IndexOf(",", start + 7);

                int textID;

                bool result = Int32.TryParse(section.dataModel.Substring(start + 7, end - start - 7), out textID);
                if (result)
                {
                    section.dataModel =
                        section.dataModel.Substring(0, start + 7) +
                        "\"" + Dictionary.get(language, textID).value + "\"" +
                        section.dataModel.Substring(end);
                }
                else
                {
                    section.dataModel =
                        section.dataModel.Substring(0, start + 7) +
                        "\"\"" +
                        section.dataModel.Substring(end);
                }

                start = end + 1;

                if (start >= section.dataModel.Length)
                {
                    break;
                }
            }

            return section;
        }

        private BrandingPackage getSectionBrandingPackage(String id)
        {
            BrandingPackage result = new BrandingPackage();

            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetSectionBrandingPackage", parameterList);
            if (table.Rows.Count < 1) { return result; }

            return result.get(table.Rows[0]["brandingPackageId"].ToString());
        }

        private String getSectionDataModel(String id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetSectionDataModel", parameterList);
            if (table.Rows.Count < 1) { return ""; }
            else { return table.Rows[0]["dataModel"].ToString(); }
        }

        private List<SubSection> getSubSectionList(String sectionId)
        {
            List<SubSection> result = new List<SubSection>();

            List<SqlParameter> parameterList = new List<SqlParameter>();

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            parameterList.Add(parameter);

            DataTable table = DataOperation.select("serviceManager.GetSubSectionList", parameterList);
            if (table.Rows.Count < 1) { return result; }

            foreach (DataRow row in table.Rows)
            {
                SubSection subSection = new SubSection();
                subSection.section.get(row["subSectionId"].ToString());
                subSection.width = row["width"].ToString();
                result.Add(subSection);
            }

            return result;

        }

    }
}