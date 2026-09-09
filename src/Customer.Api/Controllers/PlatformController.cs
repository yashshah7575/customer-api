using Customer.Common;
using Customer.Common.Authorization;
using Customer.Common.Models.Identity;
using Customer.Common.Tenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.PlatformManage)]
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
                TenantId = DemoTenants.AcmeBankId,
                Alias = DemoTenants.AcmeBankAlias,
                Name = DemoTenants.AcmeBankName
            },
            new()
            {
                TenantId = DemoTenants.ContosoFinanceId,
                Alias = DemoTenants.ContosoFinanceAlias,
                Name = DemoTenants.ContosoFinanceName
            }
        ];

        return Ok(new ApiResponseData<IReadOnlyCollection<PlatformTenantResponse>>
        {
            Data = tenants
        });
    }
}
