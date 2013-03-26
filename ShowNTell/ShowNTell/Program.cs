using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Onoffswitch.NetDuinoUtils.Utils;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace ShowNTell
{
    public class Program
    {
        public static void Main()
        {
            // write your code here

            LcdWriter.Instance.Write("BOOM!");

            NetDuinoUtils.KeepRunning();
        }

    }
}
