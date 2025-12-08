using BCrypt.Net;
using Microsoft.VisualBasic;
using Npgsql;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskTracking
{
    public static class Validator
    {
        //validators before input of values:
        public static Coworker? ValidateLogin(string login)
        {
            Coworker coworker = new Coworker();
            if(login != null)
            {
                string[] checkLogin = login.Split();
                if(checkLogin.Length != 2)
                {
                    ValidationResult.Failure("Not all the required data are in the list.");
                }

                var existingUser = Initializer.GetDbContext().Coworkers.FirstOrDefault(c => c.EMail == checkLogin[0].Trim());
                if (existingUser != null)
                {
                    var passwordHash = existingUser.Password;
                    if (BCrypt.Net.BCrypt.Verify(checkLogin[1].Trim(), passwordHash))
                    {
                        return existingUser;
                    }
                }
            }
            return null;
        }
        public static Commands? ValidateCommand(string userInput)
        {
            Commands command = new Commands();

            if (userInput != null)
            {
                return Enum.TryParse<Commands>(userInput, ignoreCase: true, out command) ?command : null;
                
            }
            return command;
        }
        public static Category? ValidateCategory(string input) 
        {
            Category category = new Category();
            if (input != null)
            {
                return Enum.TryParse<Category>(input.Trim(), ignoreCase: true, out category) ? category : null;
            }
            return category;
        }
        public static bool ValidateAccess(Category? category, Coworker? coworker)
        {
            switch(category)
            {
                case Category.Coworker:
                    return coworker.Position == Position.Admin;
                case Category.Project:
                    return coworker.Position == Position.Admin ||
                           coworker.Position == Position.Manager;
                default:
                    return true;
            };
        }

        //validators for each property: 
        private static ValidationResult OnlyNumbersValidator(string id)
        {
            if(!int.TryParse(id, out int result))
            {
                ValidationResult.Failure("IDs are only numbers!");
            }
            return new ValidationResult(true);
        }
        private static ValidationResult OnlyTextValidator(string text)
        {
            if (!text.All(char.IsLetter))
            {
                ValidationResult.Failure("This field must contain only text!");
            }
            return new ValidationResult(true);
        }
        private static ValidationResult DescriptionValidator(string description)
        {
            if(description.Length < 10)
            {
                ValidationResult.Failure("Description must be at least 10 characters long!");
            }
            return new ValidationResult(true);
        }
        private static ValidationResult NotEmptyFieldValidator(string input)
        {
            if(!input.Any(char.IsLetterOrDigit))
            {
                ValidationResult.Failure("Invalid name format!");
            }
            return new ValidationResult(true);
        }
        public static ValidationResult EmailValidator(string name)
        {
            if (name.Length < 5
                    || !name.Contains('.')
                    || !name.Contains('@'))
            {
                ValidationResult.Failure("The e-mail has invalid format!");
            }
            return new ValidationResult(true);
        }
        private static ValidationResult PasswordValidator(string password)
        {
            if (password.Length < 8
                    || !password.Any(char.IsLetter)
                    || !password.Any(char.IsDigit)
                    || !password.Any(char.IsUpper)
                    || !password.Any(ch => char.IsSymbol(ch) || char.IsPunctuation(ch)))
            {
               ValidationResult.Failure("Password must contain at least: 8 characters, 1 digit, 1 upper-case letter and 1 symbol.");
            }
            return new ValidationResult(true);
        }
        public static ValidationResult PositionValidator(string position)
        {
            if (!Enum.TryParse<Position>(position, ignoreCase: true, out var validPosition))
            {
                ValidationResult.Failure("Position not found.");
            }
            return new ValidationResult(true);
        }
        public static ValidationResult PriorityValidator(string priority)
        {
            if (!Enum.TryParse<Priority>(priority, ignoreCase: true, out var validPriority))
            {
                ValidationResult.Failure("Priority not found.");
            }
            return new ValidationResult(true);
        }
        public static ValidationResult DateTimeValidator(string date)
        {
            if (!DateTime.TryParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime validDate))
            {
                ValidationResult.Failure("Date format isn't correct");
            }
            return new ValidationResult(true);
        }
        public static ValidationResult ManagerValidator(string input)
        {
            int.TryParse(input, out var iD);
            var existingUser = Initializer.GetDbContext().Coworkers.FirstOrDefault(c => c.Id == iD);
            if (existingUser == null)
            {
                ValidationResult.Failure("No user with this ID!");
            }
            if (existingUser.Position != Position.Manager)
            {
                ValidationResult.Failure("This employee isn't a manager!");
            }
            return new ValidationResult(true);
        }
        public static ValidationResult CoworkerValidator(string id)
        {
            int.TryParse(id, out var iD);
            if (!Initializer.GetDbContext().Coworkers.Any(c => c.Id == iD))
            {
                ValidationResult.Failure("No user with this ID!");
            }
            return new ValidationResult(true);
        }
        public static ValidationResult ProjectValidator(string id)
        {
            int.TryParse(id, out var iD);
            if (!Initializer.GetDbContext().Projects.Any(c => c.Id == iD))
            {
                ValidationResult.Failure("No project with this ID!");
            }
            return new ValidationResult(true);
        }
        private static bool ReturnValidationResult(List<ValidationResult> results)
        {
            foreach (var error in results.Where(r => !r.IsValid))
            {
                AnsiConsole.MarkupLine($"[red]{error.ErrorMessage}[/]");
            }
            return results.All(r => r.IsValid);
        }
        private static void ValidateIfNotEmpty(string input, List<ValidationResult> results, Func<string, ValidationResult> validator)
        {
            if(!string.IsNullOrEmpty(input))
            {
                results.Add(validator(input));
            }
            else
            {
                results.Add(ValidationResult.Success());
            }
        }

        //validating input of values for actions:
        public static bool ValidateNewCoworkerData(List<string> data)
        {
            var results = new List<ValidationResult>();
            if (data.Count != 5)
            {
                ValidationResult.Failure("Not all the required data are in the list.");
                return false;
            }

            results.Add(OnlyTextValidator(data[0]));
            results.Add(DateTimeValidator(data[1]));
            results.Add(EmailValidator(data[2]));
            results.Add(PositionValidator(data[3]));
            results.Add(PasswordValidator(data[4]));

            return ReturnValidationResult(results);
        }
        public static bool ValidateNewProjectData(List<string> data)
        {
            var results = new List<ValidationResult>();
            if (data.Count != 5)
            {
                ValidationResult.Failure("Not all the required data are in the list.");
                return false;
            }

            results.Add(NotEmptyFieldValidator(data[0]));
            results.Add(DateTimeValidator(data[1]));
            results.Add(DescriptionValidator(data[2])); 
            results.Add(PriorityValidator(data[3]));
            results.Add(ManagerValidator(data[4]));

            return ReturnValidationResult(results);
        }
        public static bool ValidateNewTaskData(List<string> data)
        {
            var results = new List<ValidationResult>();
            if (data.Count != 7)
            {
                ValidationResult.Failure("Not all the required data are in the list.");
                return false;
            }

            results.Add(NotEmptyFieldValidator(data[0]));
            results.Add(DateTimeValidator(data[1]));
            results.Add(DescriptionValidator(data[2]));
            results.Add(PriorityValidator(data[3]));
            results.Add(ProjectValidator(data[4]));
            results.Add(ManagerValidator(data[5]));
            results.Add(CoworkerValidator(data[6]));

            return ReturnValidationResult(results);
        }
        public static bool ValidateUpdateCoworker(List<string> data)
        {
            var results = new List<ValidationResult>();
            if(data.Count != 6)
            {
                ValidationResult.Failure("Not all the required data are in the list.");
            }

            results.Add(OnlyNumbersValidator(data[0]));
            ValidateIfNotEmpty(data[1], results, OnlyTextValidator);
            ValidateIfNotEmpty(data[2], results, DateTimeValidator);
            ValidateIfNotEmpty(data[3], results, EmailValidator);
            ValidateIfNotEmpty(data[4], results, PositionValidator);
            results.Add(PasswordValidator(data[5]));

            return ReturnValidationResult(results);
        }
        public static bool ValidateUpdateProject(List<string> data)
        {
            var results = new List<ValidationResult>();
            if(data.Count != 6)
            {
                ValidationResult.Failure("Not all the required data are in the list.");
            }

            results.Add(OnlyNumbersValidator(data[0]));
            ValidateIfNotEmpty(data[1], results, NotEmptyFieldValidator);
            ValidateIfNotEmpty(data[2], results, DateTimeValidator);
            ValidateIfNotEmpty(data[3], results, DescriptionValidator);
            ValidateIfNotEmpty(data[4], results, PriorityValidator);
            ValidateIfNotEmpty(data[5], results, ManagerValidator);

            return ReturnValidationResult(results);
        }
        public static bool ValidateUpdateTask(List<string> data)
        {
            var results = new List<ValidationResult>();
            if (data.Count != 8)
            {
                ValidationResult.Failure("Not all the required data are in the list.");
            }

            results.Add(OnlyNumbersValidator(data[0]));
            ValidateIfNotEmpty(data[1], results, NotEmptyFieldValidator);
            ValidateIfNotEmpty(data[2], results, DateTimeValidator);
            ValidateIfNotEmpty(data[3], results, DescriptionValidator);
            ValidateIfNotEmpty(data[4], results, PriorityValidator);
            ValidateIfNotEmpty(data[5], results, ProjectValidator);
            ValidateIfNotEmpty(data[6], results, ManagerValidator);
            ValidateIfNotEmpty(data[7], results, CoworkerValidator);

            return ReturnValidationResult(results);
        }
        public static bool ValidateDeleteCoworker(List<string> data)
        {
            bool exists = true;
            Coworker coworker = null;
            string result = string.Join(" ", data);

            coworker = ValidateLogin(result);
            if (coworker == null)
            {
                ValidationResult.Failure("Coworker not found.");
                exists = false;
            }
            return exists;
        }
        public static bool ValidateDeleteProject(List<string> data)
        {
            string input = data[0].Trim('\'');
            int id = int.Parse(input);
            return Initializer.GetDbContext().Projects.Any(c => c.Id == id);

            //bool exists = true;
            //string input = data[0].Trim('\'');
            //int id = int.Parse(input);
            //exists = Initializer.GetProjectRepo().CheckIfProjectExists(id);
            //if(exists == false)
            //{
            //    ValidationResult.Failure("Project not found.");
            //    return false;
            //}
            //return exists;
        }
        public static bool ValidateDeleteTask(List<string> data)
        {
            string input = data[0].Trim('\'');
            int id = int.Parse(input);
            return Initializer.GetDbContext().Tasks.Any(c => c.Id == id);

            //bool exists = true;
            //string input = data[0].Trim('\'');
            //int id = int.Parse(input);
            //exists = Initializer.GetTaskRepo().CheckIfTaskExists(id);
            //if (exists == false)
            //{
            //    ValidationResult.Failure("Task not found.");
            //}
            //return exists;
        }
        public static bool ValidateRead(List<string> data)
        {
            FilterOptions filterOptions = new FilterOptions();
            var results = new List<ValidationResult>();
            foreach (string input in data)
            {
                if (!Enum.TryParse<FilterOptions>(input, ignoreCase: true, out filterOptions))
                {
                    ValidationResult.Failure("Filter option not found.");
                }
            }
            return ReturnValidationResult(results);
        }
        
    }
}
