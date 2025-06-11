using Customer.Common;
using Customer.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ApiResponseData<IEnumerable<Repository.CustomerEntity>>> GetCustomer()
        {
            var data = await _customerService.GetAllAsync();
            return new ApiResponseData<IEnumerable<Repository.CustomerEntity>>
            {
                Data = data
            };
        }

        [HttpGet("{id}")]
        public async Task<ApiResponseData<Repository.CustomerEntity>> GetCustomerById(Guid id)
        {
            return new ApiResponseData<Repository.CustomerEntity>
            {
                Data = await _customerService.GetByIdAsync(id) ?? new Repository.CustomerEntity()
            };
        }

        [HttpPost]
        public async Task<ApiResponseData<Repository.CustomerEntity>>
                AddCustomer([FromBody] Repository.CustomerEntity customer)
        {
            return new ApiResponseData<Repository.CustomerEntity>
            {
                Data = await _customerService.AddAsync(customer)
            };
        }

        [HttpPut("{id}")]
        public async Task<ApiResponseData<bool>> EditCustomer
            ([FromBody] Repository.CustomerEntity customer, [FromRoute] Guid id)
        {
            return new ApiResponseData<bool>
            {
                Data = await _customerService.UpdateAsync(customer)
            };
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponseData<bool>> DeleteCustomer(Guid id)
        {
            return new ApiResponseData<bool>
            {
                Data = await _customerService.DeleteAsync(id)
            };
        }
    }
}