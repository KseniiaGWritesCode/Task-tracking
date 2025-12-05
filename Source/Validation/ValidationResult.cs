using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string ErrorMessage { get; }
        public ValidationResult(bool isValid, string _message = "")
        {
            IsValid = isValid;
            ErrorMessage = _message;
        }
        public static ValidationResult Success() => new(true);
        public static ValidationResult Failure(string message) => new(false, message);
    }
}
