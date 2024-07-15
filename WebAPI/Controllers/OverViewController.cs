using Application.ClientWindow;
using Application.Services;
using Domen.dto;
using Domen.Models;
using Microsoft.AspNetCore.Mvc;
using NLog.Targets;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("OverView")]
    public class OverViewController : ControllerBase
    {
        private readonly ILogger<OverViewController> _logger;
        private OverViewService _overViewService;

        public OverViewController(ILogger<OverViewController> logger)
        {
            _logger = logger;
            _overViewService = new OverViewService();
        }

        [HttpGet("GetOverViewInfo", Name = "GetOverViewInfo")]
        public ActionResult<IEnumerable<OverviewItem>> GetOverViewInfo()
        {
            var overViewInfo = _overViewService.GetInfo();
            return Ok(overViewInfo);
        }

        [HttpPost("ClickOnObject", Name = "ClickOnObject")]
        public IActionResult ClickOnObject([FromBody] OverviewItem spaceObject)
        {
            _overViewService.ClickOnObject(spaceObject);

            return Ok(new { message = "space object clicked" });
        }

        [HttpPost("LockTargets", Name = "LockTargets")]
        public IActionResult LockTargets([FromBody] IEnumerable<OverviewItem> targets)
        {
            _overViewService.LockTargets(targets);

            return Ok(new { message = "targets locked" });
        }

        [HttpPost("LockTargetByName", Name = "LockTargetByName")]
        public IActionResult LockTargetByName([FromBody] string targetName)
        {
            _overViewService.LockTargetByName(targetName);

            return Ok(new { message = "target locked" });
        }
    }
}
