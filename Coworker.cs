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

        public Coworker(string name, DateTime birthday, string eMail, Position position) 
        { 
            Name = name;
            Birthday = birthday;
            EMail = eMail;
            Position = position;
        }
    }
}
