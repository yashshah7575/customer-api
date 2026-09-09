using Customer.Common;
using Customer.Common.Authorization;
using Customer.Common.Models.Identity;
using Customer.Common.Tenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.PlatformAdministration)]
[Route("api/platform")]
[Produces("application/json")]
public class PlatformController : ControllerBase
{
    [HttpGet("tenants")]
    public ActionResult<ApiResponseData<IReadOnlyCollection<PlatformTenantResponse>>> GetTenants()
    {
        IReadOnlyCollection<PlatformTenantResponse> tenants =
        [
            new()
            {
                TenantId = DemoTenants.CustomerA,
                Alias = DemoTenants.CustomerA,
                Name = "Customer A"
            },
            new()
            {
                TenantId = DemoTenants.CustomerB,
                Alias = DemoTenants.CustomerB,
                Name = "Customer B"
            }
        ];

        return Ok(new ApiResponseData<IReadOnlyCollection<PlatformTenantResponse>>
        {
            Data = tenants
        });
    }
}
