using Application.Services;
using Domen.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("Drones")]
    public class DroneController : ControllerBase
    {
        private readonly ILogger<DroneController> _logger;
        private DroneService _droneService;

        public DroneController(ILogger<DroneController> logger)
        {
            _logger = logger;
            _droneService = new DroneService();
        }

        [HttpGet("GetDronesInfo", Name = "GetDronesInfo")]
        public ActionResult<IEnumerable<Drone>> GetDronesInfo()
        {
            var dronesInfo = _droneService.GetInfo();
            return Ok(dronesInfo);
        }

        [HttpPost("Launch", Name = "Launch")]
        public IActionResult Launch()
        {
            _droneService.Launch();
            return Ok();
        }

        [HttpPost("Engage", Name = "Engage")]
        public IActionResult Engage()
        {
            _droneService.Engage();
            return Ok();
        }

        [HttpPost("Scoop", Name = "Scoop")]
        public IActionResult Scoop()
        {
            _droneService.Scoop();
            return Ok();
        }

        //todo: make method launch / engage / return concrete drone
    }
}
