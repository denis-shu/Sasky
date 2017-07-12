
using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SearchEngine
{
    public sealed class CefSharpWrapper
    {
        private static ChromiumWebBrowser _browser;

        public string Address => _browser.Address;

        public void WaitTillAddressChanges()
        {
            AutoResetEvent waitHandle = new AutoResetEvent(false);

            EventHandler<AddressChangedEventArgs> onAddressChanged = null;

            onAddressChanged = (sender, e) =>
            {
                _browser.AddressChanged -= onAddressChanged;

                waitHandle.Set();
            };

            _browser.AddressChanged += onAddressChanged;

            waitHandle.WaitOne();
        }

        public void InitializeBrowser()
        {
            CefSettings settings = new CefSettings();

            settings.CefCommandLineArgs.Add("disable-gpu", "1");

            if (!Cef.IsInitialized)
                Cef.Initialize(settings, shutdownOnProcessExit: true, performDependencyCheck: true);
            
            _browser = new ChromiumWebBrowser();//GetBrowser();

            AutoResetEvent waitHandle = new AutoResetEvent(false);

            EventHandler onBrowserInitialized = null;

            onBrowserInitialized = (sender, e) =>
            {
                _browser.BrowserInitialized -= onBrowserInitialized;

                waitHandle.Set();
            };

            _browser.BrowserInitialized += onBrowserInitialized;

            waitHandle.WaitOne();

        }

        //public static ChromiumWebBrowser GetBrowser()
        //{
        //    if (_browser == null)
        //        return new ChromiumWebBrowser();
        //    return _browser;
        //}


        public void ShutdownBrowser()
        {
            Cef.Shutdown();

        }

        public Task<T> GetResultAfterPageLoad<T>(string pageUrl, Func<Task<T>> onLoadCallback)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

            EventHandler<LoadingStateChangedEventArgs> onPageLoaded = null;

            T t = default(T);

            onPageLoaded = async (sender, e) =>
            {
                if (!e.IsLoading)
                {
                    _browser.LoadingStateChanged -= onPageLoaded;

                    t = await onLoadCallback();

                    tcs.SetResult(t);
                }
            };

            _browser.LoadingStateChanged += onPageLoaded;

            _browser.Load(pageUrl);

            return tcs.Task;
        }

        public async Task<T> EvaluateJavascript<T>(string script)
        {
            JavascriptResponse javascriptResponse = await _browser.EvaluateScriptAsync(script);

            if (javascriptResponse.Success)
            {
                object scriptResult = javascriptResponse.Result;
                return ConvertHelper.ToTypedVariable<T>(scriptResult);
            }

            throw new ScriptException(javascriptResponse.Message);
        }
    }
}
