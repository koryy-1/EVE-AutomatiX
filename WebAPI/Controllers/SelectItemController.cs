using Application.Services;
using Domen.dto;
using Domen.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("SelectedItem")]
    public class SelectItemController : ControllerBase
    {
        private readonly ILogger<SelectItemController> _logger;
        private SelectItemService _selectItemService;

        public SelectItemController(ILogger<SelectItemController> logger)
        {
            _logger = logger;
            _selectItemService = new SelectItemService();
        }

        [HttpGet("GetSelectItemInfo", Name = "GetSelectItemInfo")]
        public ActionResult<SelectedItemInfo> GetSelectItemInfo()
        {
            var routePanel = _selectItemService.GetInfo();
            return Ok(routePanel);
        }

        [HttpPost("ClickButton", Name = "ClickButton")]
        public IActionResult ClickButton([FromBody] string btnName)
        {
            var success = _selectItemService.ClickButton(btnName);
            if (success)
                return Ok();

            return BadRequest("Button not found");
        }
    }
}
