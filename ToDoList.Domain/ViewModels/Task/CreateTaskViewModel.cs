using ToDoList.Domain.Enum;

namespace ToDoList.Domain.ViewModels.Task
{
    public class CreateTaskViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public void Validate()
        {
            if(string.IsNullOrEmpty(Name))
                throw new ArgumentNullException(Name, "Укажите название задачи");
            if (string.IsNullOrEmpty(Description)) 
                throw new ArgumentNullException(Description, "Укадите описание задачи");
        }

    }
}
