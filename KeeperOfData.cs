using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class KeeperOfData
    {
        public List <Coworker> Coworkers = new();
        //public List <Coworker> Managers = new();
        public List<Task> Tasks = new List<Task>();
        public List<Project> Projects = new List<Project>();
    }
}
