using System.Threading.Tasks;

namespace SearchEngine
{
    public sealed class SearchGoogleClient
    {
        static string output;

        public static string SearchGoogle(string searchText, bool definition = false)
        {
            MainAsync(searchText, definition).Wait();
            return output;
        }
        private static async Task MainAsync(string searchText, bool definition)
        {
            CefSharpWrapper wrapper = new CefSharpWrapper();

            wrapper.InitializeBrowser();

            string[] result = await wrapper.GetResultAfterPageLoad("https://google.com", async () =>
            {
                await wrapper.EvaluateJavascript<string[]>(
    $@"document.getElementById('lst-ib').value = '{searchText}';

document.getElementById('tsf').submit()");

                wrapper.WaitTillAddressChanges();
                if (definition)
                {
                    return await wrapper.GetResultAfterPageLoad(wrapper.Address, async () =>
                        await wrapper.EvaluateJavascript<string[]>(
                            "Array.prototype.map.call(document.querySelectorAll('span._Tgc'), (a) => a.innerText);"));
                }
                else
                {
                    return await wrapper.GetResultAfterPageLoad(wrapper.Address, async () =>
                         await wrapper.EvaluateJavascript<string[]>(
                             "Array.prototype.map.call(document.querySelectorAll('span.st'), (a) => a.innerText);"));

                }
            });
            if (result == null)
            {
                output = "No definition found";
            }
            else
            {
                foreach (string text in result)
                {
                    output += text;
                }
            }

            wrapper.ShutdownBrowser();


        }
    }
}
