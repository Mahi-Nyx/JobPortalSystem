using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPortalSystem.Models;

namespace JobPortalSystem.Controllers
{
    public class AccountController : Controller
    {
        // 1. GET: Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // 2. POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new JobPortalSystem.Data.JobPortalDbContext())
                {
                    // Check if this email is already registered
                    var emailExists = db.Users.Any(u => u.Email == model.Email);
                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "This email address is already taken.");
                        return View(model);
                    }

                    // Save the user safely
                    db.Users.Add(model);
                    db.SaveChanges();
                }

                // Pass a temporary success message to the Login page
                TempData["SuccessMessage"] = "Registration successful! Please log in below.";

                return RedirectToAction("Login");
            }
            return View(model);
        }

        // 3. GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // 4. POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string Password)
        {
            using (var db = new JobPortalSystem.Data.JobPortalDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);

                if (user != null)
                {
                    // Employer must be approved by Admin
                    if (user.Role == "Employer" && user.Status == "Pending")
                    {
                        ViewBag.ErrorMessage = "Your account is pending admin approval.";
                        return View();
                    }

                    Session["UserID"] = user.UserID;
                    Session["FullName"] = user.FullName;
                    Session["Role"] = user.Role;

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }

                    if (user.Role == "Employer")
                    {
                        return RedirectToAction("EmployDashboard", "Employer");
                    }

                    if (user.Role == "JobSeeker")
                    {
                        return RedirectToAction("Dashboard", "JobSeeker");
                    }
                }

                ViewBag.ErrorMessage = "Invalid email or password.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // 5. GET: Account/ForgotPassword
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // 6. POST: Account/ForgotPassword (Direct Reset)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
            {
                ViewBag.ErrorMessage = "Please fill in all required fields.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match.";
                return View();
            }

            using (var db = new JobPortalSystem.Data.JobPortalDbContext())
            {
                // Find user in database
                var user = db.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "No account found with that email address.";
                    return View();
                }

                // Update password
                user.Password = newPassword;
                db.SaveChanges();
            }

            TempData["SuccessMessage"] = "Password updated successfully! Please log in.";
            return RedirectToAction("Login");
        }
    }
}