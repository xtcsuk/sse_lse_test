using System.Threading.Tasks;

namespace Asos.CodeTest
{
    public interface IFailoverCustomerDataAccessHelper
    {
        Task<CustomerResponse> GetCustomerById(int id);
    }

    public class FailoverCustomerDataAccessHelperAdapter : IFailoverCustomerDataAccessHelper
    {
        public Task<CustomerResponse> GetCustomerById(int id)
        {
            return FailoverCustomerDataAccess.GetCustomerById(id);
        }
    }
}