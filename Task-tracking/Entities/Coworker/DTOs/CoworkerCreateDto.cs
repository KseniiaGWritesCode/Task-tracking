using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskTracking.Entities.Coworker
{
    public class CoworkerCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTimeOffset? Birthday { get; set; }
        [Required]
        [EmailAddress]
        public string EMail { get; set; }
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Position? Position { get; set; }
        [Required]
        [StringLength(128, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$")]
        public string Password { get; set; }
        public string FavoriteToy { get; set; } = "shoe";
    }
}
