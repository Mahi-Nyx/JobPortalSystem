using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPortalSystem.Models;
using System.Data.Entity;
using JobPortalSystem.Data;

namespace JobPortalSystem.Controllers
{
    public class EmployerController : Controller
    {
        // GET: Employer
        public ActionResult EmployDashboard()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Employer")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public ActionResult PostJob()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Employer")
            {
                return RedirectToAction("Index", "Home");
            }
            var job = new Job
            {
                Deadline = DateTime.Today.AddDays(30)
            };

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostJob(Job model)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Employer")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                using (var db = new JobPortalSystem.Data.JobPortalDbContext())
                {
                    model.UserID = Convert.ToInt32(Session["UserID"]);
                    model.CreatedDate = DateTime.Now;

                    db.Jobs.Add(model);
                    db.SaveChanges();
                }

                return RedirectToAction("MyJobs");
            }

            return View(model);
        }

        public ActionResult MyJobs()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Employer")
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            using (var db = new JobPortalSystem.Data.JobPortalDbContext())
            {
                var jobs = db.Jobs
                    .Where(j => j.UserID == userId)
                    .OrderByDescending(j => j.CreatedDate)
                    .ToList();

                return View(jobs);
            }
        }

        public ActionResult EditJob(int id)
        {
            using (var db = new JobPortalDbContext())
            {
                var job = db.Jobs.Find(id);

                if (job == null)
                {
                    // Fallback safety redirect
                    return RedirectToAction("MyJobs");
                }

                return View(job);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJob(Job model)
        {
            using (var db = new JobPortalDbContext())
            {
                var job = db.Jobs.Find(model.JobID);

                if (job != null)
                {
                    job.JobTitle = model.JobTitle;
                    job.CompanyName = model.CompanyName;
                    job.Description = model.Description;
                    job.Location = model.Location;
                    job.Salary = model.Salary;
                    job.Deadline = model.Deadline;

                    db.SaveChanges();
                }
            }

            return RedirectToAction("MyJobs");
        }

        public ActionResult DeleteJob(int id)
        {
            using (var db = new JobPortalDbContext())
            {
                var job = db.Jobs.Find(id);

                if (job != null)
                {
                    db.Jobs.Remove(job);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("MyJobs");
        }

        // =======================================================
        // For applicant page logic
        // =======================================================

        // 1. GET: Loads the custom dashboard view tracking applicant resumes
        public ActionResult Applicants()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Employer")
            {
                return RedirectToAction("Index", "Home");
            }

            int currentEmployerId = Convert.ToInt32(Session["UserID"]);

            using (var db = new JobPortalDbContext())
            {
                // Grabs all system requests where the targeted position matches this employer
                var inboundApplications = db.Applications
                                            .Include(a => a.Job)
                                            .Where(a => a.Job.UserID == currentEmployerId)
                                            .OrderByDescending(a => a.AppliedDate)
                                            .ToList();

                return View(inboundApplications);
            }
        }

        // 2. POST: Processes interactive Accept/Reject actions over an AJAX endpoint pipeline
        [HttpPost]
        public ActionResult UpdateApplicationStatus(int id, string targetStatus)
        {
            if (Session["UserID"] == null)
            {
                return Json(new { success = false });
            }

            using (var db = new JobPortalDbContext())
            {
                var targetApp = db.Applications.Find(id);
                if (targetApp != null)
                {
                    targetApp.Status = targetStatus; // Mutates tracking value state to "Accepted" or "Rejected"
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
    }
}