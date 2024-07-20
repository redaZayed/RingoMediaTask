using Core.Models;
using Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace RingoMediaTask.Controllers
{
    public class RemindersController : Controller
    {
        //private readonly IReminderServices _ReminderServices;
        // لو فضلنا العمل من خلاله //if we need direct access to unity of work 
        private readonly IUnitOfWork _context;

        public RemindersController( IUnitOfWork unitOfWork)
        {
            //_ReminderServices = ReminderServices;
            _context = unitOfWork;
        }

        // GET: Reminders
        public async Task<IActionResult> Index()
        {
            return View(  _context.Reminders.GetAll());
        }

        // GET: Reminders/Create
        public IActionResult Create()
        {
            return PartialView("_ReminderForm", new Reminder());
        }

        // POST: Reminders/Create
        [HttpPost]
        public async Task<IActionResult> Create(Reminder reminder)
        {
            if (ModelState.IsValid)
            {
                _context.Reminders.Add(reminder);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_ReminderForm", reminder);
        }

        // GET: Reminders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var reminder =   _context.Reminders.GetById(id);
            if (reminder == null)
            {
                return NotFound();
            }
            return PartialView("_ReminderForm", reminder);
        }

        // POST: Reminders/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Reminder reminder)
        {
            if (id != reminder.ReminderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Reminders.Update(reminder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReminderExists(reminder.ReminderId))
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
            return PartialView("_ReminderForm", reminder);
        }

        // POST: Reminders/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var reminder =   _context.Reminders.GetById(id);
            if (reminder != null)
            {
                _context.Reminders.Delete(reminder.ReminderId);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        private bool ReminderExists(int id)
        {
            return _context.Reminders.GetById(id) != null;
        }
    }
}
 
    


