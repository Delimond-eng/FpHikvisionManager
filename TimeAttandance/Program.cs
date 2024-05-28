using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttandance
{
    class Program
    {

        static void Main(string[] args)
        {
            Task task = Task.Run(async () => await runApplication());
            task.Wait();
        }

        static async Task runApplication()
        {
            string databaseFile = "agents.db";
            FingerprintManager.FPModule_SetCollectTimes(3);
            FingerprintManager fingerprintManager = new FingerprintManager(databaseFile);

            if (FPutils_x64.FPModule_OpenDevice() != 0)
            {
                Console.WriteLine("Failed to connect to the fingerprint device.");
                return;
            }

            Console.WriteLine("Fingerprint device connected successfully.");

            Console.Write("Enter Agent ID for enrollment: ");
            string agentId = Console.ReadLine();

            if (fingerprintManager.EnrollAgent(agentId))
            {
                Console.WriteLine("Agent enrolled successfully.");
            }
            else
            {
                Console.WriteLine("Failed to enroll agent.");
        
            }

            Console.WriteLine("Starting to monitor fingerprints...");
            FingerprintManager.FPModule_SetCollectTimes(1);
            await fingerprintManager.MonitorFingerprintAsync();

            FPutils_x64.FPModule_CloseDevice();
            Console.ReadLine();

        }
    }
}
