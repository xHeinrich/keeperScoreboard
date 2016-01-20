using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
namespace StormBackend
{
    public static class HttpAsync
    {
        public static async Task<string> GetUrlAsync(string url)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("X-AjaxNavigation", "1");
                client.Headers.Add("X-Requested-With", "XMLHttpRequest");
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.124 Safari/537.36");

                return await client.DownloadStringTaskAsync(url);
            }
        }
    }
}
