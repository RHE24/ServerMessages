using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace ServerMessages
{
    class timeChecker
    {
        public void Check()
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Reading messages...");
            string messagesFile = File.ReadAllText(@"messages.cfg");
            string[] messages = messagesFile.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            int timeLoop = 0;
            while (true)
            {
                // If it has been +-10min since last read of messages file, read again
                if (timeLoop > 599)
                {
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Reading messages...");
                    messagesFile = File.ReadAllText(@"messages.cfg");
                    messages = messagesFile.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    timeLoop = 0;
                }

                // For each message, check if time specified equals current time, if so say message globally.
                foreach (string x in messages)
                {
                    string[] messageThis = x.Split('~');
                    int typeLoop = 0;
                    if (messageThis[2] != "no-loop")
                        typeLoop = Convert.ToInt16(messageThis[2].Remove(0, 5));
                    DateTime checkDateTime = DateTime.ParseExact(messageThis[0], "HH:mm:ss", null);

                    if (typeLoop == 0)
                    {
                        if (checkDateTime.ToString("HH:mm:ss") == DateTime.Now.ToString("HH:mm:ss"))
                            publicMethods.sayGlobal(messageThis[1]);
                    }
                    else
                    {
                        DateTime originalDateTime = checkDateTime;
                        do
                        {
                            checkDateTime = checkDateTime.AddHours(typeLoop);
                            if (checkDateTime.ToString("HH:mm:ss") == DateTime.Now.ToString("HH:mm:ss"))
                                publicMethods.sayGlobal(messageThis[1]);
                        } while (checkDateTime.ToString("HH:mm:ss") != originalDateTime.ToString("HH:mm:ss"));
                    }
                }

                timeLoop++;
                Thread.Sleep(1000);
            }
        }
    }
}
