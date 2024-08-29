//using System;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;

//namespace Yourinfo.Models.Admin
//{
//    public class AuthenticationMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public AuthenticationMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            var currentPath = context.Request.Path.Value;
//            var query = context.Request.Query;
//            // Check if the user is authenticated
//            if (context.User.Identity.IsAuthenticated)
//            {
//                var userRoles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

//                // Check user roles and redirect accordingly
//                if (userRoles.Contains("Admin"))
//                {
//                    // Redirect Admin users trying to access non-admin areas
//                    if (context.Request.Path.StartsWithSegments("/Customer", StringComparison.OrdinalIgnoreCase))
//                    {
//                        context.Response.Redirect("/Admin/Index");
//                        return;
//                    }
//                }
//                else if (userRoles.Contains("Customer"))
//                {
//                    // Redirect Customer users trying to access admin areas
//                    if (context.Request.Path.StartsWithSegments("/Admin", StringComparison.OrdinalIgnoreCase))
//                    {
//                        context.Response.Redirect("/Customer/Index");
//                        return;
//                    }
//                }
//            }
//            else
//            {
//                //// Handle unauthenticated users
//                //var isPublicPath = context.Request.Path.StartsWithSegments("/Home", StringComparison.OrdinalIgnoreCase);

//                //var isAdminPath = context.Request.Path.StartsWithSegments("/Admin", StringComparison.OrdinalIgnoreCase);

//                //if (!isAdminPath)
//                //{
//                //    context.Response.Redirect("/Admin/Index");
//                //    return;
//                //}
//                //else
//                //{
//                //    context.Response.Redirect("/Home/Index");
//                //    return;
//                //}

//                // Handle unauthenticated users
//                //var isPublicPath = context.Request.Path.StartsWithSegments("/Home", StringComparison.OrdinalIgnoreCase);
//                //var isAdminPath = context.Request.Path.StartsWithSegments("/Admin", StringComparison.OrdinalIgnoreCase);

//                //// Redirect unauthenticated users accessing non-public pages
//                //if (!isPublicPath && !isAdminPath)
//                //{
//                //    context.Response.Redirect("/Home/Index");
//                //    return;
//                //}

//                //// Redirect unauthenticated users trying to access admin pages
//                //if (isAdminPath)
//                //{
//                //    context.Response.Redirect("/Admin/Index");
//                //    return;
//                //}

//                // Handle unauthenticated users
//                var isPublicPath = currentPath.StartsWith("/Home", StringComparison.OrdinalIgnoreCase);
//                var isAdminPath = currentPath.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase);

//                // Redirect unauthenticated users accessing non-public pages
//                if (!isPublicPath && !isAdminPath)
//                {
//                    // Redirect only if not already on the target path
//                    if (!currentPath.Equals("/Home/Index", StringComparison.OrdinalIgnoreCase))
//                    {
//                        context.Response.Redirect("/Home/Index");
//                        return;
//                    }
//                }

//                // Redirect unauthenticated users trying to access admin pages
//                if (isAdminPath)
//                {
//                    // Redirect only if not already on the target path
//                    if (!currentPath.Equals("/Home/Index", StringComparison.OrdinalIgnoreCase))
//                    {
//                        context.Response.Redirect("/Home/Index");
//                        return;
//                    }
//                }
//            }

//            // Example: Use a query parameter to prevent redirection loop
//            if (context.Request.Query.ContainsKey("redirected"))
//            {
//                await _next(context);
//                return;
//            }
//        }
//    }
//}
//using System;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;

//namespace Yourinfo.Models.Admin
//{
//    public class AuthenticationMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public AuthenticationMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            var currentPath = context.Request.Path.Value;

//            // Check if the user is authenticated
//            if (context.User.Identity.IsAuthenticated)
//            {
//                var userRoles = context.User.Claims
//                    .Where(c => c.Type == ClaimTypes.Role)
//                    .Select(c => c.Value)
//                    .ToList();

//                // Redirect Admin users trying to access non-admin areas
//                if (userRoles.Contains("Admin") && currentPath.StartsWith("/Customer", StringComparison.OrdinalIgnoreCase))
//                {
//                    context.Response.Redirect("/Admin/Index");
//                    return;
//                }
//                // Redirect Customer users trying to access admin areas
//                else if (userRoles.Contains("Customer") && currentPath.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
//                {
//                    context.Response.Redirect("/Customer/Index");
//                    return;
//                }
//            }
//            else
//            {
//                // Handle unauthenticated users
//                if (currentPath.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) || currentPath.StartsWith("/Account", StringComparison.OrdinalIgnoreCase))
//                {
//                    context.Response.Redirect("/Account/Login");
//                    return;
//                }
//                else if (!currentPath.StartsWith("/Home", StringComparison.OrdinalIgnoreCase))
//                {
//                    context.Response.Redirect("/Home/Index");
//                    return;
//                }
//            }

//            // Proceed to the next middleware
//            await _next(context);
//        }
//    }
//}
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value.ToLower();
        if (path.StartsWith("/admin")) // Check if the path starts with /admin
        {
            if (context.User.Identity.IsAuthenticated && context.User.IsInRole("Admin"))
            {
                // User is authenticated and has the Admin role, proceed with the request
                await _next(context);
            }
            else
            {
                // Redirect to the security breach page or index page if the user is not authorized
                context.Response.Redirect("/Account/Securitybreach");
                await _next(context);
                return;
            }
        }
        else if (path.StartsWith("/customer")) // Check if the path starts with /admin
        {
            if (context.User.Identity.IsAuthenticated && context.User.IsInRole("Customer"))
            {
                // User is authenticated and has the Admin role, proceed with the request
                await _next(context);
                return;
            }
            else
            {
                // Redirect to the security breach page or index page if the user is not authorized
                context.Response.Redirect("/Account/Securitybreach");
                return;
            }
        }
        else
        {
            // Redirect to login page if the user is not authenticated
            if (path.StartsWith("/authenticated")) // Adjust the path as needed
            {
                context.Response.Redirect("/Account/Login");
                return;
            }
            await _next(context);
        }
    }
}
