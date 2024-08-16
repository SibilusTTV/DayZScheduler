using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DayZScheduler.Classes.SerializationClasses.BecClasses
{
    public class SchedulerFile
    {
        public List<JobItem> JobItems { get; set; }
    }
}
