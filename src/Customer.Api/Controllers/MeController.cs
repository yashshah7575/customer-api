using Customer.Common;
using Customer.Common.Identity;
using Customer.Common.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
[Produces("application/json")]
public class MeController : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponseData<MeResponse>> Get(
        [FromServices] ITenantContext tenantContext,
        [FromServices] IApplicationIdentity applicationIdentity)
    {
        return Ok(new ApiResponseData<MeResponse>
        {
            Data = new MeResponse
            {
                Subject = tenantContext.SubjectId,
                Username = tenantContext.Username,
                TenantId = tenantContext.TenantId,
                IsPlatformAdmin = tenantContext.IsPlatformAdmin,
                Roles = applicationIdentity.Roles
            }
        });
    }
}
