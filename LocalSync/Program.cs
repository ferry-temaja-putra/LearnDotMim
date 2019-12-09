using Dotmim.Sync;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.SqlServer;
using System;
using System.Threading.Tasks;

namespace LocalSync
{
    class Program
    {
        static void Main(string[] args)
        {
            PerformSync().GetAwaiter().GetResult();
        }

        static async Task PerformSync()
        {            
            var serverProvider = new SqlSyncProvider(
                @"Data Source=.\sqlexpress;
                Initial Catalog=AdventureWorks;
                Integrated Security=true;");
            
            var clientProvider = new SqliteSyncProvider("advworks.db");
            
            var tablesToSync = new string[] {
                "ProductCategory",
                "ProductDescription", "ProductModel",
                "Product", "ProductModelProductDescription",
                "Address", "Customer", "CustomerAddress",
                "SalesOrderHeader", "SalesOrderDetail" };
            
            var agent = new SyncAgent(clientProvider, serverProvider, tablesToSync);

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
