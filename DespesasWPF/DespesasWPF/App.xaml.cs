using Microsoft.Identity.Client;

namespace DespesasWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        static App()
        {
            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId).WithAuthority($"{Instance}{Tenant}")
                .WithDefaultRedirectUri().Build();
            TokenCacheHelper.EnableSerialization(PublicClientApp.UserTokenCache);
        }

        // Below are the clientId (Application Id) of your app registration and the tenant information. 
        // You have to replace:
        // - the content of ClientID with the Application Id for your app registration
        // - The content of Tenant by the information about the accounts allowed to sign-in in your application:
        //   - For Work or School account in your org, use your tenant ID, or domain
        //   - for any Work or School accounts, use organizations
        //   - for any Work or School accounts, or Microsoft personal account, use organizations
        //   - for Microsoft Personal account, use consumers
        private const string ClientId = "9536832e-4874-42b2-92e2-b84f762da76f";

        // Note: Tenant is important for the quickstart. We'd need to check with Andre/Portal if we
        // want to change to the AadAuthorityAudience.
        private const string Tenant = "organizations";
        private const string Instance = "https://login.microsoftonline.com/";

        public static IPublicClientApplication PublicClientApp { get; }
    }
}