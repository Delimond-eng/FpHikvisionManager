using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeAttandance
{
    class FingerprintManager
    {

        private DatabaseHelper _db;

        public FingerprintManager(string databaseFile)
        {
            _db = new DatabaseHelper(databaseFile);
        }

        public bool EnrollAgent(string agentId)
        {
            byte[] template1 = new byte[512];
            byte[] template2 = new byte[512];
            byte[] template3 = new byte[512];

            int result = FPutils_x64.FPModule_FpEnroll(template1);
            if (result != 0) return false;

            result = FPutils_x64.FPModule_FpEnroll(template2);
            if (result != 0) return false;

            result = FPutils_x64.FPModule_FpEnroll(template3);
            if (result != 0) return false;

            _db.AddAgent(agentId, template1, template2, template3);
            return true;
        }

        public string IdentifyAgent()
        {
            byte[] capturedTemplate = new byte[512];
            int result = FPutils_x64.FPModule_FpEnroll(capturedTemplate);
            if (result != 0) return null;

            int securityLevel = 3; // You can adjust the security level as needed

            using (var reader = _db.GetAllAgents())
            {
                while (reader.Read())
                {
                    byte[] dbTemplate1 = (byte[])reader["FpTemplate1"];
                    byte[] dbTemplate2 = (byte[])reader["FpTemplate2"];
                    byte[] dbTemplate3 = (byte[])reader["FpTemplate3"];

                    if (FPutils_x64.FPModule_MatchTemplate(capturedTemplate, dbTemplate1, securityLevel) == 0 ||
                        FPutils_x64.FPModule_MatchTemplate(capturedTemplate, dbTemplate2, securityLevel) == 0 ||
                        FPutils_x64.FPModule_MatchTemplate(capturedTemplate, dbTemplate3, securityLevel) == 0)
                    {
                        return reader["AgentId"].ToString();
                    }
                }
            }
            return null;
        }

        public async Task MonitorFingerprintAsync()
        {
            while (true)
            {
                int fpStatus = 0;
                int result = FPutils_x64.FPModule_DetectFinger(ref fpStatus);

                if (result == 0 && fpStatus == 1)
                {
                    string identifiedAgentId = IdentifyAgent();
                    if (identifiedAgentId != null)
                    {
                        Console.WriteLine($"Fingerprint matched with Agent ID: {identifiedAgentId}");
                    }
                    else
                    {
                        Console.WriteLine("Fingerprint did not match any agent.");
                    }

                    await Task.Delay(2000);
                }
                else
                {
                    await Task.Delay(500);
                }
            }
        }

        public static int FPModule_SetCollectTimes(int dwTimes)
        {
            return FPutils_x64.FPModule_SetCollectTimes(dwTimes);
        }
    }
}
