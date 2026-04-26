using System;
using Microsoft.AspNetCore.Mvc;

namespace CollegeWebApplication.Filters
{
    // Use on controllers/actions like [CustomAuthorize("Admin,SuperAdmin")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(string? roles = null) : base(typeof(CustomAuthorizationFilter))
        {
            // Pass the roles string to the filter implementation
            Arguments = new object[] { roles };
        }
    }
}
