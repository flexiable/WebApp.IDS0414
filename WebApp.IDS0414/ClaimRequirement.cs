using Microsoft.AspNetCore.Authorization;

namespace WebApp.IDS0414
{
    public class ClaimRequirement : IAuthorizationRequirement
    {
        public ClaimRequirement(string rolename,string rolevalue)
        {
            ClaimName = rolename;
            ClaimValue = rolevalue;
        }
        
        public string ClaimName { get; set; }
        public string ClaimValue { get; set; }
    }
}