using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayZScheduler.Classes
{
    internal class Player
    {
        private string _name;
        private int _id;
        private string _guid;

        public Player(string name, int id, string guid)
        {
            _name = name;
            _id = id;
            _guid = guid;
        }

        public string Name { get { return _name; } }
        public int Id { get { return _id; } }
        public string Guid { get { return _guid; } }
    }
}
