using BattleNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DayZScheduler.Classes.Network
{
    internal class RCON
    {
        private BattlEyeClient client;

        public RCON(string ip, int port, string password)
        {
            Connect(ip, port, password);
        }

        private void Connect(string ip, int port, string password)
        {
            BattlEyeLoginCredentials credentials = new BattlEyeLoginCredentials
            {
                Host = IPAddress.Parse(ip),
                Port = port,
                Password = password
            };
            client = new BattlEyeClient(credentials);
            client.BattlEyeDisconnected += OnDisconnect;
            client.Connect();
        }

        public void SendCommand(string command)
        {
            client.SendCommand(command);
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        private void OnDisconnect(BattlEyeDisconnectEventArgs args)
        {
            Manager.stop = true;
        }
    }
}
