using Application.Services;
using Domen.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("InfoPanel")]
    public class InfoPanelController : ControllerBase
    {
        private readonly ILogger<InfoPanelController> _logger;
        private InfoPanelService _infoPanelService;

        public InfoPanelController(ILogger<InfoPanelController> logger)
        {
            _logger = logger;
            _infoPanelService = new InfoPanelService();
        }

        [HttpGet("GetRoutePanel", Name = "GetRoutePanel")]
        public ActionResult<RoutePanel> GetRoutePanel()
        {
            var routePanel = _infoPanelService.GetRoutePanel();
            return Ok(routePanel);
        }

        [HttpGet("GetLocation", Name = "GetLocation")]
        public ActionResult<LocationInfo> GetLocation()
        {
            var location = _infoPanelService.GetLocation();
            return Ok(location);
        }
    }
}
