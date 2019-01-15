using System;
using Microsoft.AspNetCore.Mvc;
using StartupApi.Model;

namespace StartupApi.Infrastructure
{
    public class LinkRewriter
    {

        private readonly IUrlHelper urlHelper;

        public LinkRewriter(IUrlHelper urlHelper)
        {
            this.urlHelper = urlHelper;
        }

        public Link Rewrite(Link orginal)
        {
            if (orginal == null)
                return null;

            return new Link
            {
                HRef = urlHelper.Link(orginal.RouteName, orginal.RouteValues),
                Method = orginal.Method,
                Relations = orginal.Relations
            };
        }
    }
}
