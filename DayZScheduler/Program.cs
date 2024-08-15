

using BattleNET;
using DayZScheduler.Classes.SerializationClasses.ManagerConfigClasses;
using System;
using System.Net;
using DayZScheduler.Classes.SerializationClasses.Serializers;
using DayZScheduler.Classes.SerializationClasses.BecClasses;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace DayZScheduler 
{
    class Manager
    {
        private static BattlEyeClient client;
        private static ManagerConfig config;
        private static SchedulerFile scheduler;
        private static List<Timer> tasks;
        private static bool stop;

        static void Main(string[] args)
        {
            config = JSONSerializer.DeserializeJSONFile<ManagerConfig>("Config.json");
            if (config == null )
            {
                config = new ManagerConfig();
            }
            JSONSerializer.SerializeJSONFile<ManagerConfig>("Config.json", config);

            List<string> directories = FileSystem.GetDirectories(AppContext.BaseDirectory).ToList<string>();
            if (directories.Find(x => Path.GetFileName(x) == "Config") == null)
            {
                FileSystem.CreateDirectory("Config");
            }

            scheduler = XMLSerializer.DeserializeXMLFile<SchedulerFile>(Path.Combine("Config", config.Scheduler));
            if (scheduler == null )
            {
                scheduler = CreateNewSchedulerFile();
            }
            XMLSerializer.SerializeXMLFile<SchedulerFile>(Path.Combine("Config", config.Scheduler), scheduler);

            Connect(config.IP, config.Port, config.Password);

            int scheduledItems = 0;

            tasks = new List<Timer>();
            foreach (JobItem item in scheduler.JobItems)
            {
                DateTime now = DateTime.Now;
                List<string> time = item.start.Split(':').ToList<string>();
                DateTime scheduledJob = DateTime.Today.AddHours(Convert.ToDouble(time[0])).AddMinutes(Convert.ToDouble(time[1])).AddSeconds(Convert.ToDouble(time[2]));
                TimeSpan waitTime = scheduledJob - now;
                if (waitTime.TotalSeconds < 0)
                {
                    scheduledJob = scheduledJob.AddDays(1);
                    waitTime = scheduledJob - now;
                }
                tasks.Add(new Timer((object state) => { ExecuteFunction(item); }, null, waitTime, TimeSpan.FromDays(1)));
                scheduledItems++;
            }
            WriteToConsole($"{scheduledItems} tasks were scheduled");
            while (tasks.Count > 0 && !stop)
            {
                Thread.Sleep(60000);
            }
        }

        private static void ExecuteFunction(JobItem item)
        {
            WriteToConsole($"Sending command {item.cmd}");
            SendCommand(item.cmd);
        }

        public static void SendCommand(string command)
        {
            client.SendCommand(command);
        }

        public static void Connect(string ip, int port, string password)
        {
            BattlEyeLoginCredentials credentials = new BattlEyeLoginCredentials
            {
                Host = IPAddress.Parse(ip),
                Port = port,
                Password = password
            };
            client = new BattlEyeClient(credentials);
            client.Connect();
        }

        public static void Disconnect()
        {
            client.Disconnect();
        }

        private static SchedulerFile CreateNewSchedulerFile()
        {
            int interval = 1;

            int id = 0;
            string days = "1,2,3,4,5,6,7";
            string runtime = "000000";
            int loop = 0;
            string cmdShutdown = "#shutdown";
            string cmdLock = "#lock";
            string oneHourCmd = "say -1 Alert: The Server is restarting in 1 hour";
            string thirtyMinuteCmd = "say -1 Alert: The Server is restarting in 30 minutes";
            string fifteenMinuteCmd = "say -1 Alert: The Server is restarting in 15 minutes";
            string fiveMinuteCmd = "say -1 Alert: The Server is restarting in 5 minutes! Please land your helicopters as soon as possible!";
            string oneMinuteCmd = "say -1 Alert: The Server is restarting in 1 minute! Please log out in order to prevent inventory loss!";
            string restartingNowCmd = "say -1 Alert: The Server is restarting now!!";

            //string joinDCmsg = "say -1 Press B for more information on the server or ask on Discord!";

            //sch.JobItems.Add(new JobItem(id, days, "001000", "002000", -1, joinDCmsg));
            //id++;

            if (interval != 0)
            {
                SchedulerFile sch = new SchedulerFile();
                sch.JobItems = new List<JobItem>();
                for (int i = 0; i < 24; i++)
                {
                    string hour;
                    if (i < 10)
                    {
                        hour = $"0{i}";
                    }
                    else
                    {
                        hour = $"{i}";
                    }

                    if (i % interval == interval - 1 || interval == 1)
                    {
                        if (interval == 1)
                        {
                            string start = $"{hour}:00:00";
                            sch.JobItems.Add(new JobItem(id, days, start, runtime, loop, cmdShutdown));
                            id++;
                        }
                        else
                        {
                            sch.JobItems.Add(new JobItem(id, days, $"{hour}:00:00", runtime, loop, oneHourCmd));
                            id++;
                        }
                        sch.JobItems.Add(new JobItem(id, days, $"{hour}:30:00", runtime, loop, thirtyMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, days, $"{hour}:45:00", runtime, loop, fifteenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, days, $"{hour}:55:00", runtime, loop, fiveMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, days, $"{hour}:59:00", runtime, loop, oneMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, days, $"{hour}:59:00", runtime, loop, cmdLock));
                        id++;
                        sch.JobItems.Add(new JobItem(id, days, $"{hour}:59:50", runtime, loop, restartingNowCmd));
                        id++;

                    }
                    else if (i % interval == 0)
                    {
                        string start = $"{hour}:00:00";
                        sch.JobItems.Add(new JobItem(id, days, start, runtime, loop, cmdShutdown));
                        id++;
                    }
                }
                return sch;
            }
            else
            {
                return null;
            }
        }

        public static void WriteToConsole(string message)
        {
            System.Console.WriteLine(Environment.NewLine + DateTime.Now.ToString("[HH:mm:ss]") + message);
        }
    }
}