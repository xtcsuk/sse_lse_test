using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Asos.CodeTest
{
    public class CustomerDataAccess : ICustomerDataAccess
    {
        /*ToDo use http factory to get an instace of http object instead of newing it up here.*/
        private readonly HttpClient client;

        public CustomerDataAccess()
        {
            /*ToDo use a config class, startup IoC registeration and constructor injection, to avoid mulltiple config read */
            var baseUrl = ConfigurationManager.AppSettings["CustomerService.ThirdParty.Http.Connection"];

            client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<CustomerResponse> LoadCustomerAsync(int customerId)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, string.Format("/api/customers/{0}", customerId));

            var response = await client.SendAsync(httpRequest);

            var responseContent = await response.Content.ReadAsStringAsync();

            return DataDeserializer.Deserialize<CustomerResponse>(responseContent);
        }
    }
}