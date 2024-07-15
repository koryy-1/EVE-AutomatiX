using Application.Services;
using Domen.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("Inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private InventoryService _inventoryService;

        public InventoryController(ILogger<InventoryController> logger)
        {
            _logger = logger;
            _inventoryService = new InventoryService();
        }

        [HttpGet("GetInventoryInfo", Name = "GetInventoryInfo")]
        public ActionResult<IEnumerable<InventoryItem>> GetInventoryInfo()
        {
            var inventoryItems = _inventoryService.GetInfo();
            return Ok(inventoryItems);
        }

        [HttpGet("IsContainerOpened", Name = "IsContainerOpened")]
        public ActionResult<bool> IsContainerOpened()
        {
            var isOpened = _inventoryService.IsContainerOpened();
            return Ok(isOpened);
        }

        [HttpPost("LootAll", Name = "LootAll")]
        public IActionResult LootAll()
        {
            _inventoryService.LootAll();
            return Ok();
        }
    }
}
