using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class AppController
    {
        KeeperOfData keeper = new KeeperOfData();
        Validator programValidator = new Validator();
        public Category Category;
        public ChooseOperation Operation;

        public AppController()
        {
            //bool categoryValidated = programValidator.CategoryValidator();
            var categoryValidated = programValidator.CategoryValidator();

            bool goingInTasks = programValidator.OperationValidator(new Operations<TaskItem>(keeper.Tasks));
            bool goingInProjects = programValidator.OperationValidator(new Operations<Project>(keeper.Projects));
            bool goingInCoworkers = programValidator.OperationValidator(new Operations<Coworker>(keeper.Coworkers));

            if (categoryValidated == Category)
            {
                switch(Category)
                {
                    case Category.tasks:
                        goingInTasks = true;
                        goingInProjects = false;
                        goingInCoworkers = false;
                        break;
                    case Category.projects:
                        goingInProjects = true;
                        goingInTasks = false;
                        goingInCoworkers = false;
                        break;
                    case Category.coworkers:
                        goingInCoworkers = true;
                        goingInProjects = false;
                        goingInTasks = false;
                        break;
                }
            }

            if (goingInTasks)
            {
                switch (Operation)
                {
                    case ChooseOperation.create:
                        var taskOperations = new Operations<TaskItem>(keeper.Tasks);
                        ProcessingTask processingTask = new();
                        var newTask = taskOperations.Create(processingTask);
                        var taskValidated = programValidator.TaskValidator<ProcessingTask>(newTask);
                        keeper.Tasks.Add(processingTask.TransferToTaskItem(processingTask.Name, processingTask.DueDateFinal, processingTask.Description, processingTask.PriorityFinal, processingTask.ProjectFinal, processingTask.ManagerFinal, processingTask.EmployeeFinal));
                        AnsiConsole.MarkupLine($"[darkolivegreen1] Task {processingTask.Name} successfully added! [/]");
                        break;
                }
            }
        }
    }
}
