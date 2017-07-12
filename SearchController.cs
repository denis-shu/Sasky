using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SearchEngine;


namespace GoogleSearch
{
    public class SearchController : ApiController
    {
        
        [HttpPost]
        public string Post()
        {

            string text = string.Empty;
            var value = GetSearchString().Result;
            var request = value.Split();
            if (request.Contains("fullSearch"))
            {
                text = SearchGoogleClient.SearchGoogle(request[1], true);
            }

            text = SearchGoogleClient.SearchGoogle(request[1]);

            return text;
        }

        public async Task<string> GetSearchString()
        {
            var req = await Request.Content.ReadAsStringAsync();
            return req;
        }
    }
}
