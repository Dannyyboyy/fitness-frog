using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today,
            };

            SetupActivitiesSelectListItems();

            return View(entry);
        }
        // this is an attribute. Which allows us to name our method. Its a method request to help us GET AND Retrive the post
        [HttpPost]
        // below are method paramaters to help us specifity each form field value we need to capture.
        public ActionResult Add(Entry entry)
        {
            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                return RedirectToAction("Index");
            }

            SetupActivitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Get the requested entry from the repository.
            Entry entry = _entriesRepository.GetEntry((int)id);

            // TODO Return a status of Not Found if the entry wasn't found.
            if (entry == null)
            {
                return HttpNotFound();
            }

            SetupActivitiesSelectListItems();

            // TODO Pass the entry into the view

            return View(entry);
        }
        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            ValidateEntry(entry);


            // TODO Validate the entry
            // If the entry is valid...

            // 1) Use the repositroy to update the entry
            // 2) Redirect the user to the "Entries" list page
            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);

                return RedirectToAction("Index");
            }
            // 3) TODO Populate the activities select list items to the ViewBag property
            SetupActivitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // TODO Retrieve entry for the prodvided if parameter value
            Entry entry = _entriesRepository.GetEntry((int)id);

            // TODO Return "not found" if entry wasn't found
            if(entry == null)
            {
                return HttpNotFound();
            }

            // TODO Pass the entey to the view
            return View(entry);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            // TODO Delete the entry
            _entriesRepository.DeleteEntry(id);

            // TODO Redirect to the entries list page
            return RedirectToAction("Index");
        }

        private void ValidateEntry(Entry entry)
        {
            // if there aren't any duration field validation errors. 
            //Then make sure the duration is greater than 0.
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater than '0'.");
            }
        }

        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(
                        Data.Data.Activities, "Id", "Name");
        }
    }
}