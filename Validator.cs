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
        KeeperOfData keeper = new KeeperOfData();
        public bool CategoryValidator () 
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

                switch (category)
                {
                    case Category.tasks:
                        OperationValidator(new Operations<Task>(keeper.Tasks));
                        break;
                    case Category.projects:
                        OperationValidator(new Operations<Project>(keeper.Projects));
                        break;
                    case Category.coworkers:
                        OperationValidator(new Operations<Coworker>(keeper.Coworkers));
                        break;
                }
                break;
            }
            return true;
        }
        public void OperationValidator<T> (Operations<T> ops)
        {
            AnsiConsole.MarkupLine("[lightcyan1]What do you want to do - create, read, update or delete?[/]");
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
                    AnsiConsole.MarkupLine("[magenta1]Operation doesn't exhist![/]");
                    continue;
                }

                switch(operation)
                {
                    case Operation.create:
                        ops.Create(Category.tasks);
                        break;
                }
            }
        }
    }

    public enum Operation
    {
        create,
        read,
        update,
        delete
    }
}
