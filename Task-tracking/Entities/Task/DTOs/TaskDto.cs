using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskTracking.Entities.Project;

namespace TaskTracking.Entities.Task
{
    public class TaskDto
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
        public int? ProjectId { get; set; }
        [Required]
        public int? ManagerId { get; set; }
        [Required]
        public int? EmployeeId { get; set; }
    }
}
