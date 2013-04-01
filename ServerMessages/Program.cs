using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using BattleNET;

namespace ServerMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Reading messages...");
            string messagesFile = File.ReadAllText(@"messages.txt");
            string[] messages = messagesFile.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Starting RCon, checking connection...");
            checkConnection();

            int timeLoop = 0;
            while(true)
            {
                if (timeLoop > 599)
                {
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Reading messages...");
                    messagesFile = File.ReadAllText(@"messages.txt");
                    messages = messagesFile.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    timeLoop = 0;
                }

                foreach (string x in messages)
                {
                    string[] messageThis = x.Split('~');
                    if (DateTime.Now.ToString("HH:mm:ss") == messageThis[0]) sayGlobal(messageThis[1]);
                }

                timeLoop++;
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void checkConnection()
        {
            BattlEyeLoginCredentials loginCredentials = new BattlEyeLoginCredentials();
            loginCredentials.Host = IPAddress.Parse(Settings.hostRcon);
            loginCredentials.Port = Convert.ToInt16(Settings.portRcon);
            loginCredentials.Password = Settings.passRcon;
            BattlEyeClient b = new BattlEyeClient(loginCredentials);
            b.BattlEyeConnected += BattlEyeConnected;
            b.BattlEyeDisconnected += BattlEyeDisconnected;
            b.ReconnectOnPacketLoss = false;

            b.Connect();
            b.Disconnect();
        }

        private static void BattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            if (args.ConnectionResult == BattlEyeConnectionResult.Success)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Succesful connection to BE server.");
            }
            if (args.ConnectionResult == BattlEyeConnectionResult.InvalidLogin)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Connection error: Invalid login! Press ENTER to exit");
                Console.ReadLine();
                Environment.Exit(0);
            }
            if (args.ConnectionResult == BattlEyeConnectionResult.ConnectionFailed)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Connection error: server unreachable! Checking connection in 5 seconds...");
                System.Threading.Thread.Sleep(5000);
                checkConnection();
            }
        }

        private static void BattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            if (args.DisconnectionType == BattlEyeDisconnectionType.ConnectionLost)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Connection lost: Packet exception! Checking connection in 5 seconds...");
                System.Threading.Thread.Sleep(5000);
                checkConnection();
            }
            if (args.DisconnectionType == BattlEyeDisconnectionType.SocketException)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Connection lost: Socket exception! Checking connection in 5seconds...");
                System.Threading.Thread.Sleep(5000);
                checkConnection();
            }
            if (args.DisconnectionType == BattlEyeDisconnectionType.Manual)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Connection lost: Disconnected");
            }
        }

        private static void sayGlobal(String saystr)
        {
            BattlEyeLoginCredentials loginCredentials = new BattlEyeLoginCredentials();
            loginCredentials.Host = IPAddress.Parse(Settings.hostRcon);
            loginCredentials.Port = Convert.ToInt16(Settings.portRcon);
            loginCredentials.Password = Settings.passRcon;
            BattlEyeClient b = new BattlEyeClient(loginCredentials);
            b.BattlEyeConnected += BattlEyeConnected;
            b.BattlEyeDisconnected += BattlEyeDisconnected;
            b.ReconnectOnPacketLoss = true;

            b.Connect();
            b.SendCommand("Say -1 " + saystr);
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Said global: " + saystr);
            b.Disconnect();
        }
    }
    public class Settings
    {
        public static string hostRcon = getSetting(1);
        public static string portRcon = getSetting(2);
        public static string passRcon = getSetting(3);

        private static String getSetting(int x)
        {
            using (var sr = new StreamReader(@"settings.txt"))
            {
                for (int i = 1; i < x; i++)
                    sr.ReadLine();
                return sr.ReadLine().Remove(0, 5);
            }
        }
    }
}