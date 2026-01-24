using System.ComponentModel.DataAnnotations;

namespace TaskTracking.Entities.Coworker
{
    public class CoworkerGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Birthday { get; set; }
        public string EMail { get; set; }
        public Position Position { get; set; }
    }
}
