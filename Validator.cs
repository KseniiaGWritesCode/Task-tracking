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
        public bool CategoryValidator() 
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
                break;
            }
            return true;
        }

        public bool OperationValidator<T> (Operations<T> ops)
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

                if (!Enum.TryParse<ChooseOperation>(taskUserInput.Trim().ToLower(), out var operation))
                {
                    AnsiConsole.MarkupLine("[magenta1]Operation doesn't exhist![/]");
                    continue;
                }
            }
        }

        public bool TaskValidator<T> (Operations<T> ops)
        {
            ProcessingTask processingTask = new ProcessingTask();
            KeeperOfData keeperOfData = new KeeperOfData();

            if (!string.IsNullOrWhiteSpace(processingTask.Name))
            {
                for (int i = 0; i <= keeperOfData.Coworkers.Count; i++)
                {

                }
            }
            return true;
        }
    }
}
