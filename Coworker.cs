using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class Coworker : ICoworkers
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string EMail { get; set; }
        public Position Position { get; set; }
        public Project Project { get; set; }
        public TaskItem Task { get; set; }

        public Coworker(string name, DateTime birthday, string mail, Position position, KeeperOfData projects, KeeperOfData tasks) 
        { 
            Name = name;
            Birthday = birthday;
            EMail = mail;
            Position = position;
        }
    }
}
