using System;
using System.Collections.Generic;
using WebServiceForAdvertisingPlatforms.Service;

namespace WebServiceForAdvertisingPlatforms.Test
{
    public class RegionTreeServiceTests
    {
        [Fact]
        public void GetAdsByRegion_ReturnsCorrectAds()
        {
            RegionTreeService service = new RegionTreeService();
            service.LoadDataFromFile("D://regions.txt");

            var result = service.GetAdsByRegion("/ru");

            Assert.NotEmpty(result);
            Assert.Contains("Яндекс.Директ", result);
        }

        [Fact]
        public void WrongFilePath()
        {
            RegionTreeService service = new RegionTreeService();

            Assert.Throws<FileNotFoundException>(() => service.LoadDataFromFile("zxc"));
            Assert.Throws<FileNotFoundException>(() => service.LoadDataFromFile("./zxc"));
            Assert.Throws<ArgumentException>(() => service.LoadDataFromFile("D://regions.json"));
        }
    }
}
