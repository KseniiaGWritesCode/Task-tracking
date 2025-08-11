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
            bool categoryValidated = programValidator.CategoryValidator();
            bool goingInTasks = programValidator.OperationValidator(new Operations<TaskItem>(keeper.Tasks));
            bool goingInProjects = programValidator.OperationValidator(new Operations<Project>(keeper.Projects));
            bool goingInCoworkers = programValidator.OperationValidator(new Operations<Coworker>(keeper.Coworkers));

            if (categoryValidated)
            {
                switch(Category)
                {
                    case Category.tasks:
                        programValidator.OperationValidator(new Operations<TaskItem>(keeper.Tasks));
                        break;
                    case Category.projects:
                        programValidator.OperationValidator(new Operations<Project>(keeper.Projects));
                        break;
                    case Category.coworkers:
                        programValidator.OperationValidator(new Operations<Coworker>(keeper.Coworkers));
                        break;
                }
            }

            if (goingInTasks)
            {
                switch (Operation)
                {
                    case ChooseOperation.create:
                        new Operations<TaskItem>(keeper.Tasks).Create(Category.tasks);

                        break;
                }
            }
        }
    }
}
