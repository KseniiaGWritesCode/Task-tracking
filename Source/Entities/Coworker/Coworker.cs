using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class Coworker
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Birthday { get; set; }
        public string EMail { get; set; }
        public Position Position { get; set; }
        public string Password { get; set; }
        public string FavoriteToy { get; set; } = "default";
    }
}
