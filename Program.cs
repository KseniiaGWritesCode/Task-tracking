using Spectre.Console;
using System.Globalization;

namespace TaskTracking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            KeeperOfData keeperOfData = new KeeperOfData();

            keeperOfData.Coworkers.Add(new Coworker("John Smith", DateTime.ParseExact("23.04.1985", "dd.MM.yyyy", CultureInfo.InvariantCulture), "johnsmith@company.com", Position.Manager));
            keeperOfData.Coworkers.Add(new Coworker("Jane Doe", DateTime.ParseExact("04.05.1995", "dd.MM.yyyy", CultureInfo.InvariantCulture), "janedoe@company.com", Position.Manager));
            keeperOfData.Coworkers.Add(new Coworker("Tom Armstrong", DateTime.ParseExact("15.11.1981", "dd.MM.yyyy", CultureInfo.InvariantCulture), "tomarmstrong@company.com", Position.Developer));
            keeperOfData.Coworkers.Add(new Coworker("Matt Daemond", DateTime.ParseExact("30.07.1989", "dd.MM.yyyy", CultureInfo.InvariantCulture), "mattdaemond@company.com", Position.Developer));
            keeperOfData.Coworkers.Add(new Coworker("Mary Cole", DateTime.ParseExact("11.01.1992", "dd.MM.yyyy", CultureInfo.InvariantCulture), "marycole@company.com", Position.Designer));
            keeperOfData.Coworkers.Add(new Coworker("Sarah Jessica Parker", DateTime.ParseExact("02.04.1987", "dd.MM.yyyy", CultureInfo.InvariantCulture), "sarahjessicaparker@company.com", Position.Designer));

            var manager1 = keeperOfData.Coworkers.First(c => c.Name == "John Smith");
            var manager2 = keeperOfData.Coworkers.First(c => c.Name == "Jane Doe");

            var coworker1 = keeperOfData.Coworkers.First(c => c.Name == "Mary Cole");
            var coworker2 = keeperOfData.Coworkers.First(c => c.Name == "Tom Armstrong");
            var coworker3 = keeperOfData.Coworkers.First(c => c.Name == "Sarah Jessica Parker");
            var coworker4 = keeperOfData.Coworkers.First(c => c.Name == "Matt Daemond");

            keeperOfData.Projects.Add(new Project("Inhabited Mind", DateTime.ParseExact("01.02.2026", "dd.MM.yyyy", CultureInfo.InvariantCulture), "Gamifikation of the habit to envoke positiv thoughts. User is playing the role of a scientist on another planet, and must collect points by positive thoughts to communicate with the alien animals.", Priority.High, manager1));
            keeperOfData.Projects.Add(new Project("Zweite Liebe", DateTime.ParseExact("01.08.2026", "dd.MM.yyyy", CultureInfo.InvariantCulture), "Web-shop for a young german startup, which collects and re-sells premiup second-hand fashion items.", Priority.Low, manager2));

            var project1 = keeperOfData.Projects.First(p => p.Name == "Inhabited Mind");
            var project2 = keeperOfData.Projects.First(p => p.Name == "Zweite Liebe");

            keeperOfData.Tasks.Add(new TaskItem ("UX Research", DateTime.ParseExact("01.09.2025", "dd.MM.yyyy", CultureInfo.InvariantCulture), "Make a thoroughful UX-Research for the Inhabited Mind, including user and competitors research", Priority.High, project1, manager1, coworker1));
            keeperOfData.Tasks.Add(new TaskItem("App Back-end", DateTime.ParseExact("01.12.2025", "dd.MM.yyyy", CultureInfo.InvariantCulture), "Create all back-end for the Inhabited Mind Web App", Priority.Medium, project1, manager1, coworker2));
            keeperOfData.Tasks.Add(new TaskItem("Wireframes", DateTime.ParseExact("20.08.2025", "dd.MM.yyyy", CultureInfo.InvariantCulture), "Create wireframes for the Zweite Liebe project, using the prepared Info-architecture", Priority.High, project2, manager2, coworker3));
            keeperOfData.Tasks.Add(new TaskItem("Frontend for the main page", DateTime.ParseExact("20.10.2025", "dd.MM.yyyy", CultureInfo.InvariantCulture), "Make frontend for the start page of the web-shop", Priority.Medium, project2, manager2, coworker4));
            
            AnsiConsole.MarkupLine ("[palegreen1_1]Type the name of a category first (tasks, projects or coworkers):[/]");

            while (true)
            {
                AppController appController = new AppController();
            }
            
        }
    }
}
