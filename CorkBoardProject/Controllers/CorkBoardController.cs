﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorkBoardProject.Models;
using CorkBoardProject.ViewModels;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace CorkBoardProject.Controllers
{
    public class CorkBoardController : Controller
    {
        CorkBoardTemplateEntities2 corkboardDbContext = new CorkBoardTemplateEntities2();
        CorkBoardTemplateEntities1 categoryDbContext = new CorkBoardTemplateEntities1();
        CorkBoardTemplateEntities userDbContext = new CorkBoardTemplateEntities();

        // GET: CorkBoard
        public ViewResult Index()
        {
            var corkboards = corkboardDbContext.Corkboards.ToList();
            return View(corkboards);
        }

        public ActionResult CorkboardDetails(int id)
        {
            var corkboard = corkboardDbContext.Corkboards.SingleOrDefault(c => c.Cid == id);
            if (corkboard == null) {
                return HttpNotFound();
            }
            return View(corkboard);
        }
        //Return Register view
        public ActionResult AddCorkboard()
        {
            var categoryType = categoryDbContext.CorkboardCategories.ToList();

            var viewModel = new AddCorkboardVewModel
            {
                CorkboardCategories = categoryType
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveCorkboard(AddCorkboardVewModel corkboardDetails)
        {
            //We check if the model state is valid or not. We have used DataAnnotation attributes.
            //If any form value fails the DataAnnotation validation the model state becomes invalid.
            if (!ModelState.IsValid) {
                var categoryType = categoryDbContext.CorkboardCategories.ToList();

                var viewModel = new AddCorkboardVewModel
                {
                    CorkboardCategories = categoryType
                };
                return View("AddCorkboard",viewModel);
            }
                //create database context using Entity framework 
                using (corkboardDbContext)
                {
                    if (corkboardDetails.VisibilityTypeId.ToString().Equals("Private")) {

                        User user = userDbContext.Users.Where(query => query.Email.Equals(User.Identity.Name)
                        && query.Password.Equals(corkboardDetails.Pin)).SingleOrDefault();
                        if (user != null)
                        {
                            CorkboardCategory corkboardCategory = categoryDbContext.
                                                            CorkboardCategories.
                                                            Where(query => query.Id.Equals(corkboardDetails.CategoryTypeId))
                                                            .SingleOrDefault();
                            Corkboard corkboard = new Corkboard();
                            corkboard.UserId = user.Id;
                            corkboard.Title = corkboardDetails.Title;
                            corkboard.CategoryId = corkboardCategory.Id;
                            corkboard.VisibilityId = 1;
                            corkboard.Watch = 0;
                            corkboard.DateTime = DateTime.Now;

                            corkboardDbContext.Corkboards.Add(corkboard);
                            corkboardDbContext.SaveChanges();
                    }
                        else {
                            ModelState.AddModelError("Failure", "Wrong Password!");
                            return RedirectToAction("AddCorkboard", "CorkBoard");
                        }
                    }
                    else if (corkboardDetails.VisibilityTypeId.ToString().Equals("Public")) {
                        Corkboard corkboard = new Corkboard();
                        CorkboardCategory corkboardCategory = categoryDbContext.
                                                            CorkboardCategories.
                                                            Where(query => query.Id.Equals(corkboardDetails.CategoryTypeId))
                                                            .SingleOrDefault();

                        string Email = User.Identity.Name;
                        User user = userDbContext.Users.Where(query => query.Email.Equals(Email)).SingleOrDefault();
                        corkboard.UserId = user.Id;
                        corkboard.Title = corkboardDetails.Title;
                        corkboard.CategoryId = corkboardCategory.Id;
                        corkboard.VisibilityId = 0;
                        corkboard.Watch = 0;
                        corkboard.DateTime = DateTime.Now;

                        corkboardDbContext.Corkboards.Add(corkboard);
                        corkboardDbContext.SaveChanges();
                }
                }
                 
                 ViewBag.Message = "New CorkBoard has been added saved";
                 return RedirectToAction("Index", "CorkBoard");
        }
        
        public ActionResult ConfirmPrivateCorkBoard()
        {
            return View();
        }

        //The login form is posted to this method.
        [HttpPost]
        public ActionResult ConfirmPrivateCorkboard(ConfirmPrivateCorkboardViewModel model)
        {
            //Checking the state of model passed as parameter.
            if (ModelState.IsValid)
            {

                //Validating the user, whether the user is valid or not.
                var isValidPassword = IsValidPassword(model);

                //If user is valid & present in database, we are redirecting it to Welcome page.
                if (isValidPassword != null)
                {
                    //FormsAuthentication.SetAuthCookie(model.Pin, false);
                    return RedirectToAction("ViewCorkboard");
                }
                else
                {
                    //If the username and password combination is not present in DB then error message is shown.
                    ModelState.AddModelError("Failure", "Wrong password !");
                    return View();
                }
            }
            else
            {
                //If model state is not valid, the model with error message is returned to the View.
                return View(model);
            }
        }

        //function to check if User is valid or not
        public User IsValidPassword(ConfirmPrivateCorkboardViewModel model)
        {
            using (var dataContext = new CorkBoardTemplateEntities())
            {
                //Retireving the user details from DB based on username and password enetered by user.
                string Email = User.Identity.Name;
                User user = dataContext.Users.Where(query => query.Email.Equals(Email) && query.Password.Equals(model.Pin)).SingleOrDefault();
                //If user is present, then true is returned.
                if (user == null)
                    return null;
                //If user is not present false is returned.
                else
                    return user;
            }
        }

        public ActionResult ViewCorkboard(int id)
        {
            var corkboard = corkboardDbContext.Corkboards.SingleOrDefault(c => c.Cid == id);
            if (corkboard == null)
            {
                return HttpNotFound();
            }
            return View(corkboard);
        }

       
    }
}
    
