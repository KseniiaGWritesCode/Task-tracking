using Spectre.Console;
using System.Globalization;

namespace TaskTracking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                AnsiConsole.MarkupLine("[palegreen1_1]Type the name of a category first (tasks, projects or coworkers):[/]");
                AppController appController = new AppController();
            }
            
        }
    }
}
