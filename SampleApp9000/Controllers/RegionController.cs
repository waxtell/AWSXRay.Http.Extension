using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SampleApp9000.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegionController : ControllerBase
    {
        public RegionController()
        {
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

                //var content = await response.Content.ReadAsStringAsync();
            }

            return new List<Region>();
        }
    }
}
