using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public static class ActionsFromCombination
    {
        private static readonly Dictionary<(Commands?, Category?), Action> _mainMessages = new()
        {
            { (Commands.Create, Category.Coworker), () => AnsiConsole.MarkupLine("[palegreen1_1]Enter the coworkers data: 'name' 'birthday' 'e-mail' 'position' 'password':[/]") },
            { (Commands.Create, Category.Project), () => AnsiConsole.MarkupLine("[palegreen1_1]Enter the project data: 'name' 'due date' 'description' 'priority' 'managers id':[/]") },
            { (Commands.Create, Category.Task), () => AnsiConsole.MarkupLine("[palegreen1_1]Enter the task data: 'name' 'due date' 'description' 'priority' 'projects id' 'managers id' 'performers id':[/]") },
            { (Commands.Update, Category.Coworker), () => AnsiConsole.MarkupLine("[palegreen1_1]Enter the coworkers data: 'id'(mandatory) 'name' 'birthday' 'e-mail' 'position' 'password'(mandatory):[/]") },
            { (Commands.Update, Category.Project), () => AnsiConsole.MarkupLine("[palegreen1_1]Enter the project data: 'id'(mandatory) 'name' 'due date' 'description' 'priority' 'managers id':[/]") },
            { (Commands.Update, Category.Task), () => AnsiConsole.MarkupLine("[palegreen1_1]Enter the task data: 'id'(mandatory) 'name' 'due date' 'description' 'priority' 'projects id' 'managers id' 'performers id':[/]") },
            { (Commands.Delete, Category.Coworker), () => AnsiConsole.MarkupLine("[palegreen1_1]Enter the coworkers 'e-mail' 'password':[/]") },
            { (Commands.Delete, Category.Project), () => AnsiConsole.MarkupLine("[palegreen1_1]Enter the project 'id':[/]") },
            { (Commands.Delete, Category.Task), () => AnsiConsole.MarkupLine("[palegreen1_1]Enter the task 'id':[/]") },
            { (Commands.ReadAll, Category.Coworkers), () =>  AnsiConsole.MarkupLine("[palegreen1_1]Type any 'symbol' to continue[/]") },
            { (Commands.ReadAll, Category.Projects), () =>  AnsiConsole.MarkupLine("[palegreen1_1]Type any 'symbol' to continue[/]") },
            { (Commands.ReadAll, Category.Tasks), () =>  AnsiConsole.MarkupLine("[palegreen1_1]Type any 'symbol' to continue[/]") },
            { (Commands.Read, Category.Coworkers), () =>  AnsiConsole.MarkupLine("[palegreen1_1]Filter options: 'Project', 'Position':[/]") },
            { (Commands.Read, Category.Projects), () =>  AnsiConsole.MarkupLine("[palegreen1_1]Filter options: 'Coworker', 'Priority':[/]") },
            { (Commands.Read, Category.Tasks), () =>  AnsiConsole.MarkupLine("[palegreen1_1]Filter options: 'Coworker', 'Priority', 'Project':[/]") }
        };
        public static void ShowMessageForMainInput(Commands? command, Category? category, Action<string>? onError = null)
        {
            try
            {
                if (_mainMessages.TryGetValue((command, category), out var message))
                {
                    message.Invoke();
                }
                else
                {
                    onError.Invoke("This combination isn't processed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static readonly Dictionary<FilterOptions, Action> _filterInputMessages = new()
        {
            { FilterOptions.Project, () => AnsiConsole.MarkupLine("[mediumpurple1]Type Project ID:[/]")},
            { FilterOptions.Position, () => AnsiConsole.MarkupLine("[mediumpurple1]Type Position:[/]")},
            { FilterOptions.Coworker, () => AnsiConsole.MarkupLine("[mediumpurple1]Type Coworkers ID:[/]")},
            { FilterOptions.Priority, () => AnsiConsole.MarkupLine("[mediumpurple1]Type Priority:[/]")}
        };

        public static void ShowMessagesForFilters(FilterOptions filterOptions, Action<string>? onError = null)
        {
            try
            {
                if (_filterInputMessages.TryGetValue(filterOptions, out var message))
                {
                    message.Invoke();
                }
                else
                {
                    onError.Invoke("This combination isn't processed.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        //validators:
        private static readonly Dictionary<(Commands?, Category?), Func<List<string>, bool>> _mainValidators = new()
        {
            { (Commands.Create, Category.Coworker), Validator.ValidateNewCoworkerData },
            { (Commands.Create, Category.Project), Validator.ValidateNewProjectData },
            { (Commands.Create, Category.Task), Validator.ValidateNewTaskData },
            { (Commands.Update, Category.Coworker), Validator.ValidateUpdateCoworker },
            { (Commands.Update, Category.Project), Validator.ValidateUpdateProject},
            { (Commands.Update, Category.Task), Validator.ValidateUpdateTask },
            { (Commands.Delete, Category.Coworker), Validator.ValidateDeleteCoworker },
            { (Commands.Delete, Category.Project), Validator.ValidateDeleteProject },
            { (Commands.Delete, Category.Task), Validator.ValidateDeleteTask },
            { (Commands.ReadAll, Category.Coworkers), _ => true},
            { (Commands.ReadAll, Category.Projects), _ => true},
            { (Commands.ReadAll, Category.Tasks), _ => true},
            { (Commands.Read, Category.Coworkers), Validator.ValidateRead},
            { (Commands.Read, Category.Projects), Validator.ValidateRead},
            { (Commands.Read, Category.Tasks), Validator.ValidateRead}
        };

        public static bool TryValidate(Commands? command, Category? category, List<string> data)
        {
            if (_mainValidators.TryGetValue((command, category), out var validator))
            {
                return validator(data);
            }
            return false;
        }

        private static readonly Dictionary<FilterOptions, Func<string, ValidationResult>> _readValidators = new()
        {
            { FilterOptions.Project, Validator.ProjectValidator},
            { FilterOptions.Position, Validator.PositionValidator},
            { FilterOptions.Coworker, Validator.CoworkerValidator},
            { FilterOptions.Priority, Validator.PriorityValidator}
        };

        public static bool ValidatingFilterInput(FilterOptions filterOption)
        {
            if(_readValidators.TryGetValue(filterOption, out var validator))
            {
                return true;
            }
            return false;
        }

        public static List<FilterOptions> ToFilterOptions(List<string> data)
        {
            List<FilterOptions> filters = new List<FilterOptions>();
            FilterOptions filterOptions = new FilterOptions();
            foreach (var item in data)
            {
                Enum.TryParse<FilterOptions>(item, ignoreCase: true, out filterOptions);
                filters.Add(filterOptions);
            }
            return filters;
        }
    }
}
