﻿using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DayZScheduler.Classes.SerializationClasses.ManagerConfigClasses
{
    internal class ManagerConfig
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public int Interval { get; set; }
        public bool IsOnUpdate { get; set; }
        public bool OnlyRestarts {  get; set; }
        public string BePath { get; set; }
        public bool AutoLoadBans {  get; set; }
        public int Ban {  get; set; }
        public bool AsciiNickOnly { get; set; }
        public bool AsciiChatOnly { get; set; }
        public List<string> IgnoreChatChars { get; set; }
        public int Warnings { get; set; }
        public List<char> DisallowPlayerNameChars { get; set; }
        public int MinPlayerNameLength { get; set; }
        public int MaxPlayerNameLength { get; set; }
        public bool UseWordFilter { get; set; }
        public string WordFilterFile { get; set; }
        public bool UseWhiteList { get; set; }
        public string WhiteListFile { get; set; }
        public string WhiteListKickMsg { get; set; }
        public bool UseNickFilter { get; set; }
        public string NickFilterFile { get; set; }
        public string Scheduler { get; set; }
        public int KickLobbyIdlers { get; set; }
        public bool ChatChannelFiles { get; set; }
        public int SlotLimit { get; set; }
        public string SlotLimitKickMsg { get; set; }
        public int Timeout { get; set; }

        public ManagerConfig()
        {
            IP = "127.0.0.1";
            Port = 2306;
            Password = "YourRConPassword";
            Interval = 4;
            OnlyRestarts = false;
            IsOnUpdate = false;
            BePath = Path.Combine("..", "Server", "Profiles", "BattlEye");
            AutoLoadBans = true;
            Ban = 3;
            AsciiNickOnly = false;
            AsciiChatOnly = true;
            IgnoreChatChars = new List<string> {
                "€",
                "£",
                "Æ",
                "ø",
                "Ø",
                "å",
                "Å",
                "ö",
                "ä",
                "ü",
                "ß"
            };
            Warnings = 3;
            DisallowPlayerNameChars = new List<char>{
                '[',
                ']',
                '{',
                '}',
                '(',
                ')',
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9'
            };
            MinPlayerNameLength = 3;
            MaxPlayerNameLength = 16;
            UseWordFilter = true;
            WordFilterFile = "BadWords.txt";
            UseWhiteList = false;
            WhiteListFile = "WhiteList.txt";
            WhiteListKickMsg = "You are not whitelisted on this server.";
            UseNickFilter = true;
            NickFilterFile = "BadNames.txt";
            Scheduler = "Scheduler.json";
            KickLobbyIdlers = 300;
            ChatChannelFiles = true;
            SlotLimit = -1;
            SlotLimitKickMsg = "The Server has reached its player limit.";
            Timeout = 60;
        }
    }
}
