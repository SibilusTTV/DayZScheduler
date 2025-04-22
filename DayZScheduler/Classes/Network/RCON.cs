
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
            _client.PlayerConnected += _client_PlayerConnected;
            _client.PlayerDisconnected += _client_PlayerDisconnected;
            _client.PlayerRemoved += _client_PlayerRemoved;
            _client.MessageReceived += _client_MessageReceived;
            _client.ReconnectOnFailure = false;
            _client.Connect();
            if (Manager.config != null)
            {
                _client.WaitUntilConnected(Manager.config.ConnectTimeout * 1000);
            }
            else
            {
                _client.WaitUntilConnected(10000);
            }
            return _client;
        }

        private void _client_PlayerRemoved(object? sender, BytexDigital.BattlEye.Rcon.Events.PlayerRemovedArgs e)
        {
            int playerIndex = _players.FindIndex(x => x.Name == e.Name && x.Id == e.Id && x.Guid == e.Guid);
            if (playerIndex >= 0)
            {
                if (_playersCount != 0)
                {
                    _playersCount--;
                }
                _players.RemoveAt(playerIndex);
                Manager.WriteToConsole($"Player {e.Name} was removed. Current player count is {PlayersCount}");
            }
        }

        private void _client_PlayerDisconnected(object? sender, BytexDigital.BattlEye.Rcon.Events.PlayerDisconnectedArgs e)
        {
            int playerIndex = _players.FindIndex(x => x.Name == e.Name && x.Id == e.Id);
            if (playerIndex >= 0)
            {
                if (_playersCount != 0)
                {
                    _playersCount--;
                }
                _players.RemoveAt(playerIndex);
                Manager.WriteToConsole($"Player {e.Name} disconnected. Current player count is {PlayersCount}");
            }
        }

        private void _client_PlayerConnected(object? sender, BytexDigital.BattlEye.Rcon.Events.PlayerConnectedArgs e)
        {
            if (Manager.BannedUsers.Count > 0 && Manager.BannedUsers.Contains(e.Guid))
            {
                _client.Send($"kick {e.Id} \"{Manager.config?.BannedMessage}\"");
                Manager.WriteToConsole($"Player {e.Name} was kicked, because they are banned");
                return;
            }
            else if (Manager.config != null && Manager.config.UseNickFilter && Manager.FilteredNicks.Count > 0)
            {
                foreach (string filteredNick in Manager.FilteredNicks)
                {
                    if (e.Name.ToLower().Contains(filteredNick.ToLower()))
                    {
                        _client.Send($"kick {e.Id} \"{Manager.config?.FilteredNickMessage}\"");
                        Manager.WriteToConsole($"Player {e.Name} was kicked, because they are using forbidden words in their user name");
                        return;
                    }
                }
            }

            if (_players.Find(x => x.Name == e.Name && x.Id == e.Id && x.Guid == e.Guid) == null)
            {
                _playersCount++;
                _players.Add(new Player(e.Name, e.Id, e.Guid));
                Manager.WriteToConsole($"Player {e.Name} connected. Current player count is {PlayersCount}");
            }
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
