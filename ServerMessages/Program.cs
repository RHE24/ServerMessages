using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using BattleNET;
using System.Runtime.InteropServices;

namespace ServerMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Starting Client...");

            // Handler to listen for close window
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            // Creating necessary files if they don't exist yet
            createFile("messages.cfg");
            createFile("settings.cfg");
            if(settings.saveChat == "true")
                createFile("chat.log");

            // If savechat is true, connect, otherwise try connection and disconnect
            beConn.b.BattlEyeConnected += BattlEyeConnected;
            beConn.b.BattlEyeDisconnected += BattlEyeDisconnected;
            if (settings.saveChat == "true")
            {
                beConn.b.BattlEyeMessageReceived += writeChatLog;
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Listening to chat...");
            }
            beConn.b.ReconnectOnPacketLoss = true;

            beConn.b.Connect();

            // Start timeChecker
            timeChecker timecheckObj = new timeChecker();
            Thread timecheckThread = new Thread(timecheckObj.Check);
            timecheckThread.Start();
        }

        private static void BattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            // This method decides what to do when getting connected to the BE server
            if (args.ConnectionResult == BattlEyeConnectionResult.Success)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Succesful connection to BE server.");
            }
            if (args.ConnectionResult == BattlEyeConnectionResult.InvalidLogin)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Connection error: Invalid login details! Please check your password in settings.cfg. Press ENTER to exit");
                Console.ReadLine();
                Environment.Exit(0);
            }
            if (args.ConnectionResult == BattlEyeConnectionResult.ConnectionFailed)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Connection error: server unreachable! Checking connection in 5 seconds...");
                Thread.Sleep(5000);
                if(!beConn.b.Connected) beConn.b.Connect();
            }
        }

        private static void BattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            // This method decides what to do when getting disconnected from the BE server
            if (args.DisconnectionType == BattlEyeDisconnectionType.SocketException)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Connection lost: Socket exception! Reconnecting in 5 seconds...");
                Thread.Sleep(5000);
                if (!beConn.b.Connected) 
                    beConn.b.Connect();
            }
            if (args.DisconnectionType == BattlEyeDisconnectionType.Manual)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Connection lost: Disconnected");
            }
        }

        private static void writeChatLog(BattlEyeMessageEventArgs args)
        {
            // This method writes sidechat to the log file

            // First check whether chat msg was a side-chat msg
            if (args.Message.Substring(0, Math.Min(args.Message.Length, 6)) == "(Side)")
            {
                // Check if linecount >= amount specified
                if (File.ReadLines(@"chat.log").Count() >= Convert.ToInt16(settings.splitChatLogAt))
                {
                    // If so, rename file and create new one
                    string fileNameWithTime = "chat_" + DateTime.Now.ToString("dd_MM_yyyy-HH_mm_ss") + ".log";
                    File.Move(@"chat.log", @fileNameWithTime);
                    createFile("chat.log");
                }
                // Write to logfile
                TextWriter chatlog = new StreamWriter(@"chat.log", append: true);
                chatlog.WriteLine(args.Message.Remove(0, 7));
                chatlog.Close();
            }
            
        }

        private static void createFile(String filename)
        {
            // This method creates files if they don't exist
            if (!File.Exists(@filename))
            {
                using (FileStream fileCreate = File.Create(@filename))
                {
                    fileCreate.Close();  
                }
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Created file: " + filename);
            }
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2 || eventType == 0)
            {
                // Disconnect from server on close
                if(beConn.b.Connected) 
                    beConn.b.Disconnect();
            }
            return false;
        }

        // Close handler stuff
        static ConsoleEventDelegate handler;
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}