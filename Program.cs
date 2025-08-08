using Spectre.Console;

namespace TaskTracking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AnsiConsole.MarkupLine ("[palegreen1_1]Type the name of a category first (tasks, projects or coworkers):[/]");

            while (true)
            {
                AppController appController = new AppController();
            }
            
        }
    }
}
