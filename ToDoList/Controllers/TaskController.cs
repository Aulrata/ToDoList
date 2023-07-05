using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ToDoList.Controllers
{
    public class TaskController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }


    }
}