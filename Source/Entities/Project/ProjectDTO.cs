using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public int ManagerId { get; set; }
    }
}
