using Newtonsoft.Json;
using System.Text;

namespace VposApi.Helpers
{
    public static class HttpClientHelper
    {
        public static async Task<string> PostJsonAsync(string url, object data)
        {
            using var client = new HttpClient();
            var json = JsonConvert.SerializeObject(data);
            var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }
    }

}
