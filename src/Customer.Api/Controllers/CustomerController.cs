using Customer.Common;
using Customer.Common.Models.Customer;
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResponseData<IEnumerable<CustomerResponse>?>> GetCustomer()
        {
            return new ApiResponseData<IEnumerable<CustomerResponse>?>
            {
                Data = await _customerService.GetAllAsync()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiResponseData<CustomerResponse?>> GetCustomerById(Guid id)
        {
            return new ApiResponseData<CustomerResponse?>
            {
                Data = await _customerService.GetByIdAsync(id)
            };
        }

        [HttpPost]
        public async Task<ApiResponseData<CustomerResponse>>
                AddCustomer([FromBody] CreateCustomerRequest customer)
        {
            return new ApiResponseData<CustomerResponse>
            {
                Data = await _customerService.AddAsync(customer)
            };
        }

        [HttpPut("{id}")]
        public async Task<ApiResponseData<bool>> EditCustomer
            ([FromBody] UpdateCustomerRequest customer, [FromRoute] Guid id)
        {
            return new ApiResponseData<bool>
            {
                Data = await _customerService.UpdateAsync(customer, id)
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