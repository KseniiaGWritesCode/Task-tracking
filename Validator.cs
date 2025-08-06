using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class Validator
    {
        public void CategoryValidator () 
        {
            while (true)
            {
                string categoryUserInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(categoryUserInput))
                {
                    AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
                    continue;
                }

                if (!Enum.TryParse<Category>(categoryUserInput.Trim().ToLower(), out var category))
                {
                    AnsiConsole.MarkupLine("[magenta1]Category doesn't exhist![/]");
                    continue;
                }

                var keeper = new KeeperOfData();
                switch (category)
                {
                    case Category.tasks:
                        var catTasks = new Operations<Task>(keeper.Tasks);
                        break;
                    case Category.projects:
                        var catProjects = new Operations<Project>(keeper.Projects);
                        break;
                    case Category.coworkers:
                        var catCoworkers = new Operations<Coworker>(keeper.Coworkers);
                        break;
                }
                break;
            }
        }
        public void OperationValidator()
        {
            while (true)
            {
                string taskUserInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(taskUserInput))
                {
                    AnsiConsole.MarkupLine("[magenta1]Empty input![/]");
                    continue;
                }

                if (!Enum.TryParse<Operation>(taskUserInput.Trim().ToLower(), out var operation))
                {
                    AnsiConsole.MarkupLine("[magenta1]Category doesn't exhist![/]");
                    continue;
                }

                switch(operation)
                {
                    case Operation.create:
                        
                        break;
                }
            }
        }
    }
    public enum Category
    {
        tasks,
        projects,
        coworkers
    }

    public enum Operation
    {
        create,
        read,
        update,
        delete
    }
}
