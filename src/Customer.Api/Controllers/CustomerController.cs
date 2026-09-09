using Customer.Api.Authentication;
using Customer.Common;
using Customer.Common.Authorization;
using Customer.Common.Models.Customer;
using Customer.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/customers")]
[RequireTenant]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.CustomersRead)]
    public async Task<ActionResult<ApiResponseData<IEnumerable<CustomerResponse>?>>> GetCustomer()
    {
        return Ok(new ApiResponseData<IEnumerable<CustomerResponse>?>
        {
            Data = await _customerService.GetAllAsync()
        });
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CustomersRead)]
    public async Task<ActionResult<ApiResponseData<CustomerResponse?>>> GetCustomerById(Guid id)
    {
        return Ok(new ApiResponseData<CustomerResponse?>
        {
            Data = await _customerService.GetByIdAsync(id)
        });
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CustomersWrite)]
    public async Task<ActionResult<ApiResponseData<CustomerResponse>>> AddCustomer(
        [FromBody] CreateCustomerRequest customer)
    {
        return Ok(new ApiResponseData<CustomerResponse>
        {
            Data = await _customerService.AddAsync(customer)
        });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CustomersWrite)]
    public async Task<ActionResult<ApiResponseData<bool>>> EditCustomer(
        [FromBody] UpdateCustomerRequest customer,
        [FromRoute] Guid id)
    {
        return Ok(new ApiResponseData<bool>
        {
            Data = await _customerService.UpdateAsync(customer, id)
        });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CustomersWrite)]
    public async Task<ActionResult<ApiResponseData<bool>>> DeleteCustomer(Guid id)
    {
        return Ok(new ApiResponseData<bool>
        {
            Data = await _customerService.DeleteAsync(id)
        });
    }
}
