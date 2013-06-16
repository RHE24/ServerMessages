using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerMessages
{
    class publicMethods
    {
        public static void sayGlobal(String saystr)
        {
            // This method sends messages via BE to all players (globally)
            if (!beConn.b.Connected) 
                beConn.b.Connect();
            beConn.b.SendCommand("Say -1 " + saystr);
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Said global: " + saystr);
        }
    }
}
