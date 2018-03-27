using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Data.Models
{
    public class DataOperation
    {
        public static DataTable select(String storedProcedure, List<SqlParameter> parameterList)
        {
            System.Diagnostics.Debug.WriteLine("storedProcedure:" + storedProcedure);

            String connectionString = System.Configuration.ConfigurationManager.AppSettings["connectionString"].ToString();
            DataTable dataTable = new DataTable();
            
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand(storedProcedure, sqlConnection);

                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    if (parameterList.Count > 0)
                    {
                        foreach (SqlParameter parameter in parameterList)
                        {
                            sqlCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                        }
                    }

                    sqlConnection.Open();

                    dataTable.Load(sqlCommand.ExecuteReader());
                }
            }
            catch
            {
                System.Diagnostics.Debug.Write("Failed retrieve data for " + storedProcedure);
            }

            return dataTable;
        }
    }
}