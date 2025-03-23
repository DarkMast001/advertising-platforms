using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using WebServiceForAdvertisingPlatforms.Service;

namespace WebServiceForAdvertisingPlatforms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly RegionTreeService _regionTreeService;
        private readonly ILogger<AdsController> _logger;

        //public AdsController(RegionTreeService regionTreeService)
        //{
        //    _regionTreeService = regionTreeService;
        //}

        public AdsController(RegionTreeService regionTreeService, ILogger<AdsController> logger)
        {
            _regionTreeService = regionTreeService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromBody] string filePath)
        {
            _logger.LogInformation($"UploadFile called with filePath: {filePath}");

            try
            {
                await _regionTreeService.LoadDataFromFile(filePath);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "File not found: {FilePath}", filePath);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during file upload: {FilePath}", filePath);
                return BadRequest(ex.Message);
            }
            _logger.LogInformation("File uploaded successfully.");
            return Ok($"Данные успешно загружены.");
        }

        [HttpGet("search")]
        public IActionResult SearchAds([FromQuery] string region)
        {
            _logger.LogInformation($"SearchAds called with region: {region}");

            var ads = _regionTreeService.GetAdsByRegion(region);
            if (ads.Count == 0)
            {
                _logger.LogWarning($"No ads found for region: {region}");
            }
            return Ok(ads);
        }
    }
}
