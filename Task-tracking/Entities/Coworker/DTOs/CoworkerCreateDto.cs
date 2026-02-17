using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace TaskTracking.Entities.Coworker
{
    public class CoworkerCreateDto
    {
        [BindRequired]
        public string Name { get; set; }
        [BindRequired]
        public DateTimeOffset Birthday { get; set; }
        [BindRequired]
        [EmailAddress]
        public string EMail { get; set; }
        [BindRequired]
        [EnumDataType(typeof(Position))]
        public Position Position { get; set; }
        [BindRequired]
        [StringLength(128, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$")]
        public string Password { get; set; }
        public string FavoriteToy { get; set; } = "shoe";
    }
}
