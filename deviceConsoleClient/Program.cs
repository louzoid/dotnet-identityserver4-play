using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace deviceConsoleClient
{
    public class Program
    {
        static IDiscoveryCache _cache = new DiscoveryCache("http://localhost:5000");

        public static async Task Main()
        {
            Console.Title = "Console Device Flow";

            var authorizeResponse = await RequestAuthorizationAsync();

            var tokenResponse = await RequestTokenAsync(authorizeResponse);
            //tokenResponse.Show();
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            Console.ReadLine();
            await CallServiceAsync(tokenResponse.AccessToken);
        }

        static async Task<DeviceAuthorizationResponse> RequestAuthorizationAsync()
        {
            var disco = await _cache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            var client = new HttpClient();
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = "deviceClient",
                ClientSecret = "secret"
            });

            if (response.IsError) throw new Exception(response.Error);

            Console.WriteLine($"user code   : {response.UserCode}");
            Console.WriteLine($"device code : {response.DeviceCode}");
            Console.WriteLine($"URL         : {response.VerificationUri}");
            Console.WriteLine($"Complete URL: {response.VerificationUriComplete}");

            Console.WriteLine($"\nPress enter to launch browser ({response.VerificationUri})");
            Console.ReadLine();

            Process.Start(new ProcessStartInfo(response.VerificationUri) { UseShellExecute = true });
            return response;
        }

        private static async Task<TokenResponse> RequestTokenAsync(DeviceAuthorizationResponse authorizeResponse)
        {
            var disco = await _cache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            var client = new HttpClient();

            while (true)
            {
                var response = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "deviceClient",
                    ClientSecret = "secret",
                    DeviceCode = authorizeResponse.DeviceCode
                });

                if (response.IsError)
                {
                    if (response.Error == OidcConstants.TokenErrors.AuthorizationPending || response.Error == OidcConstants.TokenErrors.SlowDown)
                    {
                        Console.WriteLine($"{response.Error}...waiting.");
                        Thread.Sleep(authorizeResponse.Interval * 1000);
                    }
                    else
                    {
                        throw new Exception(response.Error);
                    }
                }
                else
                {
                    return response;
                }
            }
        }

        static async Task CallServiceAsync(string token)
        {
            var baseAddress = "http://localhost:5002";

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.SetBearerToken(token);
            var response = await client.GetStringAsync("identity");

            //"\n\nService claims:".ConsoleGreen();
            Console.WriteLine("\n\nService claims:");
            Console.WriteLine(JArray.Parse(response));
        }
    }
}
