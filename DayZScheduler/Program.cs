
using DayZScheduler.Classes.Helpers;
using DayZScheduler.Classes.Network;
using DayZScheduler.Classes.SerializationClasses.SchedulerFileClasses;
using DayZScheduler.Classes.SerializationClasses.SchedulerConfigClasses;
using DayZScheduler.Classes.SerializationClasses.Serializers;
using Microsoft.VisualBasic.FileIO;

namespace DayZScheduler
{
    class Manager
    {
        private static RCON? rconClient;
        private static SchedulerConfig? config;
        private static SchedulerFile? scheduler;
        private static List<Timer>? tasks;
        public static bool stop = false;

        #region Constants
        public const string CONFIG_FOLDER = "Config";
        public static string CONFIG_NAME = "Config.json";
        #endregion Constants

        static void Main(string[] args)
        {
            GetStartParameters(args);

            try
            {
                List<string> directories = FileSystem.GetDirectories(AppContext.BaseDirectory).ToList<string>();
                if (directories.Find(x => Path.GetFileName(x) == CONFIG_FOLDER) == null)
                {
                    FileSystem.CreateDirectory(CONFIG_FOLDER);
                }
            }
            catch (Exception ex)
            {
                WriteToConsole(ex.ToString());
                return;
            }

            config = JSONSerializer.DeserializeJSONFile<SchedulerConfig>(Path.Combine(CONFIG_FOLDER, CONFIG_NAME));
            if (config == null)
            {
                config = new SchedulerConfig();
            }
            JSONSerializer.SerializeJSONFile<SchedulerConfig>(Path.Combine(CONFIG_FOLDER, CONFIG_NAME), config);

            if (config.Interval <= 0)
            {
                WriteToConsole("The interval needs to be at least 1");
                return;
            }

            scheduler = SchedulerUpdater.CreateNewSchedulerFile(config);
            if (scheduler == null)
            {
                WriteToConsole("It's not a good time to update");
                return;
            }
            else
            {
                JSONSerializer.SerializeJSONFile<SchedulerConfig>(Path.Combine(CONFIG_FOLDER, config.Scheduler), scheduler);
            }


            WriteToConsole($"Waiting for {config.Timeout} seconds until TimeOut is over");
            Thread.Sleep(config.Timeout * 1000);

            CreateTasks(scheduler.JobItems);

            WriteToConsole("Connecting to the Server");
            rconClient = new RCON(config.IP, config.Port, config.Password);
            if (!rconClient.IsConnected())
            {
                WriteToConsole("Couldn't connect to the Server");
                return;
            }
            else
            {
                WriteToConsole("Connected to the Server");
            }

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            WriteToConsole("Scheduling all tasks");
            while (tasks != null && tasks.Count > 0 && !stop)
            {
                Thread.Sleep(60000);
            }
            WriteToConsole("The Server was disconnected");
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            stop = true;
            if (rconClient != null) 
            {
                rconClient.Disconnect();
            }
        }

        private static void GetStartParameters(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-config":
                        if (i + 1 < args.Length)
                        {
                            CONFIG_NAME = args[i + 1];
                        }
                        CONFIG_NAME = args[i + 1];
                        break;
                }
            }
        }

        private static void CreateTasks(List<JobItem> items)
        {
            int scheduledItems = 0;

            tasks = new List<Timer>();
            foreach (JobItem item in items)
            {
                DateTime now = DateTime.Now;
                DateTime scheduledJob;
                if (item.IsTimeOfDay)
                {
                    scheduledJob = DateTime.Today;
                }
                else
                {
                    scheduledJob = DateTime.Now;
                }
                scheduledJob = scheduledJob.AddHours(item.WaitTime["hours"]).AddMinutes(item.WaitTime["minutes"]).AddSeconds(item.WaitTime["seconds"]);

                TimeSpan waitTime = scheduledJob - now;
                if (waitTime.TotalSeconds < 0)
                {
                    scheduledJob = scheduledJob.AddDays(1);
                    waitTime = scheduledJob - now;
                }

                TimeSpan sp;
                if (item.Interval["hours"] == 0 && item.Interval["minutes"] == 0 && item.Interval["seconds"] == 0)
                {
                    sp = TimeSpan.FromDays(1);
                }
                else
                {
                    sp = TimeSpan.FromHours(item.Interval["hours"]);
                    sp.Add(TimeSpan.FromMinutes(item.Interval["minutes"]));
                    sp.Add(TimeSpan.FromSeconds(item.Interval["seconds"]));
                }

                if (item.Loop > 0)
                {
                    for (int i = 0; i < item.Loop; i++)
                    {
                        tasks.Add(new Timer((object state) => { ExecuteFunction(item); }, null, waitTime.Add(TimeSpan.FromSeconds(i)), sp));
                        scheduledItems++;
                    }
                }
                else
                {
                    tasks.Add(new Timer((object state) => { ExecuteFunction(item); }, null, waitTime, sp));
                    scheduledItems++;
                }
            }
            WriteToConsole($"{scheduledItems} tasks were created");
        }

        private static void ExecuteFunction(JobItem item)
        {
            WriteToConsole($"Sending command {item.Cmd}");
            if (rconClient != null)
            {
                rconClient.SendCommand(item.Cmd);
            }
        }

        public static void WriteToConsole(string message)
        {
            System.Console.WriteLine(Environment.NewLine + DateTime.Now.ToString("[HH:mm:ss]") + message);
        }
    }
}