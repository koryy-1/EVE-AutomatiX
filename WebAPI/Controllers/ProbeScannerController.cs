using Application.Services;
using Domen.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("ProbeScanner")]
    public class ProbeScannerController : ControllerBase
    {
        private readonly ILogger<ProbeScannerController> _logger;
        private ProbeScannerService _probeScannerService;

        public ProbeScannerController(ILogger<ProbeScannerController> logger)
        {
            _logger = logger;
            _probeScannerService = new ProbeScannerService();
        }

        [HttpGet("GetProbeScanResults", Name = "GetProbeScanResults")]
        public ActionResult<IEnumerable<ProbeScanItem>> GetProbeScanResults()
        {
            var overViewInfo = _probeScannerService.GetInfo();
            return Ok(overViewInfo);
        }
    }
}
