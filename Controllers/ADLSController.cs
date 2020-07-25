using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Poc_PIM_ADLS.Services;

namespace Poc_PIM_ADLS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADLSController : ControllerBase
    {
        private readonly ILogger<ADLSController> _logger;
        private readonly IDataLakeServiceClient _dataLakeServiceClient;

        public ADLSController(ILogger<ADLSController> logger, IDataLakeServiceClient dataLakeServiceClient)
        {
            _logger = logger;
            _dataLakeServiceClient = dataLakeServiceClient;
        }

        [HttpGet("GetDirectoryContent/{fileSystem}/{directory}")]
        public  async Task<ActionResult<IEnumerable<string>>> GetDirectoryContent(string fileSystem,string directory)
        {
           var result =  await _dataLakeServiceClient.ListFilesInDirectory(fileSystem, directory, true);

            return Ok(result);
        }
        
        [HttpPost("PublishItem/{itemId}")]
        public async Task< IActionResult> PublishItem(string itemId)
        {
            var stringContent = await GetRequestBodyContentAsync();
            var id = itemId + "_"+Guid.NewGuid().ToString();
            await _dataLakeServiceClient.UploadJsonData("outputfs", "output", stringContent, id);
            return Ok();
        }

        [HttpPost("EventGridWebHook")]
        public async Task<IActionResult> EventGridWebHook()
        {
            var stringContent = await GetRequestBodyContentAsync();
            var @event = JsonConvert.DeserializeObject<EventGridTopic[]>(stringContent);
            
            foreach(var item in @event)
            {
                if (item.EventType.Equals(Const.EventTypeBlobCreated))
                {
                    await _dataLakeServiceClient.HandleEventTypeBlobCreated(item);
                }

                if (item.EventType.Equals(Const.EventSubscriptionValidation))
                {
                    await _dataLakeServiceClient.HandleEventSubscriptionValidation(item);
                }
            }

             return Ok();
        }

        private async Task<string> GetRequestBodyContentAsync()
        {
            var streamContent = new StreamContent(this.HttpContext.Request.Body);
            var stringContent = await streamContent.ReadAsStringAsync();
            return stringContent;
        }

    }

  
}
