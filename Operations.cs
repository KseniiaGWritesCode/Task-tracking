using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class Operations<T>
    {
        KeeperOfData listOfTasks = new KeeperOfData();
        KeeperOfData listOfProjects = new KeeperOfData();
        KeeperOfData listOfCoworkers = new KeeperOfData();

        List<T> list = new();
        public Operations(List<T> list) 
        { 
           
        }
        public List<T> See(T item)
        {
            return list;
        }

        public void Create<T> (T item)
        {
            switch(item)
            {
                case Category.tasks:
                    TaskCreate();
                    break;
            }
        }

        public void TaskCreate()
        {
            AnsiConsole.MarkupLine("[lightcyan1]Please enter the following parameters of the new task, each supplemented with '':[/]");
            AnsiConsole.MarkupLine("[darkolivegreen1] 'name' 'due date: dd.mm.yyyy' 'description' 'priority: low, medium or high' 'project' 'manager' 'employee'[/]");
            
            string taskInput = Console.ReadLine();

            //listOfTasks.Tasks.Add(new Task());
        }
    }
}
