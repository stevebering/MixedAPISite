using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Runtime.Caching;
using System.Net.Http;

namespace MixedAPISite.Infrastructure
{
    public class ApiOutputCacheAttribute<T>
        : ActionFilterAttribute where T: class
    {
        private TimeSpan _timespan;
        private bool _anonymousOnly;
        private TimeSpan _clientTimespan;

        private string _cacheKey;
        private static readonly ObjectCache WebApiCache = MemoryCache.Default;

        public ApiOutputCacheAttribute(double timespan = 5.0, double clientTimespan = 5.0, bool anonymousOnly = false)
        {
            if (timespan <= 0)
            {
                throw new ArgumentOutOfRangeException("timespan", "Value must be greater than zero.");
            }

            if (clientTimespan <= 0)
            {
                throw new ArgumentOutOfRangeException("clientTimespan", "Value must be greater than zero.");
            }

            _timespan = TimeSpan.FromSeconds(timespan);
            _clientTimespan = TimeSpan.FromSeconds(clientTimespan);

            _anonymousOnly = anonymousOnly;
        }

        private bool IsCacheable(HttpActionContext context)
        {
            if (_anonymousOnly && Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                return false;
            }

            if (context.Request.Method == System.Net.Http.HttpMethod.Get)
            {
                return true;
            }

            return false;
        }

        private CacheControlHeaderValue SetClientCacheValue()
        {
            var cacheControl = new CacheControlHeaderValue();
            cacheControl.MaxAge = _clientTimespan;
            cacheControl.MustRevalidate = true;

            return cacheControl;
        }

        private Action<HttpActionExecutedContext> Callback { set; get; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            if (!IsCacheable(actionContext))
            {
                return;
            }

            _cacheKey = string.Join(":",
                actionContext.Request.RequestUri.AbsolutePath,
                actionContext.Request.Headers.Accept.FirstOrDefault().ToString());

            T cachedValue = WebApiCache.Get(_cacheKey) as T;
            if (cachedValue != null)
            {
                actionContext.Response = actionContext.Request.CreateResponse<T>(System.Net.HttpStatusCode.NotModified, cachedValue);
                actionContext.Response.Content.Headers.ContentType = new MediaTypeHeaderValue(WebApiCache.Get(_cacheKey + "+ContentType").ToString());
                return;
            }
            Callback = (actionExecutedContext) =>
            {
                var output = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
                WebApiCache.Add(_cacheKey, output, DateTimeOffset.UtcNow.AddSeconds(_timespan.TotalSeconds));
                WebApiCache.Add(_cacheKey + "+ContentType", actionExecutedContext.Response.Content.Headers.ContentType.MediaType, DateTimeOffset.UtcNow.AddSeconds(_timespan.TotalSeconds));
            };
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext == null)
            {
                throw new ArgumentNullException("actionExecutedContext");
            }
            Callback(actionExecutedContext);
        }
    }
}