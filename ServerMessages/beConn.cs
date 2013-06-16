using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleNET;

namespace ServerMessages
{
    public static class beConn
    {
        public static BattlEyeClient b { get; set; }

        static beConn()
        {
            b = new BattlEyeClient(settings.loginCredentials);
        }
    }
}