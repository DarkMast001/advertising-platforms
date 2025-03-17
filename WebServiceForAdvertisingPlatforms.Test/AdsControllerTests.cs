using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServiceForAdvertisingPlatforms.Controllers;
using WebServiceForAdvertisingPlatforms.Service;

namespace WebServiceForAdvertisingPlatforms.Test
{
    public class AdsControllerTests
    {
        [Fact]
        public void SearchAdsWithCorrectData()
        {
            RegionTreeService regionTreeService = new RegionTreeService();
            regionTreeService.LoadDataFromFile("D://regions.txt");

            AdsController controller = new AdsController(regionTreeService);

            var result = controller.SearchAds("/ru/svrd");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var ads = Assert.IsType<List<string>>(okResult.Value);
            Assert.Contains("Яндекс.Директ", ads);
            Assert.Contains("Крутая реклама", ads);
        }

        [Fact]
        public void SearchAdsForNonExistentRegion()
        {
            RegionTreeService regionTreeService = new RegionTreeService();
            regionTreeService.LoadDataFromFile("D://regions.txt");

            AdsController controller = new AdsController(regionTreeService);

            var result = controller.SearchAds("/eu");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var ads = Assert.IsType<List<string>>(okResult.Value);
            Assert.Empty(ads);
        }

        [Fact]
        public void UploadFileWhenFileExists()
        {
            RegionTreeService regionTreeService = new RegionTreeService();
            AdsController controller = new AdsController(regionTreeService);

            var result = controller.UploadFile("D://regions.txt");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Данные успешно загружены.", okResult.Value);
        }

        [Fact]
        public void UploadFileWhenFileDoesNotExist()
        {
            RegionTreeService regionTreeService = new RegionTreeService();
            AdsController controller = new AdsController(regionTreeService);

            var result = controller.UploadFile("D://zxcccc.txt");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("File is not exist.", notFoundResult.Value);
        }
    }
}
