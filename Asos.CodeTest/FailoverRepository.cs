using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Asos.CodeTest
{
    public class FailoverRepository : IFailoverRepository
    {
        public List<FailoverEntry> GetFailOverEntries()
        {
            var failoverEntries = new List<FailoverEntry>();

            /*ToDo only retrieve the value once by creating a config class and register it at startup using framework IoC and constructor injecting it.*/
            var connectionString = ConfigurationManager.ConnectionStrings["FailoverDatabase.Connection"]
                .ConnectionString;

            /*ToDo use of Dapper or another ORM */
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                var command =
                    new SqlCommand("GetFailoverEntries", sqlConnection) {CommandType = CommandType.StoredProcedure};

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var failoverData = new FailoverEntry {DateTime = reader.GetDateTime(0)};

                        failoverEntries.Add(failoverData);
                    }
                }
            }

            return failoverEntries;
        }
    }
}