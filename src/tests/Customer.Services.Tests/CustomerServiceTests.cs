using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Customer.Common.Models.Customer;
using Customer.Repository;
using Customer.Repository.Interface;
using Customer.Service;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Customer.Service.Tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private Mock<ICustomerRepository> _repo;
        private Mock<IMapper> _mapper;
        private Mock<ILogger<CustomerService>> _logger;
        private CustomerService _customerService;

        [SetUp]
        public void SetUp()
        {
            _repo = new Mock<ICustomerRepository>();
            _mapper = new Mock<IMapper>();
            _logger = new Mock<ILogger<CustomerService>>();
            _customerService = new CustomerService(_repo.Object, _mapper.Object, _logger.Object);
        }

        [Test]
        public async Task AddAsync_ValidRequest_ReturnsMappedResponse()
        {
            // Arrange
            var req = new CreateCustomerRequest { Email = "john@contoso.com" };
            var ent = new CustomerEntity { Email = req.Email, Id = Guid.NewGuid() };
            var resp = new CustomerResponse { Email = req.Email, Id = ent.Id };

            _mapper.Setup(m => m.Map<CustomerEntity>(req)).Returns(ent);
            _repo.Setup(r => r.AddAsync(ent)).ReturnsAsync(ent);
            _mapper.Setup(m => m.Map<CustomerResponse>(ent)).Returns(resp);

            // Act
            var result = await _customerService.AddAsync(req);

            // Assert
            result.Should().BeEquivalentTo(resp);
            _repo.Verify(r => r.AddAsync(ent), Times.Once);
        }

        [Test]
        public void AddAsync_WhenRepositoryThrows_Rethrows()
        {
            var req = new CreateCustomerRequest { Email = "boom@contoso.com" };
            _mapper.Setup(m => m.Map<CustomerEntity>(req)).Returns(new CustomerEntity());
            _repo.Setup(r => r.AddAsync(It.IsAny<CustomerEntity>())).ThrowsAsync(new InvalidOperationException());

            Func<Task> act = async () => await _customerService.AddAsync(req);
            act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test]
        public async Task DeleteAsync_ForwardsReturnValue()
        {
            var id = Guid.NewGuid();
            _repo.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);

            var result = await _customerService.DeleteAsync(id);

            result.Should().BeTrue();
            _repo.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task GetAllAsync_ReturnsMappedCollection()
        {
            var entities = new List<CustomerEntity> { new() { Id = Guid.NewGuid() } };
            var dtos = new List<CustomerResponse> { new() { Id = entities[0].Id } };

            _repo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
            _mapper.Setup(m => m.Map<IEnumerable<CustomerResponse>>(entities)).Returns(dtos);

            var result = await _customerService.GetAllAsync();

            result.Should().BeEquivalentTo(dtos);
        }

        [Test]
        public async Task GetByIdAsync_WhenFound_ReturnsMappedCustomer()
        {
            var id = Guid.NewGuid();
            var ent = new CustomerEntity { Id = id };
            var dto = new CustomerResponse { Id = id };

            _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(ent);
            _mapper.Setup(m => m.Map<CustomerResponse>(ent)).Returns(dto);

            var result = await _customerService.GetByIdAsync(id);

            result.Should().BeEquivalentTo(dto);
        }

        [Test]
        public void GetByIdAsync_WhenNotFound_ThrowsKeyNotFoundException()
        {
            var id = Guid.NewGuid();
            _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CustomerEntity?)null);

            Func<Task> act = async () => await _customerService.GetByIdAsync(id);
            act.Should().ThrowAsync<KeyNotFoundException>()
               .WithMessage($"Customer with id: {id} does not exist*");
        }

        [Test]
        public void UpdateAsync_WhenCustomerNotFound_ThrowsKeyNotFoundException()
        {
            var id = Guid.NewGuid();
            _repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((CustomerEntity?)null);

            Func<Task> act = async () => await _customerService.UpdateAsync(new UpdateCustomerRequest(), id);
            act.Should().ThrowAsync<KeyNotFoundException>()
               .WithMessage($"Customer with id: {id} does not exist to update");
        }
    }
}