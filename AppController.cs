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
        Validator programValidator = new Validator();
        public Category Category;
        public ChooseOperation Operation;

        public AppController()
        {
            Category = programValidator.CategoryValidator();
            Operation = programValidator.OperationValidator();

            switch (Operation)
            {
                case ChooseOperation.create:

                    if(Category == Category.tasks)
                    {
                        var taskOperations = new Operations<TaskItem>(KeeperOfData.Tasks);
                        var taskCreating = taskOperations.TaskCreate();
                        ProcessingTask processingTask = new(taskCreating);
                        var taskValidated = programValidator.TaskValidator<ProcessingTask>(processingTask);
                        KeeperOfData.Tasks.Add(processingTask.TransferToTaskItem(processingTask.Name, processingTask.DueDateFinal, processingTask.Description, processingTask.PriorityFinal, processingTask.ProjectFinal, processingTask.ManagerFinal, processingTask.EmployeeFinal));
                        AnsiConsole.MarkupLine($"[darkolivegreen1] Task {processingTask.Name} successfully added! [/]");
                    }
                    break;
                case ChooseOperation.read:

                    if(Category == Category.tasks)
                    {

                    }
                    break;
            }
        }
    }
}
