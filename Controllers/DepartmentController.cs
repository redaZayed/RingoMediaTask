using Core.Models;
using Core.Common.Request_Model;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Core.Common;
using Core.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RingoMediaTask.Pages.Controllers
{
    public class DepartmentController : Controller
    {
        // if we need to use services module as  API back end interaction
        private readonly IDepartmentServices _departmentServices; 
         // لو فضلنا العمل من خلاله //if we need direct access to unity of work 
        private readonly IUnitOfWork _UnitOfWork;
        public DepartmentController(IDepartmentServices departmentServices, IUnitOfWork unitOfWork)
        {
            _departmentServices = departmentServices;
            _UnitOfWork = unitOfWork;
        }
        // GET: DepartmentController
        public ActionResult Index(SearchDepartment request)
        {
            List<Department>? departmentList = new List<Department>();
            Response_ViewModel<List<Department>> departmentsQuery = _departmentServices.GetDepartments(request);
            if (departmentsQuery != null)
                departmentList = departmentsQuery.Result!.Data;

            ViewBag.Departments = new SelectList(_UnitOfWork.Departments.GetAll(), "DepartmentId", "DepartmentName");

            return View(departmentList);
            
            //OR we can use the following commented Lines
            //var Departs = _UnitOfWork.Departments.GetAll();
            //return View(Departs);
        }
        [HttpPost]
        public IActionResult Search(SearchDepartment searchModel)
        {
            // Populate the ParentDepartments dropdown
             
            ViewBag.Departments = new SelectList(_UnitOfWork.Departments.GetAll(), "DepartmentId", "DepartmentName");

            // Search logic
            var departments = _UnitOfWork.Departments.GetAll();

            if (!string.IsNullOrEmpty(searchModel.SearchText))
            {
                departments = departments.Where(d => d.DepartmentName.Contains(searchModel.SearchText));
            }

            if (searchModel.DepartmentId.HasValue)
            {
                departments = departments.Where(d => d.ParentDepartmentId == searchModel.DepartmentId);
            }

            // Pass search results to view
            var model = new SearchDepartment
            {
                Departments = departments.ToList() 
            };

            return View("Index", departments);
        }
        // GET: DepartmentController/Details/5
        public ActionResult Details(int id)
        {
            Department? department = new Department();
            Response_ViewModel<Department> departmentsQuery = _departmentServices.GetDepartmentById(id);
            if (departmentsQuery != null)
                department = departmentsQuery.Result!.Data;
            return View(department);
        }

        // GET: DepartmentController/Create
        public ActionResult Create()
        {
            ViewBag.Departments = new SelectList(_UnitOfWork.Departments.GetAll(), "DepartmentId", "DepartmentName");
            return PartialView("_DepartmentForm", new Department());
        }

        // POST: DepartmentController/Create
        [HttpPost]
        
        public async Task<ActionResult> Create(Department department)
        {
            try
            {
               

                //If we will use the services module  لو اشتغلنا من خلال مديول الخدمات 
                if (ModelState.IsValid)
                {
                    _UnitOfWork.Departments.Add(department);
                    await _UnitOfWork.SaveChangesAsync();
                    return Json(new { success = true });

                    // OR Use This commented Code
                    //Response_ViewModel<Department> departmentsQuery = _departmentServices.GetDepartmentById(id);
                    //if (departmentsQuery != null)
                    //    department = departmentsQuery.Result!.Data;
                    //  return Ok();
                    //
                }
            }
            catch (Exception ex)
            {
                //log ex
            }
            ViewBag.Departments = new SelectList(_UnitOfWork.Departments.GetAll(), "DepartmentId", "DepartmentName", department.ParentDepartmentId);
            return PartialView("_DepartmentForm", department);

        }

        // GET: DepartmentController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var department=  _UnitOfWork.Departments.GetById(id);
            if (department == null)
                return NotFound();
            ViewBag.Departments = new SelectList(_UnitOfWork.Departments.GetAll(), "DepartmentId", "DepartmentName", department.ParentDepartmentId);
            return PartialView("_DepartmentForm", department);
        }

        // POST: DepartmentController/Edit/5
        [HttpPost]
  
        public async Task<ActionResult> Edit(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _UnitOfWork.Departments.Update(department);
                    await _UnitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(new { success = true });
            }
            ViewBag.Departments = new SelectList(_UnitOfWork.Departments.GetAll(), "Id", "Name", department.ParentDepartmentId);
            return PartialView("_DepartmentForm", department);
             
        }
        
        // POST: DepartmentController/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var department =   _UnitOfWork.Departments.GetById(id);
                if (department != null)
                {
                    _UnitOfWork.Departments.Delete(id);
                    await _UnitOfWork.SaveChangesAsync();
                }
                return Json(new { success = true });
            }
            catch
            {
                return BadRequest();
            }
        }
        private bool DepartmentExist(int departmentId)
        {
            return (_UnitOfWork.Departments.GetById(departmentId) != null);
        }
        private bool DepartmentExists(int id)
        {
            return _UnitOfWork.Departments.GetAll().Any(e => e.DepartmentId == id);
        }

        [HttpGet]
        public async Task<IActionResult> IsDepartmentNameUnique(string name)
        {
            var exists =   _UnitOfWork.Departments
                .GetAll(d => d.DepartmentName.Equals(name, StringComparison.OrdinalIgnoreCase)).Any();

            return Json(!exists);
        }
    }

}
