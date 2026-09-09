using Customer.Api.Authentication;
using Customer.Common;
using Customer.Common.Authorization;
using Customer.Common.Identity;
using Customer.Common.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.TenantManage)]
[RequireTenant]
[Route("api/tenant")]
[Produces("application/json")]
public class TenantController : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponseData<TenantResponse>> Get([FromServices] ITenantContext tenantContext)
    {
        return Ok(new ApiResponseData<TenantResponse>
        {
            Data = new TenantResponse
            {
                TenantId = tenantContext.TenantId,
                TenantAlias = tenantContext.TenantAlias
            }
        });
    }
}
