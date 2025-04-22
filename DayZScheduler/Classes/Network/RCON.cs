
using BytexDigital.BattlEye.Rcon;

namespace DayZScheduler.Classes.Network
{
    internal class RCON
    {
        private RconClient _client;
        private int _playersCount;
        private List<Player> _players;

        public RCON(string ip, int port, string password)
        {
            _client = Connect(ip, port, password);
            _playersCount = 0;
            _players = new List<Player>();
        }

        public int PlayersCount { get { return _playersCount; } }
        public List<Player> Players { get { return _players; } }

        private RconClient Connect(string ip, int port, string password)
        {
            Manager.WriteToConsole($"Connecting to {ip}:{port} with password {password}");
            RconClient _client = new RconClient(ip, port, password);
            _client.MessageReceived += _client_MessageReceived;
            _client.PlayerConnected += _client_PlayerConnected;
            _client.PlayerDisconnected += _client_PlayerDisconnected;
            _client.PlayerRemoved += _client_PlayerRemoved;
            _client.Disconnected += _client_Disconnected;
            _client.ReconnectOnFailure = false;
            _client.Connect();
            _client.WaitUntilConnected(10);
            return _client;
        }

        private void _client_Disconnected(object? sender, EventArgs e)
        {
            Manager.stop = true;
        }

        private void _client_PlayerRemoved(object? sender, BytexDigital.BattlEye.Rcon.Events.PlayerRemovedArgs e)
        {
            int playerIndex = _players.FindIndex(x => x.Name == e.Name && x.Id == e.Id && x.Guid == e.Guid);
            if (playerIndex < 0)
            {
                _playersCount--;
                _players.RemoveAt(playerIndex);
            }
            Manager.WriteToConsole($"Player {e.Name} removed. Current player count is {PlayersCount}");
        }

        private void _client_PlayerDisconnected(object? sender, BytexDigital.BattlEye.Rcon.Events.PlayerDisconnectedArgs e)
        {
            int playerIndex = _players.FindIndex(x => x.Name == e.Name && x.Id == e.Id);
            if (playerIndex < 0)
            {
                _playersCount--;
                _players.RemoveAt(playerIndex);
            }
            Manager.WriteToConsole($"Player {e.Name} disconnected. Current player count is {PlayersCount}");
        }

        private void _client_PlayerConnected(object? sender, BytexDigital.BattlEye.Rcon.Events.PlayerConnectedArgs e)
        {
            _playersCount++;
            _players.Add(new Player(e.Name, e.Id, e.Guid));
            Manager.WriteToConsole($"Player {e.Name} connected. Current player count is {PlayersCount}");
        }

        private void _client_MessageReceived(object? sender, string e)
        {
            Manager.WriteToConsole(e);
        }

        public void SendCommand(string command)
        {
            _client.Send(command);
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public bool IsConnected()
        {
            return _client.IsConnected;
        }
    }
}
