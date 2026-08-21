using System.Linq;
using System.Web.Mvc;
using JobPortalSystem.Data;

namespace JobPortalSystem.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Dashboard()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            using (var db = new JobPortalDbContext())
            {
                ViewBag.TotalUsers = db.Users.Count();
                ViewBag.JobSeekers = db.Users.Count(u => u.Role == "JobSeeker");
                ViewBag.Employers = db.Users.Count(u => u.Role == "Employer");
            }

            return View();
        }

        public ActionResult ManageUsers()
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            using (var db = new JobPortalSystem.Data.JobPortalDbContext())
            {
                var users = db.Users.ToList();
                return View(users);
            }
        }
        public ActionResult ApproveUser(int id)
        {
            using (var db = new JobPortalSystem.Data.JobPortalDbContext())
            {
                var user = db.Users.Find(id);

                if (user != null)
                {
                    user.Status = "Active";
                    db.SaveChanges();
                }
            }

            return RedirectToAction("ManageUsers");
        }

        public ActionResult DeleteUser(int id)
        {
            using (var db = new JobPortalSystem.Data.JobPortalDbContext())
            {
                var user = db.Users.Find(id);

                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("ManageUsers");
        }
    }
}