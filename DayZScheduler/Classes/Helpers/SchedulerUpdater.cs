using DayZScheduler.Classes.SerializationClasses.BecClasses;
using DayZScheduler.Classes.SerializationClasses.ManagerConfigClasses;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DayZScheduler.Classes.Helpers
{
    internal static class SchedulerUpdater
    {
        public static SchedulerFile? CreateNewSchedulerFile(ManagerConfig config)
        {
            if (config != null)
            {
                if (config.IsOnUpdate)
                {
                    return CreateOnUpdateSchedulerFile(config.Interval);
                }
                else
                {
                    if (config.OnlyRestarts)
                    {
                        return CreateOnlyRestartsSchedulerFile(config);
                    }
                    else
                    {
                        return CreateFullSchedulerFile(config);
                    }
                }
            }
            else
            {
                return null;
            }
        }

        private static SchedulerFile CreateFullSchedulerFile(ManagerConfig config)
        {
            int interval = config.Interval;
            SchedulerFile sch = new SchedulerFile();
            sch.JobItems = new List<JobItem>();
            int id = 0;

            foreach (JobItem item in config.CustomMessages)
            {
                item.ID = id;
                sch.JobItems.Add(item);
                id++;
            }

            if (interval > 0)
            {
                Dictionary<string, double> runtime = new Dictionary<string, double>
                {
                    {"hours", 0},
                    {"minutes", 0},
                    {"seconds", 0}

                };
                Dictionary<string, double> runtimeDCmsg = new Dictionary<string, double>
                {
                    {"hours", 0},
                    {"minutes", 20},
                    {"seconds", 0}

                };
                int loop = 0;
                int restartNowLoop = 5;
                string cmdShutdown = "#shutdown";
                string cmdLock = "#lock";
                string oneHourCmd = "say -1 Alert: The Server is restarting in 1 hour";
                string thirtyMinuteCmd = "say -1 Alert: The Server is restarting in 30 minutes";
                string fifteenMinuteCmd = "say -1 Alert: The Server is restarting in 15 minutes";
                string fiveMinuteCmd = "say -1 Alert: The Server is restarting in 5 minutes! Please land your helicopters as soon as possible!";
                string oneMinuteCmd = "say -1 Alert: The Server is restarting in 1 minute! Please log out in order to prevent inventory loss!";
                string restartingNowCmd = "say -1 Alert: The Server is restarting now!!";
                
                string serverRestartNotice = "";
                if (interval == 1)
                {
                    serverRestartNotice = $"say -1 The Server restarts every hour";
                }
                else
                {
                    serverRestartNotice = $"say -1 The Server restarts every {interval} hours";
                }

                sch.JobItems.Add(new JobItem(id, false, new Dictionary<string, double> { { "hours", 0 }, { "minutes", 10 }, { "seconds", 0 } }, runtimeDCmsg, -1, serverRestartNotice));
                id++;

                for (int i = 0; i < 24; i++)
                {
                    if (i % interval == interval - 1 || interval == 1)
                    {
                        if (interval == 1)
                        {
                            sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", i }, { "minutes", 0 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                            id++;
                        }
                        else
                        {
                            sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", i }, { "minutes", 0 }, { "seconds", 0 } }, runtime, loop, oneHourCmd));
                            id++;
                        }
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", i }, { "minutes", 30 }, { "seconds", 0 } }, runtime, loop, thirtyMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", i }, { "minutes", 45 }, { "seconds", 0 } }, runtime, loop, fifteenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", i }, { "minutes", 55 }, { "seconds", 0 } }, runtime, loop, fiveMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", i }, { "minutes", 59 }, { "seconds", 0 } }, runtime, loop, oneMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", i }, { "minutes", 59 }, { "seconds", 0 } }, runtime, loop, cmdLock));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", i }, { "minutes", 59 }, { "seconds", 50 } }, runtime, restartNowLoop, restartingNowCmd));

                    }
                    else if (i % interval == 0)
                    {
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", i }, { "minutes", 0 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                        id++;
                    }
                }
            }
            return sch;
        }
        
        private static SchedulerFile CreateOnlyRestartsSchedulerFile(ManagerConfig config)
        {
            int interval = config.Interval;
            SchedulerFile sch = new SchedulerFile();
            sch.JobItems = new List<JobItem>();
            int id = 0;

            foreach (JobItem item in config.CustomMessages)
            {
                item.ID = id;
                sch.JobItems.Add(item);
                id++;
            }

            if (interval > 0)
            {
                Dictionary<string, double> runtime = new Dictionary<string, double>
                {
                    { "hours", 0 },
                    { "minutes", 0 },
                    { "seconds", 0 }
                };
                int loop = 0;
                string cmdShutdown = "#shutdown";
                string cmdLock = "#lock";

                for (int i = 0; i < 24; i++)
                {
                    if (i % interval == interval - 1 || interval == 1)
                    {
                        if (interval == 1)
                        {
                            sch.JobItems.Add(new JobItem(id, false, new Dictionary<string, double> { { "hours", i }, { "minutes", 0 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                            id++;
                        }
                        sch.JobItems.Add(new JobItem(id, false, new Dictionary<string, double> { { "hours", i }, { "minutes", 59 }, { "seconds", 0 } }, runtime, loop, cmdLock));
                        id++;
                    }
                    else if (i % interval == 0)
                    {
                        sch.JobItems.Add(new JobItem(id, false, new Dictionary<string, double> { { "hours", i }, { "minutes", 0 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                        id++;
                    }
                }
            }
            return sch;
        }

        private static SchedulerFile? CreateOnUpdateSchedulerFile(int interval)
        {
            if (interval > 0)
            {
                SchedulerFile sch = new SchedulerFile();
                sch.JobItems = new List<JobItem>();
                int id = 0;
                Dictionary<string, double> runtime = new Dictionary<string, double>
                {
                    { "hours", 0 },
                    { "minutes", 0 },
                    { "seconds", 0 }
                };
                int loop = 0;
                int restartNowLoop = 5;
                string cmdShutdown = "#shutdown";
                string cmdLock = "#lock";
                string twentyMinuteCmd = "say -1 Alert: The Server is restarting in 20 minutes to load updated mods! Please restart your game afterwards!";
                string fifteenMinuteCmd = "say -1 Alert: The Server is restarting in 15 minutes to load updated mods! Please restart your game afterwards!";
                string tenMinuteCmd = "say -1 Alert: The Server is restarting in 10 minutes to load updated mods! Please restart your game afterwards!";
                string fiveMinuteCmd = "say -1 Alert: The Server is restarting in 5 minutes to load updated mods! ! Please land your helicopters as soon as possible and restart your game afterwards!";
                string oneMinuteCmd = "say -1 Alert: The Server is restarting in 1 minute to load updated mods! ! Please log out in order to prevent inventory loss and restart your game afterwards!";
                string restartingNowCmd = "say -1 Alert: The Server is restarting now to load updated mods!! Please your restart your game afterwards!";
                DateTime currentTime = DateTime.Now;

                double hour = currentTime.Hour;

                double nextHour = currentTime.AddHours(1).Hour;

                if (hour % interval == interval - 1 || interval == 1)
                {
                    if (currentTime.Minute >= 0 && currentTime.Minute < 5)
                    {
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 5 }, { "seconds", 0 } }, runtime, loop, tenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 10 }, { "seconds", 0 } }, runtime, loop, fiveMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 14 }, { "seconds", 0 } }, runtime, loop, oneMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 14 }, { "seconds", 0 } }, runtime, loop, cmdLock));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 14 }, { "seconds", 50 } }, runtime, restartNowLoop, restartingNowCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 15 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                        return sch;
                    }
                    else if (currentTime.Minute >= 5 && currentTime.Minute < 20)
                    {
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 10 }, { "seconds", 0 } }, runtime, loop, twentyMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 15 }, { "seconds", 0 } }, runtime, loop, fifteenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 20 }, { "seconds", 0 } }, runtime, loop, tenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 25 }, { "seconds", 0 } }, runtime, loop, fiveMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 29 }, { "seconds", 0 } }, runtime, loop, oneMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 29 }, { "seconds", 0 } }, runtime, loop, cmdLock));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 29 }, { "seconds", 50 } }, runtime, restartNowLoop, restartingNowCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 30 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                        return sch;
                    }
                }
                else
                {
                    if (currentTime.Minute >= 0 && currentTime.Minute < 5)
                    {
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 5 }, { "seconds", 0 } }, runtime, loop, tenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 10 }, { "seconds", 0 } }, runtime, loop, fiveMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 14 }, { "seconds", 0 } }, runtime, loop, oneMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 14 }, { "seconds", 0 } }, runtime, loop, cmdLock));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 14 }, { "seconds", 50 } }, runtime, restartNowLoop, restartingNowCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 15 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                        return sch;
                    }
                    else if (currentTime.Minute >= 5 && currentTime.Minute < 20)
                    {
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 10 }, { "seconds", 0 } }, runtime, loop, twentyMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 15 }, { "seconds", 0 } }, runtime, loop, fifteenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 20 }, { "seconds", 0 } }, runtime, loop, tenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 25 }, { "seconds", 0 } }, runtime, loop, fiveMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 29 }, { "seconds", 0 } }, runtime, loop, oneMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 29 }, { "seconds", 0 } }, runtime, loop, cmdLock));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 29 }, { "seconds", 50 } }, runtime, restartNowLoop, restartingNowCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 30 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                        return sch;
                    }
                    else if (currentTime.Minute >= 20 && currentTime.Minute < 35)
                    {
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 25 }, { "seconds", 0 } }, runtime, loop, twentyMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 30 }, { "seconds", 0 } }, runtime, loop, fifteenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 35 }, { "seconds", 0 } }, runtime, loop, tenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 40 }, { "seconds", 0 } }, runtime, loop, fiveMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 44 }, { "seconds", 0 } }, runtime, loop, oneMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 44 }, { "seconds", 0 } }, runtime, loop, cmdLock));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 44 }, { "seconds", 50 } }, runtime, restartNowLoop, restartingNowCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 45 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                        return sch;
                    }
                    else if (currentTime.Minute >= 35 && currentTime.Minute < 50)
                    {
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 40 }, { "seconds", 0 } }, runtime, loop, twentyMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 45 }, { "seconds", 0 } }, runtime, loop, fifteenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 50 }, { "seconds", 0 } }, runtime, loop, tenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 55 }, { "seconds", 0 } }, runtime, loop, fiveMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 59 }, { "seconds", 0 } }, runtime, loop, oneMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 59 }, { "seconds", 0 } }, runtime, loop, cmdLock));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 59 }, { "seconds", 50 } }, runtime, restartNowLoop, restartingNowCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", nextHour }, { "minutes", 0 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                        return sch;
                    }
                    else if (currentTime.Minute >= 50 && currentTime.Minute < 60)
                    {

                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", hour }, { "minutes", 55 }, { "seconds", 0 } }, runtime, loop, twentyMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", nextHour }, { "minutes", 0 }, { "seconds", 0 } }, runtime, loop, fifteenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", nextHour }, { "minutes", 5 }, { "seconds", 0 } }, runtime, loop, tenMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", nextHour }, { "minutes", 10 }, { "seconds", 0 } }, runtime, loop, fiveMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", nextHour }, { "minutes", 14 }, { "seconds", 0 } }, runtime, loop, oneMinuteCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", nextHour }, { "minutes", 14 }, { "seconds", 0 } }, runtime, loop, cmdLock));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", nextHour }, { "minutes", 14 }, { "seconds", 50 } }, runtime, restartNowLoop, restartingNowCmd));
                        id++;
                        sch.JobItems.Add(new JobItem(id, true, new Dictionary<string, double> { { "hours", nextHour }, { "minutes", 15 }, { "seconds", 0 } }, runtime, loop, cmdShutdown));
                        return sch;
                    }
                }
            }
            return null;
        }
    }
}
