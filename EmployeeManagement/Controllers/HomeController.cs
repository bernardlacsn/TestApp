using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeerepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IEmployeeRepository employeerepository,
                                IHostingEnvironment hostingEnvironment)
        {
            _employeerepository = employeerepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = _employeerepository.GetAllEmployee();
            return View(model);

        }

        public IActionResult Details(int? Id)
        {
            throw new Exception("Error in DetailsView");

            Employee employee = _employeerepository.GetEmployee(Id.Value);
            if (employee == null) {

                Response.StatusCode = 404;
                return View("EmployeeNotFound", Id.Value);  
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //to save employee
        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                //string uniqueFileName = null;
                string uniqueFileName = ProcessUploadedFile(model);
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email=model.Email,
                    Department= model.Department,
                    PhotoPath=uniqueFileName
                };

                _employeerepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
            
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Employee employee =_employeerepository.GetEmployee(Id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeerepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if (model.Photo != null) {

                    if (model.ExistingPhotoPath != null)
                    {
                      string getExistingPhoto= Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(getExistingPhoto);
                    }
                    employee.PhotoPath  = ProcessUploadedFile(model);
                } 
                
                _employeerepository.Update(employee);
                return RedirectToAction("Index");
            }
            return View();

        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }

                   
            }

            return uniqueFileName;
        }
    }
}
