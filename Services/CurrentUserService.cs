using System.Security.Claims;

namespace MyPets.Services
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Id() => _httpContextAccessor.HttpContext!.User.FindFirst("id").Value;

        public string Email()
        {
            var a = _httpContextAccessor.HttpContext!.User;
            var b = a.Claims.Where(x=>x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").FirstOrDefault().Value;
            return b;
        } 
        public ClaimsPrincipal? UserClaims => _httpContextAccessor.HttpContext?.User;
    }
}
