using Application.ClientWindow;
using Domen.dto;
using Microsoft.AspNetCore.Mvc;
using Domen.Models;
using System.Threading;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("GameClient")]
    public class GameClientController : ControllerBase
    {
        private readonly ILogger<GameClientController> _logger;
        private static Client _client;

        static GameClientController()
        {
            var nick = "Yavisha";
            _client = Client.GetInstance(nick);
        }

        public GameClientController(ILogger<GameClientController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetClientParams", Name = "GetClientParams")]
        public ActionResult<ClientParamsDto> GetClientParams()
        {
            //todo: mapping
            return Ok(_client.ClientParams2Dto(_client.ClientParams));
        }

        [HttpGet("GetStatus", Name = "GetStatus")]
        public ActionResult<SearchingStatus> GetStatus()
        {
            _logger.LogDebug($"isActualRootAddress = {_client.Status.IsActualRootAddress}");
            return Ok(_client.Status);
        }

        [HttpGet("CheckRootAddressActuality", Name = "CheckRootAddressActuality")]
        public ActionResult<SearchingStatus> CheckRootAddressActuality()
        {
            var searchingStatus = _client.CheckRootAddressActuality();
            _logger.LogDebug($"isActualRootAddress = {searchingStatus.IsActualRootAddress}");
            return Ok(searchingStatus);
        }

        [HttpPost("UpdateClientParams", Name = "UpdateClientParams")]
        public IActionResult UpdateClientParams([FromBody] ClientParamsDto clientParamsDto)
        {
            var clientParams = _client.Dto2ClientParams(clientParamsDto);

            var success = _client.SetClientParams(clientParams);
            if (success)
            {
                _logger.LogDebug("ClientParams updated successfully");
                return Ok(new { message = "ClientParams updated successfully" });
            }
            else
            {
                _logger.LogDebug("Failed to update ClientParams");
                return BadRequest(new { message = "Failed to update ClientParams" });
            }
        }

        [HttpPost("StartSearch", Name = "StartSearch")]
        public ActionResult<ClientParamsDto> StartSearch()
        {
            if (_client.IsActualRootAddress(_client.ClientParams))
            {
                return Ok(_client.ClientParams2Dto(_client.ClientParams));
            }

            _logger.LogDebug("Search for root address started");

            //async func
            _client.StartSearchingRootAddress();

            _logger.LogDebug($"The root address search is over, new root address is {_client.ClientParams.RootAddress}");

            return Ok(_client.ClientParams2Dto(_client.ClientParams));
        }

        [HttpPost("StopSearch", Name = "StopSearch")]
        public IActionResult StopSearch()
        {
            _client.StopSearching();
            return Ok(new { message = "root address search has been stopped" });
        }

        //todo: обработка ошибки когда клиент игры закрылся и рут адрес нужно заново искать
        //значит IsActualRootAddress = false;
    }
}
