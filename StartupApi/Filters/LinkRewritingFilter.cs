using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using StartupApi.Infrastructure;
using StartupApi.Model;

namespace StartupApi.Filters
{
    public class LinkRewritingFilter : IAsyncResultFilter
    {
        private readonly IUrlHelperFactory urlHelperFactory;


        public LinkRewritingFilter(IUrlHelperFactory urlHelperFactory)
        {
            this.urlHelperFactory = urlHelperFactory;
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var asObjectResult = context.Result as ObjectResult;
            bool shouldSkip = asObjectResult?.StatusCode >= 400
                || asObjectResult?.Value == null
                || asObjectResult?.Value as Resource == null;

            if (shouldSkip)
                return next();

            var rewriter = new LinkRewriter(urlHelperFactory.GetUrlHelper(context));
            RewriteAllLinks(asObjectResult.Value, rewriter);

            return next();
        }

        private static void RewriteAllLinks(object model, LinkRewriter rewriter)
        {
            if (model == null)
                return;

            var allProperties = model
                    .GetType().GetTypeInfo()
                    .GetAllProperties()
                    .Where(x => x.CanRead)
                    .ToArray();

            var linkProperties = allProperties
                .Where(x => x.CanWrite && x.PropertyType == typeof(Link));

            foreach (var linkProperty in linkProperties)
            {
                var rewritten = rewriter.Rewrite(linkProperty.GetValue(model) as Link);

                if (rewritten == null)
                    continue;

                linkProperty.SetValue(model, rewritten);

                if (linkProperty.Name == nameof(Resource.Self))
                {
                    allProperties.SingleOrDefault(x => x.Name == nameof(Resource.HRef))?
                        .SetValue(model, rewritten.HRef);

                    allProperties.SingleOrDefault(x => x.Name == nameof(Resource.Method))?
                        .SetValue(model, rewritten.Method);

                    allProperties.SingleOrDefault(x => x.Name == nameof(Resource.Relations))?
                        .SetValue(model, rewritten.Relations);
                }

                var arrayProperties = allProperties.Where(x => x.PropertyType.IsArray);
                RewriteLinksInArray(arrayProperties, model, rewriter);

                var objectProperties = allProperties
                    .Except(linkProperties)
                    .Except(arrayProperties);
                RewriteLinksInNestedObjects(objectProperties, model, rewriter);

            }


        }

        private static void RewriteLinksInNestedObjects(IEnumerable<PropertyInfo> objectProperties, object model, LinkRewriter rewriter)
        {
            foreach (var objectProperty in objectProperties)
            {
                if (objectProperty.PropertyType == typeof(string))
                    continue;

                var typeInfo = objectProperty.PropertyType.GetTypeInfo();
                if (typeInfo.IsClass)
                {
                    RewriteAllLinks(objectProperty.GetValue(model), rewriter);
                }
            }

        }

        private static void RewriteLinksInArray(IEnumerable<PropertyInfo> arrayProperties, object model, LinkRewriter rewriter)
        {
            foreach (var arrayProperty in arrayProperties)
            {
                var array = arrayProperty.GetValue(model) as Array ?? new Array[0];
                foreach (var element in array)
                {
                    RewriteAllLinks(element, rewriter);

                }
            }
        }
    }
}
