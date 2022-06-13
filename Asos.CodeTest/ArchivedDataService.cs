using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Asos.CodeTest
{
    public class ArchivedDataService : IArchivedDataService
    {
        private readonly string connectionString;

        public ArchivedDataService()
        {
            /*ToDo only retrieve the value once by creating a config class and register it at startup using framework IoC and constructor injecting it.*/
            connectionString = ConfigurationManager.ConnectionStrings["Archive.Database.Connection"].ConnectionString;
        }

        public Customer GetArchivedCustomer(int customerId)
        {
            /*ToDo use of Dapper or another ORM */
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                var command = new SqlCommand("SELECT Id, Name FROM Customer WHERE CustomerId = @customerId", sqlConnection) { CommandType = CommandType.Text };
                command.Parameters.AddWithValue("@customerId", customerId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Customer { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                    }
                }
            }

            return null;
        }
    }
}