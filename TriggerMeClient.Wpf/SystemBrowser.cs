
using IdentityModel.OidcClient.Browser;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TriggerMeClient.Wpf;
using Awesomium.Windows.Controls;

namespace TriggerMeClient.Wpf
{
    public class SystemBrowser : IdentityModel.OidcClient.Browser.IBrowser,IDisposable
    {
        private readonly int _port;
        private readonly string _path;
        private WebControl _browser;
        private LoopbackHttpListener _listener;
        public SystemBrowser(int port, WebControl browser, string path = null)
        {
            _port = port;
            _path = path;
            _browser = browser;
          
        }

        public void Dispose()
        {
            _listener.Dispose();
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            _listener= new LoopbackHttpListener(_port, _path);

            OpenBrowser(options.StartUrl);

                try
                {
                    var result = await _listener.WaitForCallbackAsync();
                    if (String.IsNullOrWhiteSpace(result))
                    {
                        return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = "Empty response." };
                    }

                    return new BrowserResult { Response = result, ResultType = BrowserResultType.Success };
                }
                catch (TaskCanceledException ex)
                {
                    return new BrowserResult { ResultType = BrowserResultType.Timeout, Error = ex.Message };
                }
                catch (Exception ex)
                {
                    return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = ex.Message };
                }
                finally
                {
                _listener.Dispose();
                }
            
        }
        

        public   void OpenBrowser(string url)
        {
             _browser.Source=new Uri(url);
        }
    }

    public class LoopbackHttpListener : IDisposable
    {
        IAppBuilder appBuilder;
        public LoopbackHttpListener(IAppBuilder app)
        {
            appBuilder = app;
        }
        const int DefaultTimeout = 60 * 5; // 5 mins (in seconds)

        IDisposable _webapp;
        TaskCompletionSource<string> _source = new TaskCompletionSource<string>();
        string _url;

        public string Url => _url;

        public LoopbackHttpListener(int port, string path = null)
        {
            path = path ?? String.Empty;
            if (path.StartsWith("/")) path = path.Substring(1);
            _url = $"http://127.0.0.1:{port}/{path}";
            try
            {
                _webapp = WebApp.Start(_url, Configure);
            }
            catch (Exception ex)
            {

              
            }
           
        }

      

        public void Dispose()
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);
                try
                {
                    _webapp.Dispose();
                }
                catch
                {

                }
            });
        }

        void Configure(IAppBuilder app)
        {
            
            app.Run(async ctx =>
            {
                if (ctx.Request.Method == "GET")
                {
                    SetResult(ctx.Request.QueryString.Value, ctx);
                }
                else if (ctx.Request.Method == "POST")
                {
                    if (!ctx.Request.ContentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Response.StatusCode = 415;
                    }
                    else
                    {
                        using (var sr = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                        {
                            var body = await sr.ReadToEndAsync();
                            SetResult(body, ctx);
                        }
                    }
                }
                else
                {
                    ctx.Response.StatusCode = 405;
                }
            });
        }

        private void SetResult(string value, IOwinContext ctx)
        {
            try
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "text/html";
                ctx.Response.WriteAsync("<h1>You can now return to the application.</h1>");
                ctx.Response.Body.Flush();

                _source.TrySetResult(value);
            }
            catch
            {
                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "text/html";
                ctx.Response.WriteAsync("<h1>Invalid request.</h1>");
                ctx.Response.Body.Flush();
            }
            this.Dispose();
        }

        public Task<string> WaitForCallbackAsync(int timeoutInSeconds = DefaultTimeout)
        {
            Task.Run(async () =>
            {
                await Task.Delay(timeoutInSeconds * 1000);
                _source.TrySetCanceled();
            });

            return _source.Task;
        }
    }
}