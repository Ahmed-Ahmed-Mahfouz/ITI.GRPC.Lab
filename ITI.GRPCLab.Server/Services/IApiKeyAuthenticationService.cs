using Microsoft.AspNetCore.Authentication;

namespace ITI.GRPCLab.Server.Services
{
    public interface IApiKeyAuthenticationService
    {
        bool Authenticate();
    }
}
