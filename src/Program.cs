namespace src
{
    using Azure.Core;
    using Azure.Identity;
    using Azure.Storage.Blobs;
    using Medallion.Threading;
    using Medallion.Threading.Azure;
    using System.Threading.Tasks;

    public class Program
    {
        private const string storageAccountBlobEndpoint = "blob.core.windows.net";

        static async Task Main(string[] args)
        {
            string storageAccountName = (Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_NAME") ?? "tistoragewus2mdrrahman");
            string storageContainerName = (Environment.GetEnvironmentVariable("STORAGE_CONTAINER_NAME") ?? "mirrormaker-checkpoints");
            BlobContainerClient container = new(new Uri($"https://{storageAccountName}.{storageAccountBlobEndpoint}/{storageContainerName}"), new VisualStudioCredential());
            var distributedLock = new AzureBlobLeaseDistributedLock(container, "https://huehue.blob.windows.net/hue");
            Guid id = Guid.NewGuid();
            TimeSpan timeout = TimeSpan.FromSeconds(1);

            while (true)
            {
                await using (AzureBlobLeaseDistributedLockHandle? handle = await distributedLock.TryAcquireAsync(timeout))
                {
                    if (handle != null)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Console.WriteLine($"[{DateTime.Now}][{id}] I have the lock");
                            Thread.Sleep(1000);
                        }
                        handle.Dispose();
                    }
                }
                Console.WriteLine($"[{DateTime.Now}][{id}] I don't have the lock");
                Thread.Sleep(100);
            }
        }
    }
}
