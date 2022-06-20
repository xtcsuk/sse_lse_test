using System;
using System.Threading.Tasks;

namespace Asos.CodeTest
{
    public class CustomerService
    {
        private readonly IAppSettings _settings;
        private readonly IFailoverRepository _failoverRepository;
        private readonly ICustomerDataAccess _customerDataAccess;
        private readonly IArchivedDataService _archivedDataService;
        private readonly IFailoverCustomerDataAccessHelper _failOverCustomerDataAccessHelper;

        public CustomerService(IAppSettings settings, IFailoverRepository failoverRepository,
            ICustomerDataAccess customerDataAccess, IArchivedDataService archivedDataService,
            IFailoverCustomerDataAccessHelper failOverCustomerDataAccessHelper)
        {
            _settings = settings;
            _failoverRepository = failoverRepository;
            _customerDataAccess = customerDataAccess;
            _archivedDataService = archivedDataService;
            _failOverCustomerDataAccessHelper = failOverCustomerDataAccessHelper;
        }

        public async Task<Customer> GetCustomer(int customerId, bool isCustomerArchived)
        {
            if (isCustomerArchived)
            {
                Customer archivedCustomer = GetCutomerFromArchive(customerId);
                return archivedCustomer;
            }

            var customerResponse = await GetCustomerFromRelevantStore(customerId);

            Customer customer;
            if (customerResponse.IsArchived)
            {
                customer = GetCutomerFromArchive(customerId);
            }
            else
            {
                customer = customerResponse.Customer;
            }

            return customer;
        }

        private async Task<CustomerResponse> GetCustomerFromRelevantStore(int customerId)
        {
            int failedRequests = 0;
            if (_settings.IsFailoverModeEnabled)
            {
                failedRequests = GetNumberOfFailedRequest();
            }

            CustomerResponse customerResponse;
            if (failedRequests > 100)
            {
                customerResponse = await _failOverCustomerDataAccessHelper.GetCustomerById(customerId);
            }
            else
            {
                customerResponse = await _customerDataAccess.LoadCustomerAsync(customerId);
            }

            return customerResponse;
        }

        private int GetNumberOfFailedRequest()
        {
            var failoverEntries = _failoverRepository.GetFailOverEntries();

            var failedRequests = 0;

            foreach (var failoverEntry in failoverEntries)
            {
                if (failoverEntry.DateTime > DateTime.Now.AddMinutes(-10))
                {
                    failedRequests++;
                }
            }

            return failedRequests;
        }

        private Customer GetCutomerFromArchive(int customerId)
        {
            var archivedCustomer = _archivedDataService.GetArchivedCustomer(customerId);
            return archivedCustomer;
        }

    }
}
