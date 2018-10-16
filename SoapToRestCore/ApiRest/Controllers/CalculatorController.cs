using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApiRest.Model;
using ApiRest.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiRest.Controllers
{
    [Route("api/[controller]")]
    public class CalculatorController : Controller
    {
		[HttpGet("speedup/{core}/{serialtime}/{time}")]
		public async Task<IActionResult> Speedup(string core, string serialTime, string time)
		{
			var result = new Speedup();

			using (var client = new HttpClient())
			{
				var request = CalculatorService.GetRequestSpeedup(core, serialTime, time);
				var content = new StringContent(request, Encoding.UTF8, "text/xml");
				var action = "http://tempuri.org/ICalculator/Speedup";

				client.DefaultRequestHeaders.Add("SOAPAction", action);

				using (var response = await client.PostAsync(CalculatorService.Url, content))
				{
					var asyncstring = await response.Content.ReadAsStringAsync();
					var soapResponse = Transform.Exec(asyncstring);
					var serialize = JsonConvert.DeserializeObject<SpeedupRoot>(soapResponse);

					result.Calculated = serialize.Envelope.Body.SpeedupResponse.SpeedupResult.Calculated;
				}
			}

			return Ok(result);
		}

		[HttpGet("efficiency/{core}/{speedup}")]
		public async Task<IActionResult> Efficiency(string core, string speedUp)
		{
			var result = new Efficiency();

			using (var client = new HttpClient())
			{
				var request = CalculatorService.GetRequestEfficiency(core, speedUp);
				var content = new StringContent(request, Encoding.UTF8, "text/xml");
				var action = "http://tempuri.org/ICalculator/Efficiency";

				client.DefaultRequestHeaders.Add("SOAPAction", action);

				using (var response = await client.PostAsync(CalculatorService.Url, content))
				{
					var asyncstring = await response.Content.ReadAsStringAsync();
					var soapResponse = Transform.Exec(asyncstring);
					var serialize = JsonConvert.DeserializeObject<EfficiencyRoot>(soapResponse);

					result.Calculated = serialize.Envelope.Body.EfficiencyResponse.EfficiencyResult.Calculated;
				}
			}

			return Ok(result);
		}


		// GET api/values
		[HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
