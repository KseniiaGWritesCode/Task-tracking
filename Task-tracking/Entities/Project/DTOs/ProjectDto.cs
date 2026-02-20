using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskTracking.Entities.Coworker;

namespace TaskTracking.Entities.Project
{
    public class ProjectDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTimeOffset? DueDate { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Priority? Priority { get; set; }
        [Required]
        public int? ManagerId { get; set; }
    }
}
