using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPortalSystem.Data;
using JobPortalSystem.Models;
using System.Data.Entity;

namespace JobPortalSystem.Controllers
{
    public class JobSeekerController : Controller
    {
        // Guard to ensure user is logged in as JobSeeker
        private bool IsAuthorized()
        {
            return Session["Role"] != null && Session["Role"].ToString() == "JobSeeker";
        }

        // GET: JobSeeker/Dashboard
        public ActionResult Dashboard()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");

            int currentUserId = (int)Session["UserId"];

            using (var db = new JobPortalDbContext())
            {
                ViewBag.AvailableJobsCount = db.Jobs.Count();
                ViewBag.AppliedCount = db.Applications.Count(a => a.UserID == currentUserId);
            }

            return View("JobSeekerDashboard");
        }

        // GET: JobSeeker/BrowseJobs
        public ActionResult BrowseJobs()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");

            using (var db = new JobPortalDbContext())
            {
                var openJobs = db.Jobs.OrderByDescending(j => j.CreatedDate).ToList();
                return View(openJobs);
            }
        }

        // GET: JobSeeker/Apply
        public ActionResult Apply(int? jobId)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");

            // Safety check: if jobId parameter is missing, don't crash, redirect back safely.
            if (jobId == null)
            {
                return RedirectToAction("BrowseJobs");
            }

            using (var db = new JobPortalDbContext())
            {
                var job = db.Jobs.FirstOrDefault(j => j.JobID == jobId.Value);

                // Prevent access if job does not exist or deadline has passed
                if (job == null || (job.Deadline.HasValue && job.Deadline.Value.Date < DateTime.Today))
                {
                    TempData["ErrorMessage"] = "This job posting has expired and is no longer accepting applications.";
                    return RedirectToAction("BrowseJobs");
                }
            }

            var appModel = new Application { JobID = jobId.Value };
            return View(appModel);
        }

        // POST: JobSeeker/Apply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(JobPortalSystem.Models.Application app, HttpPostedFileBase CVFile)
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");

            // Prevent properties assigned programmatically from causing validation state failures
            ModelState.Remove("CVPath");
            ModelState.Remove("Status");

            using (var db = new JobPortalDbContext())
            {
                var job = db.Jobs.FirstOrDefault(j => j.JobID == app.JobID);

                // Server-side check for deadline prior to saving
                if (job == null || (job.Deadline.HasValue && job.Deadline.Value.Date < DateTime.Today))
                {
                    TempData["ErrorMessage"] = "Cannot submit application. This job posting has expired.";
                    return RedirectToAction("BrowseJobs");
                }

                if (ModelState.IsValid)
                {
                    string trackingPath = "";

                    // Save the CV file physically to the server filesystem
                    if (CVFile != null && CVFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(CVFile.FileName);
                        string extension = Path.GetExtension(CVFile.FileName);
                        string uniqueName = fileName + "_" + Guid.NewGuid().ToString().Substring(0, 8) + extension;

                        string directoryPath = Server.MapPath("~/Uploads/CVs/");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        CVFile.SaveAs(Path.Combine(directoryPath, uniqueName));
                        trackingPath = "~/Uploads/CVs/" + uniqueName;
                    }

                    var directApp = new JobPortalSystem.Models.Application()
                    {
                        JobID = app.JobID,
                        FullName = app.FullName,
                        Email = app.Email,
                        Phone = app.Phone,
                        Education = app.Education,
                        CVPath = trackingPath,
                        UserID = (int)Session["UserId"],
                        Status = "Pending",
                        AppliedDate = DateTime.Now
                    };

                    db.Applications.Add(directApp);
                    db.SaveChanges();

                    // Temporary flash message caught dynamically on the dashboard view
                    TempData["SuccessMessage"] = "Application submitted successfully!";
                    return RedirectToAction("Dashboard", "JobSeeker");
                }
            }

            return View(app);
        }

        // GET: JobSeeker/MyApplications
        public ActionResult MyApplications()
        {
            if (!IsAuthorized()) return RedirectToAction("Index", "Home");

            int currentUserId = (int)Session["UserId"];

            using (var db = new JobPortalDbContext())
            {
                var myApps = db.Applications.Include("Job").Where(a => a.UserID == currentUserId).ToList();
                return View(myApps);
            }
        }
    }
}