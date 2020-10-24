using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using OrchardCore.ContentManagement.Routing;

namespace OrchardCore.Autoroute.Routing
{
    public class AutoRouteTransformer : DynamicRouteValueTransformer
    {
        private readonly IAutorouteEntries _entries;
        private readonly AutorouteOptions _options;

        public AutoRouteTransformer(IAutorouteEntries entries, IOptions<AutorouteOptions> options)
        {
            _entries = entries;
            _options = options.Value;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            (var found, var entry) = await _entries.TryGetEntryByPathAsync(httpContext.Request.Path.Value);

            if (found)
            {
                var routeValues = new RouteValueDictionary(_options.GlobalRouteValues)
                {
                    [_options.ContentItemIdKey] = entry.ContentItemId
                };

                if (!String.IsNullOrEmpty(entry.JsonPath))
                {
                    routeValues[_options.JsonPathKey] = entry.JsonPath;
                }

                // Prevents the original values from being re-added to the dynamic ones.
                values.Clear();

                return routeValues;

            }

            return null;
        }
    }
}
