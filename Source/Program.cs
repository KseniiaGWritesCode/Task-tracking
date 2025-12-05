using Spectre.Console;
using System.Globalization;
using System.Text.RegularExpressions;
using Npgsql;

namespace TaskTracking
{
    public class Program
    {
        const string APP_CONFIG_FILE = "config.json";
        private static NpgsqlConnection connection;

        static void Main(string[] args)
        {
            // Initialize
            try
            {
                Initializer.LoadConfig(APP_CONFIG_FILE);
                connection = Initializer.GetConnectionToDB();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[magenta1]Initialization error:[/] {ex.Message}");
            }







            Coworker? coworker = null;
            Commands? validCommand = null;
            Category? validCategory = null;
            List<string> data = new List<string>();
            string userInput = null;
            string readInput = null;
            try
            {
                //login:
                AnsiConsole.MarkupLine("[palegreen1_1]Hello! Type login to login.[/]");
                readInput = ReadingInput(userInput);

                string[] parsingLogin = readInput.Split();
                validCommand = Validator.ValidateCommand(parsingLogin[0].Trim());
                if (validCommand == null)
                {
                    AnsiConsole.MarkupLine("[magenta1]Wrong input![/]");
                    return;
                }

                if (validCommand == Commands.Login)
                {
                    AnsiConsole.MarkupLine("[palegreen1_1]Enter your e-mail and password:[/]");
                    readInput = ReadingInput(userInput);
                    coworker = Login(readInput);
                    if (coworker == null)
                    {
                        return;
                    }
                }

                //actions for user:
                while (true)
                {
                    AnsiConsole.MarkupLine("[palegreen1_1]Choose your action:[/]");
                    ShowListOfCommands();
                    readInput = ReadingInput(userInput);

                    string[] parsingInput = readInput.Split();
                    validCommand = Validator.ValidateCommand(parsingInput[0]);
                    if (validCommand == null)
                    {
                        AnsiConsole.MarkupLine("[magenta1]Wrong input! Probably something is missing or you've typed an extra 'field'.[/]");
                        continue;
                    }
                    
                    if (parsingInput.Length == 2)
                    {
                        validCategory = Validator.ValidateCategory(parsingInput[1].Trim());
                        if (validCategory == null)
                        {
                            AnsiConsole.MarkupLine("[magenta1]Wrong input of the second part![/]");
                            continue;
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[magenta1]Wrong input![/]");
                        continue;
                    }

                    bool checkAccess = Validator.ValidateAccess(validCategory, coworker);
                    if (!checkAccess)
                    {
                        AnsiConsole.MarkupLine("[magenta1]You don't have access to this action.[/]");
                        return;
                    }

                    //Showing the message of data to input:
                    if(validCommand != null && validCategory != null)
                    {
                        ActionsFromCombination.ShowMessageForMainInput(validCommand, validCategory);
                        readInput = ReadingInput(userInput);

                        var matches = Regex.Matches(readInput, "'(.*?)'");
                        data = matches.Select(m => m.Groups[1].Value.Trim()).ToList();
                        var validatedInput = ActionsFromCombination.TryValidate(validCommand, validCategory, data);
                        if (validatedInput == true)
                        {
                            //showing result for read (all and filtered):
                            if (validCommand == Commands.ReadAll)
                            {
                                Table tableAll = Operations.ShowAll(validCategory);
                                AnsiConsole.Write(tableAll);
                            }
                            
                            if (validCommand == Commands.Read)
                            {
                                var filterOptions = ActionsFromCombination.ToFilterOptions(data);
                                List<bool> veryfied = new List<bool>();
                                List<string> filterOptionsValues = new List<string>();
                                foreach (var filterOption in filterOptions)
                                {
                                    ActionsFromCombination.ShowMessagesForFilters(filterOption);
                                    readInput = ReadingInput(userInput);
                                    if (string.IsNullOrWhiteSpace(readInput))
                                    {
                                        AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
                                        continue;
                                    }
                                    filterOptionsValues.Add(readInput);
                                    var verifying = ActionsFromCombination.ValidatingFilterInput(filterOption);
                                    if (verifying)
                                    {
                                        veryfied.Add(verifying);
                                    }
                                }

                                if (veryfied.Count != data.Count)
                                {
                                    AnsiConsole.MarkupLine("[magenta1]Wrong input![/]");
                                    continue;
                                }

                                Table filteredData = new Table();
                                switch (validCategory)
                                {
                                    case Category.Coworkers:
                                        filteredData = Operations.CoworkerFiltered(filterOptions, filterOptionsValues);
                                        break;
                                    case Category.Projects:
                                        filteredData = Operations.ProjectsFiltered(filterOptions, filterOptionsValues);
                                        break;
                                    case Category.Tasks:
                                        filteredData = Operations.TasksFiltered(filterOptions, filterOptionsValues);
                                        break;
                                }
                                AnsiConsole.Write(filteredData);
                            }

                            //showing result for create or update or delete
                            switch (validCategory)
                            {
                                case Category.Coworker:
                                    var coworkerSuccess = Operations.CoworkerDataToDB(data);
                                    if (coworkerSuccess)
                                    {
                                        AnsiConsole.MarkupLine("[bold yellow]Success![/]");
                                    }
                                    break;
                                case Category.Project:
                                    var projectSuccess = Operations.ProjectDataToDB(data);
                                    if (projectSuccess)
                                    {
                                        AnsiConsole.MarkupLine("[bold yellow]Success![/]");
                                    }
                                    break;
                                case Category.Task:
                                    var categorySuccess = Operations.TaskDataToDB(data);
                                    if (categorySuccess)
                                    {
                                        AnsiConsole.MarkupLine("[bold yellow]Success![/]");
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[magenta1]Wrong input![/]");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[magenta1]Exception caught![/] {ex.Message}");
            }
        }
        private static Table ListOfCommands()
        {
            var table = new Table();
            table.AddColumn("[bold yellow]Command[/]");
            table.AddColumn("[bold yellow]Description[/]");
            table.AddRow("[lightcyan1]Create coworker[/]", "to create a new employee profile in system");
            table.AddRow("[lightcyan1]Create project[/]", "to create a new project");
            table.AddRow("[lightcyan1]Create task[/]", "to form a new task inside of a project");
            table.AddRow("[lightcyan1]Update coworker[/]", "to change information about an existing employee");
            table.AddRow("[lightcyan1]Update project[/]", "to change information about an existing project");
            table.AddRow("[lightcyan1]Update task[/]", "to change information about an existing task");
            table.AddRow("[lightcyan1]Delete coworker[/]", "to delete a profile on an employee from the system");
            table.AddRow("[lightcyan1]Delete project[/]", "to delete a project");
            table.AddRow("[lightcyan1]Delete task[/]", "to delete a task");
            table.AddRow("[lightcyan1]ReadAll coworkers[/]", "to show all employee profiles");
            table.AddRow("[lightcyan1]ReadAll projects[/]", "to show all projects");
            table.AddRow("[lightcyan1]ReadAll tasks[/]", "to show all tasks");
            table.AddRow("[lightcyan1]Read coworkers[/]", "to show filtered employee profiles (options come next)");
            table.AddRow("[lightcyan1]Read projects[/]", "to show filtered projects (options come next)");
            table.AddRow("[lightcyan1]Read tasks[/]", "to show filtered tasks (options come next)");

            return table;
        }
        private static void ShowListOfCommands()
        {
            AnsiConsole.Write(ListOfCommands());
        }
        private static Coworker? Login(string input)
        {
            var coworker = Validator.ValidateLogin(input);
            if (coworker == null)
            {
                AnsiConsole.MarkupLine("[magenta1]Please check your email and password carefully and try again.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[lightcyan1]Welcome back,{coworker.Name}![/]");
            }
            return coworker;
        }
        private static string ReadingInput(string input)
        {
            input = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
            }
            return input;
        }
    }
}
//'Jonh Smith The Great Guru' '05.05.1555' 'thewise@ofalltimes.it' 'admin' 'the4GrEE^Atest9'     thewise@ofalltimes.it the4GrEE^Atest9
//'Sarah Connor' '10.01.1962' 'sconnor@rules.com' 'manager' 'Te6RRm!%natOR'                      sconnor@rules.com Te6RRm!%natOR
//'Arnold Schwarzenegger' '20.11.1960' 'illbeback@it.com' 'manager' 'F77&hdg_lnlk6'
//'John Connor' '05.08.1985' 'jconnor@rules.com' 'developer' 'kbhT6%8FB!_bhj'
//'Jessica Parker' '08.09.1975' 'themoos@southpark.com' 'developer' 'JP67&vgyv_kbY'
//'Tray Parker' '18.10.1978' 'trayp@sp.com' 'designer' 'jnjH77hG^j'
//'Matt Stone' '24.07.1980' 'matt@stone.com' 'designer' 'kh7^bjh8GT'

//'Personal website' '02.02.2026' 'Create a badass cool website' 'medium' 'illbeback@it.com'

//'testing user' '01.01.2001' 'testing@test.it' 'tesTinG6#!9' 'developer' 
//'test' '01.01.2026' 'iihmihih oi,hohoo' 'low' '11'
//