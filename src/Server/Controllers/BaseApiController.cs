using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Uni.Scan.Shared.Localizers;

namespace Uni.Scan.Server.Controllers
{
    /// <summary>
    /// Abstract BaseApi Controller Class
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController<T> : ControllerBase
    {
        private IStringLocalizer<FrontEndLocalizer> _localizer;

        protected IStringLocalizer<FrontEndLocalizer> Localizer => _localizer ??=
            HttpContext.RequestServices.GetService<IStringLocalizer<FrontEndLocalizer>>();
    }
}