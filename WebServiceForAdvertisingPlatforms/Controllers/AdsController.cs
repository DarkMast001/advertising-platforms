using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebServiceForAdvertisingPlatforms.Service;

namespace WebServiceForAdvertisingPlatforms.Controllers
{
    public class FileUploadRequest
    {
        public string FilePath { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly RegionTreeService _regionTreeService;

        public AdsController(RegionTreeService regionTreeService)
        {
            _regionTreeService = regionTreeService;
        }

        // Загрузка данных из файла
        [HttpPost("upload")]
        public IActionResult UploadFile([FromBody] string filePath)
        {
            try
            {
                _regionTreeService.LoadDataFromFile(filePath);
                return Ok($"Данные успешно загружены.");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Поиск рекламных площадок по региону
        [HttpGet("search")]
        public IActionResult SearchAds([FromQuery] string region)
        {
            var ads = _regionTreeService.GetAdsByRegion(region);
            return Ok(ads);
        }
    }
}
