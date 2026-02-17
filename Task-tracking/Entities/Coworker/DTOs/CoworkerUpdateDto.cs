using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace TaskTracking.Entities.Coworker
{
    public class CoworkerUpdateDto
    {
        [BindRequired]
        public string Name { get; set; }
        [BindRequired]
        public DateTimeOffset Birthday { get; set; }
        [BindRequired]
        [EnumDataType(typeof(Position))]
        public Position Position { get; set; }
        public string FavoriteToy { get; set; } = "shoe";
    }
}
