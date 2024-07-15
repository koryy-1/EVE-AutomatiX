using Application.Services;
using Domen.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("HudInterface")]
    public class HudInterfaceController : ControllerBase
    {
        private readonly ILogger<HudInterfaceController> _logger;
        private HudInterfaceService _hudInterfaceService;

        public HudInterfaceController(ILogger<HudInterfaceController> logger)
        {
            _logger = logger;
            _hudInterfaceService = new HudInterfaceService();
        }

        [HttpGet("GetHudInfo", Name = "GetHudInfo")]
        public ActionResult<HudInterface> GetHudInfo()
        {
            var overViewInfo = _hudInterfaceService.GetInfo();
            return Ok(overViewInfo);
        }

        [HttpGet("GetShipHP", Name = "GetShipHP")]
        public ActionResult<HealthPoints> GetShipHP()
        {
            var overViewInfo = _hudInterfaceService.GetShipHP();
            return Ok(overViewInfo);
        }

        [HttpGet("GetCurrentSpeed", Name = "GetCurrentSpeed")]
        public ActionResult<int> GetCurrentSpeed()
        {
            var overViewInfo = _hudInterfaceService.GetCurrentSpeed();
            return Ok(overViewInfo);
        }

        [HttpGet("GetAllModules", Name = "GetAllModules")]
        public ActionResult<IEnumerable<Module>> GetAllModules()
        {
            var overViewInfo = _hudInterfaceService.GetAllModules();
            return Ok(overViewInfo);
        }

        [HttpGet("GetShipFlightMode", Name = "GetShipFlightMode")]
        public ActionResult<ShipFlightMode> GetShipFlightMode()
        {
            var overViewInfo = _hudInterfaceService.GetShipFlightMode();
            return Ok(overViewInfo);
        }

        [HttpGet("GetCenterPos", Name = "GetCenterPos")]
        public ActionResult<Point> GetCenterPos()
        {
            var overViewInfo = _hudInterfaceService.GetCenterPos();
            return Ok(overViewInfo);
        }
    }
}
