using ITI.AuthDemo.Server;

namespace ITI.GRPCLab.Server.Services
{
    public class ApiKeyAuthenticationService : IApiKeyAuthenticationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ApiKeyAuthenticationService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public bool Authenticate()
        {
            var context = _httpContextAccessor.HttpContext;

            if (!context.Request.Headers.TryGetValue(Consts.ApiKeyHeaderName, out var apiKey))
                return false;

            var apiKeySettings = _configuration.GetSection(Consts.ApiKeySettingsKey).Value;

            return apiKey == apiKeySettings;
        }
    }
}
