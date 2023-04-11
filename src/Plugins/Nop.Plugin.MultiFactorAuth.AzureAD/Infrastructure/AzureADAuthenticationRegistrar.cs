using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.AzureAD.Infrastructure
{
    /// <summary>
    /// Represents registrar of AzureAD authentication service
    /// </summary>
    public class AzureADAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                //set credentials
                var settings = EngineContext.Current.Resolve<AzureADExternalAuthSettings>();
                
                options.ClientId = string.IsNullOrEmpty(settings?.ClientId) ? nameof(options.ClientId) : settings.ClientId;
                options.ClientSecret = string.IsNullOrEmpty(settings?.ClientSecret) ? nameof(options.ClientSecret) : settings.ClientSecret;
                
                options.Authority = string.IsNullOrEmpty(settings?.Authority) ? nameof(options.Authority) : settings.Authority;
                
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                
                options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = false };
                
                //store access and refresh tokens for the further usage
                options.SaveTokens = true;
                // "login", "none", "consent", "select_account"
                options.Prompt = "select_account";
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme, options => {
                options.LoginPath = AzureADAuthenticationDefaults.LoginPath;
                options.LogoutPath = AzureADAuthenticationDefaults.LogoutPath;
                options.Cookie.HttpOnly = true;
            });
        }
    }
}