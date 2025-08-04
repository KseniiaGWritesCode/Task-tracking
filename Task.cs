using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    class Task : IToDoProperties
    {
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        Priority Priority { get; set; }
        Project Project { get; set; }
        Coworker Manager { get; set; }
    }
}
