using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SampleApp9000.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegionController : ControllerBase
    {
        private readonly ILogger<RegionController> _logger;

        public RegionController(ILogger<RegionController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Please note this is not a demonstrate of best practices, but rather
        /// a demonstrate of functionality
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Region>> Get()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://dog.ceo");
                var msg = new HttpRequestMessage(HttpMethod.Get, "api/breeds/image/random");
                msg.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                var response = await httpClient.SendAsync(msg);

                var content = await response.Content.ReadAsStringAsync();
                //var response = await httpClient.GetAsync("api/breeds/image/random");
            }

            return new List<Region>();
        }
    }
}
