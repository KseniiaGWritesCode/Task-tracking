using System.ComponentModel.DataAnnotations;

namespace TaskTracking.Entities.Coworker
{
    public class CoworkerGetAllDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
    }
}
