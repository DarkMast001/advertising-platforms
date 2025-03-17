using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebServiceForAdvertisingPlatforms.Service;

namespace WebServiceForAdvertisingPlatforms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly RegionTreeService _regionTreeService;

        public AdsController(RegionTreeService regionTreeService)
        {
            _regionTreeService = regionTreeService;
        }

        [HttpPost("upload")]
        public IActionResult UploadFile([FromBody] string filePath)
        {
            try
            {
                _regionTreeService.LoadDataFromFile(filePath);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok($"Данные успешно загружены.");
        }

        [HttpGet("search")]
        public IActionResult SearchAds([FromQuery] string region)
        {
            var ads = _regionTreeService.GetAdsByRegion(region);
            return Ok(ads);
        }
    }
}
