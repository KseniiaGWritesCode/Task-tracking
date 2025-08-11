using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class KeeperOfData
    {
        public List <Coworker> Coworkers = new List<Coworker>();
        public List<TaskItem> Tasks = new List<TaskItem>();
        public List<Project> Projects = new List<Project>();
    }
}
