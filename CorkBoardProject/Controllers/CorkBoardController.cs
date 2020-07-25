using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CorkBoardProject.Models;
using CorkBoardProject.ViewModels;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Data.Entity;
using System.Collections;

namespace CorkBoardProject.Controllers
{
    public class CorkBoardController : Controller {

        CorkBoardTemplateEntities6 watchDbContext = new CorkBoardTemplateEntities6();
        CorkBoardTemplateEntities3 pushpinDbContext = new CorkBoardTemplateEntities3();
        CorkBoardTemplateEntities2 corkboardDbContext = new CorkBoardTemplateEntities2();
        CorkBoardTemplateEntities1 categoryDbContext = new CorkBoardTemplateEntities1();
        CorkBoardTemplateEntities userDbContext = new CorkBoardTemplateEntities();

        // GET: CorkBoard
        public ActionResult Index()
        {
            Dictionary<string,string> rCO = new Dictionary<string, string>();

            Dictionary<string, int> ppCount = new Dictionary<string, int>();

            User user = userDbContext.Users.Where(query => query.Email.Equals(User.Identity.Name)).SingleOrDefault();

            var corkboards = corkboardDbContext.Corkboards.Where(query => query.UserId.Equals(user.Id)).OrderBy(cb => cb.Title).ToList();

            var recentCorks = corkboardDbContext.Corkboards.OrderByDescending(cb => cb.DateTime).Take(4).ToList();

            foreach (var eachRecentCorkBoard in recentCorks)
            {
                String eachName = userDbContext.Users.Single(uid => uid.Id == eachRecentCorkBoard.UserId).First_Name + " " +
                       userDbContext.Users.Single(uid => uid.Id == eachRecentCorkBoard.UserId).Last_Name;
                rCO[eachRecentCorkBoard.Title] = eachName;
            };

            foreach (var eachRecentCorkBoard in corkboards)
            {
                int eachName = pushpinDbContext.Pushpins.Count(x => x.CorkboardId == eachRecentCorkBoard.Cid);
                ppCount[eachRecentCorkBoard.Title] = eachName;
            };


            var viewModel = new HomePageViewModel
            {
                userName = userDbContext.Users.Single(uid => uid.Email == User.Identity.Name).First_Name + " " +
                userDbContext.Users.Single(uid => uid.Email == User.Identity.Name).Last_Name,

                userDetails =  user,

                myCorkboards = corkboards,

                recentCorkboards = recentCorks,

                pushpinCount = ppCount,

                recentCorkboardOwners = rCO
            };
           return View(viewModel);
            
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
        [ValidateAntiForgeryToken]
        public ActionResult SaveCorkboard(AddCorkboardVewModel corkboardDetails)
        {
            if (corkboardDetails.VisibilityTypeId.ToString().Equals("Private"))
            {
                if (ModelState.IsValid)
                {
                    //create database context using Entity framework 
                    using (corkboardDbContext)
                    {
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
                        else
                        {
                            var categoryType = categoryDbContext.CorkboardCategories.ToList();

                            var viewModel = new AddCorkboardVewModel
                            {
                                CorkboardCategories = categoryType
                            };
                            ModelState.AddModelError("Failure", "Wrong Password!");
                            return View("AddCorkboard", viewModel);
                        }
                    }
                }
                else
                {
                    var categoryType = categoryDbContext.CorkboardCategories.ToList();

                    var viewModel = new AddCorkboardVewModel
                    {
                        CorkboardCategories = categoryType
                    };
                    return View("AddCorkboard", viewModel);
                }
            }
            else if (corkboardDetails.VisibilityTypeId.ToString().Equals("Public"))
            {
                    //create database context using Entity framework 
                    using (corkboardDbContext)
                    {
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
            else {
                var categoryType = categoryDbContext.CorkboardCategories.ToList();

                var viewModel = new AddCorkboardVewModel
                {
                    CorkboardCategories = categoryType
                };
                return View("AddCorkboard", viewModel);
            }
            ViewBag.Message = "New CorkBoard has been added saved";
            return RedirectToAction("Index", "CorkBoard");
        }

        public ActionResult ConfirmPrivateCorkBoard(int id)
        {
            var viewModel = new ConfirmPrivateCorkboardViewModel
            {
                CId = id,
                CorkboardTitle = corkboardDbContext.Corkboards.SingleOrDefault(c => c.Cid == id).Title

            };
            return View(viewModel);
        }

        //The login form is posted to this method.
        [HttpPost]
        public ActionResult ConfirmPrivateCorkboard(ConfirmPrivateCorkboardViewModel model, int id)
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

                    var corkboard = corkboardDbContext.Corkboards.SingleOrDefault(c => c.Cid == model.CId);
                    return RedirectToAction("ViewCorkboard","CorkBoard",new { id = id});
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
            var watchList = watchDbContext.Watchs.Where(w => w.CorkboardId == id).ToList();
            var watchCount = watchDbContext.Watchs.Count(w => w.CorkboardId == id);

            ArrayList watchEmail = new ArrayList();
            foreach (var eachLike in watchList)
            { 
                watchEmail.Add(userDbContext.Users.SingleOrDefault(u => u.Id == eachLike.UserId).Email);
            };

            var viewModel = new ViewCorkboardViewModel
            {
                corkboardId = id,
                watch = watchCount,
                watchEmail = watchEmail,
                corkboardCategory = categoryDbContext.CorkboardCategories.Single(c => c.Id == corkboard.CategoryId).Category,
                corkboardVisibility = corkboardDbContext.Corkboards.SingleOrDefault(c => c.Cid == id).VisibilityId,
                corkBoardUserId = corkboard.UserId,
                loggedInUserId = userDbContext.Users.SingleOrDefault(uid => uid.Email == User.Identity.Name).Id,
                userName = userDbContext.Users.Single(uid => uid.Id == corkboard.UserId).First_Name +" "+
                userDbContext.Users.Single(uid => uid.Id == corkboard.UserId).Last_Name,
                corkboardTitle = corkboard.Title,
                lastUpdateTime = corkboard.DateTime,
                pushpins = pushpinDbContext.Pushpins.Where(query => query.CorkboardId == id).ToList()
            };
            return View(viewModel);
        }

        
        public ActionResult IncrementWatch(int corkboardId)
        {
            var corkboard = corkboardDbContext.Corkboards.SingleOrDefault(c => c.Cid == corkboardId);
            if (corkboard == null)
            {
                return HttpNotFound();
            }
            using (watchDbContext)
            {
                Watch watch = new Watch();

                watch.CorkboardId = corkboardId;
                watch.UserId = userDbContext.Users.SingleOrDefault(uid => uid.Email == User.Identity.Name).Id;

                watchDbContext.Watchs.Add(watch);
                watchDbContext.SaveChanges();
            }      
            return RedirectToAction("ViewCorkboard", "CorkBoard", new { id = corkboardId });
            //return View(viewModel);
        }

    }
}
    
