using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Asos.CodeTest
{
    public class FailoverCustomerDataAccess
    {
        public static async Task<CustomerResponse> GetCustomerById(int id)
        {
            /*ToDo use http factory to get an instace of http object instead of newing it up here.*/

            var client = new HttpClient() { BaseAddress = new Uri("https://failover-api/endpoint/data") };

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("/customers/{0}", id));

            var response = await client.SendAsync(httpRequest);

            var responseContent = await response.Content.ReadAsStringAsync();

            return DataDeserializer.Deserialize<CustomerResponse>(responseContent);
        }
    }
}