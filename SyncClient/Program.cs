using Dotmim.Sync;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using System;
using System.Threading.Tasks;

namespace SyncClient
{
    class Program
    {
        static void Main(string[] args)
        {
            PerformSync().GetAwaiter().GetResult();
        }

        static async Task PerformSync()
        {
            var serverUri = new Uri(Properties.Settings.Default.SyncServer);
            var proxyClientProvider = new WebProxyClientProvider(serverUri);

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
