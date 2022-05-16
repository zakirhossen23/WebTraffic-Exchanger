
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace WebTraffic_Exchanger.classes
{
    class GetPost
    {
        public async Task<JToken> Post(String body, String url)
        {
            //"{\n\t\"email\":\"zakiristesting@gmail.com\",\n\t\"password\":\"zakir%%$\"\n\t\n}"
            var json = body;
            var data = new StringContent(json, Encoding.UTF8, "application/json");
                       
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, data);
                string result = response.Content.ReadAsStringAsync().Result;
                var parsed = JObject.Parse(result);
                return parsed;
                int authid = int.Parse(parsed.SelectToken("id").ToString());
                Properties.Settings.Default.Authid = authid;
                Properties.Settings.Default.Save();
            }

        }

        public async Task<JToken> GetbyQuery(String url)
        {
         
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                string result = response.Content.ReadAsStringAsync().Result;
                var parsed = JObject.Parse(result);
                return parsed;
            }

        }


    }
}
