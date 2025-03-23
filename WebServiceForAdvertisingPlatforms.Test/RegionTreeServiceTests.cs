using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebServiceForAdvertisingPlatforms.Service;

namespace WebServiceForAdvertisingPlatforms.Test
{
    public class RegionTreeServiceTests
    {
        [Fact]
        public async Task GetAdsByRegion_ReturnsCorrectAds()
        {
            RegionTreeService service = new RegionTreeService();
            await service.LoadDataFromFile("D://regions.txt");

            var result = service.GetAdsByRegion("/ru");

            Assert.NotEmpty(result);
            Assert.Contains("Яндекс.Директ", result);
        }

        [Fact]
        public void WrongFilePath()
        {
            RegionTreeService service = new RegionTreeService();

            Assert.ThrowsAsync<FileNotFoundException>(() => service.LoadDataFromFile("zxc"));
            Assert.ThrowsAsync<FileNotFoundException>(() => service.LoadDataFromFile("./zxc"));
            Assert.ThrowsAsync<ArgumentException>(() => service.LoadDataFromFile("D://regions.json"));
        }
    }
}
