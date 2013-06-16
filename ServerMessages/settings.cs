using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using BattleNET;

namespace ServerMessages
{
    class settings
    {
        // This class reads the settings.cfg and puts everything into variables for later use.
        public static string hostRcon = getSetting(1, 5);
        public static string portRcon = getSetting(2, 5);
        public static string passRcon = getSetting(3, 5);
        public static string saveChat = getSetting(4, 9);
        public static string splitChatLogAt = getSetting(5, 6);

        public static BattlEyeLoginCredentials loginCredentials = new BattlEyeLoginCredentials(host: IPAddress.Parse(hostRcon), port: Convert.ToInt16(portRcon), password: passRcon);

        private static String getSetting(int x, int remChar)
        {
            using (var sr = new StreamReader(@"settings.cfg"))
            {
                if (File.ReadLines(@"settings.cfg").Count() < 5)
                {
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Your settings.cfg is incorrectly formatted. Press ENTER to exit.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                for (int i = 1; i < x; i++)
                    sr.ReadLine();
                string returnedLine = sr.ReadLine().Remove(0, remChar);
                sr.Close();
                return returnedLine;
            }
        }
    }
}
