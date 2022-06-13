using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Asos.CodeTest.UnitTests
{
    [TestFixture]
    public sealed class CustomerServiceShould
    {
        private CustomerService _customerService;
        private Mock<IFailoverRepository> _failoverRepository;
        private Mock<IAppSettings> _appSettings;
        private Mock<ICustomerDataAccess> _customerDataAccess;
        private Mock<IArchivedDataService> _archivedDataService;
        private Mock<IFailoverCustomerDataAccessHelper> _failoverCustomerDataAccessHelper;

        const int customerId = 12345;
        private readonly Customer _expectedCustomer = new Customer { Id = customerId, Name = "Test Customer" };


        [SetUp]
        public void Init()
        {
            _failoverRepository = new Mock<IFailoverRepository>();
            _customerDataAccess = new Mock<ICustomerDataAccess>();
            _archivedDataService = new Mock<IArchivedDataService>();
            _failoverCustomerDataAccessHelper = new Mock<IFailoverCustomerDataAccessHelper>();
            _appSettings = new Mock<IAppSettings>();

            _appSettings.Setup(mock => mock.IsFailoverModeEnabled).Returns(true);

            _customerService = new CustomerService(_appSettings.Object, _failoverRepository.Object,
                _customerDataAccess.Object, _archivedDataService.Object, _failoverCustomerDataAccessHelper.Object);
        }

        [Test]
        public async Task ReturnsCustomerFromMainCustomerDataStore_WHEN_NotInFailOver_AND_NotArchived()
        {
            // Arrange
            var emptyFailoverEntries = new List<FailoverEntry>();

            _customerDataAccess.Setup(mock => mock.LoadCustomerAsync(customerId))
                    .ReturnsAsync(new CustomerResponse { Customer = _expectedCustomer, IsArchived = false });
            _failoverRepository.Setup(mock => mock.GetFailOverEntries()).Returns(emptyFailoverEntries);

            // Act
            var result = await _customerService.GetCustomer(customerId, false);

            // Assert
            Assert.AreEqual(_expectedCustomer, result);
        }

        [Test]
        public async Task ReturnsCustomerFromArchivedCustomerDataStore_WHEN_Archived()
        {
            // Arrange
            _archivedDataService.Setup(mock => mock.GetArchivedCustomer(customerId)).Returns(_expectedCustomer);

            // Act
            var result = await _customerService.GetCustomer(customerId, true);

            // Assert
            Assert.AreEqual(_expectedCustomer, result);
        }

        [Test]
        public async Task ReturnsCustomerFromFailOverDataStore_WHEN_InFailOver_AND_NotArchived()
        {
            // Arrange
            var failOverEntries = GetFailOverMockData();

            _failoverRepository.Setup(mock => mock.GetFailOverEntries()).Returns(failOverEntries);
            _failoverCustomerDataAccessHelper.Setup(mock => mock.GetCustomerById(customerId))
                .ReturnsAsync(new CustomerResponse { Customer = _expectedCustomer, IsArchived = false });

            // Act
            var result = await _customerService.GetCustomer(customerId, false);

            // Assert
            Assert.AreEqual(_expectedCustomer, result);
        }


        [Test]
        public async Task ReturnsCustomerFromFailOverDataStore_WHEN_InFailOver_AND_Archived()
        {
            // Arrange
            var failOverEntries = GetFailOverMockData();

            _failoverRepository.Setup(mock => mock.GetFailOverEntries()).Returns(failOverEntries);
            _archivedDataService.Setup(mock => mock.GetArchivedCustomer(customerId)).Returns(_expectedCustomer);
            _failoverCustomerDataAccessHelper.Setup(mock => mock.GetCustomerById(customerId))
                .ReturnsAsync(new CustomerResponse { Customer = _expectedCustomer, IsArchived = true });

            // Act
            var result = await _customerService.GetCustomer(customerId, false);

            // Assert
            Assert.AreEqual(_expectedCustomer, result);
        }

        private static List<FailoverEntry> GetFailOverMockData()
        {
            var failOverEntries = new List<FailoverEntry>();
            for (var i = 0; i < 101; i++)
            {
                failOverEntries.Add(new FailoverEntry
                {
                    DateTime = DateTime.Now
                });
            }

            return failOverEntries;
        }


    }
}