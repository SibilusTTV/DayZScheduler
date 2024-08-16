
using BytexDigital.BattlEye.Rcon;

namespace DayZScheduler.Classes.Network
{
    internal class RCON
    {
        private RconClient client;

        public RCON(string ip, int port, string password)
        {
            client = Connect(ip, port, password);
        }

        private RconClient Connect(string ip, int port, string password)
        {
            Manager.WriteToConsole($"Connecting to {ip}:{port} with password {password}");
            RconClient _client = new RconClient(ip, port, password);
            _client.MessageReceived += _client_MessageReceived;
            _client.Connect();
            _client.WaitUntilConnected();
            return _client;
        }

        private void _client_MessageReceived(object? sender, string e)
        {
            Manager.WriteToConsole(e);
        }

        public void SendCommand(string command)
        {
            client.Send(command);
            if (command == "#shutdown")
            {
                Manager.stop = true;
            }
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public bool IsConnected()
        {
            return client.IsConnected;
        }
    }
}
