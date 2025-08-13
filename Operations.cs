using Microsoft.VisualBasic;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public T Create<T> (T item) where T : new()
        {
            item = new T();

            switch (item)
            {
                case Category.tasks:
                    TaskCreate();
                    break;
            }
            return item;
        }

        public void TaskCreate()
        {
            AnsiConsole.MarkupLine("[lightcyan1]Please enter the following parameters of the new task, each supplemented with '':[/]");
            AnsiConsole.MarkupLine("[darkolivegreen1] 'name' 'dd.mm.yyyy (due date)' 'description' 'priority (low, medium or high)' 'project' 'manager' 'employee'[/]");
            
            string taskInput = Console.ReadLine();
            List<string> tasks = new List<string>();

            try
            {
                var matches = Regex.Matches(taskInput, "'([^']*)'|(\\S+)");
                foreach (Match match in matches)
                {
                    tasks.Add(match.Value);
                }

                FormingNewItem<ProcessingTask>(tasks);
            }
            
            catch(Exception ex)
            {
                AnsiConsole.MarkupLine("[magenta1]Something went wrong![/]");
                return;
            }
        }

        public T FormingNewItem<T> (List<string> newItem) where T : new()
        {
            T item = new T();
            switch(item)
            {
                case Category.tasks:
                    ProcessingTask processingTask = new ProcessingTask();

                    for (int i = 0; i < newItem.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                processingTask.Name = newItem[i];
                                break;
                            case 1:
                                processingTask.DueDate = newItem[i];
                                break;
                            case 2:
                                processingTask.Description = newItem[i];
                                break;
                            case 3:
                                processingTask.Priority = newItem[i];
                                break;
                            case 4:
                                processingTask.Project = newItem[i];
                                break;
                            case 5:
                                processingTask.Manager = newItem[i];
                                break;
                            case 6:
                                processingTask.Employee = newItem[i];
                                break;
                        }
                    }
                    break;
            }
            
            return item;
        }
    }
}
