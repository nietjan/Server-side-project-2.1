using ApplicationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Text;


namespace Infrastructure {
    public class UserSessionIdentity : IUserSession {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;    

        public UserSessionIdentity(IHttpContextAccessor httpContextAccessor, 
            UserManager<IdentityUser> userManager) {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserIdentityId() {
            //get logged in user
            var result = _httpContextAccessor?.HttpContext?.User;
            if(result == null) {
                return string.Empty;
            }

            //get user identity
            var user = _userManager.GetUserAsync(result).Result;
            if (user == null) {
                return string.Empty;
            }

            return user.Id;
        }
    }
}
