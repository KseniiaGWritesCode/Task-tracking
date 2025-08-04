using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    interface ICoworkers
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string EMail { get; set; }
    }
}
