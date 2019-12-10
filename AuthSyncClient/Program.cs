using AuthSyncSharedData;
using Dotmim.Sync;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AuthSyncClient
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {            
            MainAsync().GetAwaiter().GetResult();
        }

        static void ConfigureHttpClient()
        {
            client.BaseAddress = new Uri(Properties.Settings.Default.SyncServer);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        static async Task MainAsync()
        {
            ConfigureHttpClient();

            var user = await Authenticate();
            if (user == null)
            {
                Console.WriteLine("Unauthorised");
                return;
            }

            await SyncData(user.Token);
        }

        static async Task<User> Authenticate()
        {
            var loginInfo = new LoginInfo()
            {
                UserName = "admin",
                Password = "admin"
            };

            var response = await client.PostAsJsonAsync(
                "api/auth", loginInfo);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsAsync<User>();
        }

        static async Task SyncData(string token)
        {
            var serverUri = new Uri(Properties.Settings.Default.SyncServer + "api/sync");
            var proxyClientProvider = new WebProxyClientProvider(serverUri);
            proxyClientProvider.AddCustomHeader("Authorization", token);

            var clientProvider = new SqliteSyncProvider("advworks.db");

            var agent = new SyncAgent(clientProvider, proxyClientProvider);

            var progress = new Progress<ProgressArgs>(s =>
                Console.WriteLine($"{s.Context.SyncStage}:\t{s.Message}"));

            do
            {
                var context = await agent.SynchronizeAsync(progress);
                Console.WriteLine($"Total Changes downloaded: \t{context.TotalChangesDownloaded}");
                Console.WriteLine($"Total Changes Uploaded: \t{context.TotalChangesUploaded}");
                Console.WriteLine($"Total Changes Conflicts: \t{context.TotalSyncConflicts}");

            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }
    }
}
