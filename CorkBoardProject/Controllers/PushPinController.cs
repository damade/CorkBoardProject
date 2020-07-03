using AutoMapper;
using CorkBoardProject.Models;
using CorkBoardProject.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Ast.Selectors;

namespace CorkBoardProject.Controllers
{
    public class PushPinController : Controller
    {
        CorkBoardTemplateEntities5 commentDbContext = new CorkBoardTemplateEntities5();
        CorkBoardTemplateEntities4 likeDBContext = new CorkBoardTemplateEntities4();
        CorkBoardTemplateEntities3 pushpinDbContext = new CorkBoardTemplateEntities3();
        CorkBoardTemplateEntities2 corkboardDbContext = new CorkBoardTemplateEntities2();
        CorkBoardTemplateEntities1 categoryDbContext = new CorkBoardTemplateEntities1();
        CorkBoardTemplateEntities userDbContext = new CorkBoardTemplateEntities();

        // GET: PushPin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddPushpin(int id)
        {
            var viewModel = new AddPushpinViewModel
            {
                CorkboardId = id,
                CorkboardTitle = corkboardDbContext.Corkboards.SingleOrDefault(cid => cid.Cid == id).Title
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPushpin(AddPushpinViewModel pushpinDetails, int id)
        {
            Uri uriResult;
            bool check = IsImageUrl(pushpinDetails.Url);
            if (Uri.IsWellFormedUriString(pushpinDetails.Url, UriKind.Absolute) && check)
            {
                if (ModelState.IsValid)
                {
                    using (pushpinDbContext)
                    {
                        Pushpin pushpin = new Pushpin();

                        pushpin.CorkboardId = id;
                        pushpin.Url = pushpinDetails.Url;
                        pushpin.Description = pushpinDetails.Description;
                        pushpin.Tags = pushpinDetails.Tags;
                        pushpin.DateTime = DateTime.Now;

                        Corkboard corkboardInDb = corkboardDbContext.Corkboards.Where(query => query.Cid.Equals(id)).SingleOrDefault();
                        corkboardInDb.DateTime = DateTime.Now;

                        pushpinDbContext.Pushpins.Add(pushpin);
                        pushpinDbContext.SaveChanges();
                        corkboardDbContext.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("Failure", "Fill in the right details");
                    return View("AddPushpin", pushpinDetails);
                }

            }
            else if (Uri.TryCreate(pushpinDetails.Url, UriKind.Absolute, out uriResult)
                     && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) && check)
                    {
                        if (ModelState.IsValid)
                        {
                            using (pushpinDbContext)
                            {
                                Pushpin pushpin = new Pushpin();

                                pushpin.CorkboardId = id;
                                pushpin.Url = pushpinDetails.Url;
                                pushpin.Description = pushpinDetails.Description;
                                pushpin.Tags = pushpinDetails.Tags;
                                pushpin.DateTime = DateTime.Now;

                                var corkboardInDb = corkboardDbContext.Corkboards.SingleOrDefault(c => c.Cid == id);
                                corkboardInDb.DateTime = DateTime.Now;

                                pushpinDbContext.Pushpins.Add(pushpin);
                                pushpinDbContext.SaveChanges();
                                corkboardDbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Failure", "Fill in the right details");
                            return View("AddPushpin", pushpinDetails);
                        }
                  }
            else
            {
                  ModelState.AddModelError("Failure", "Enter a valid http or https url");
                  return View("AddPushpin", pushpinDetails);
            }
            ViewBag.Message = "New CorkBoard has been added saved";
            return RedirectToAction("Index", "CorkBoard");
        }

        bool IsImageUrl(string URL)
        {
            var req = (HttpWebRequest)HttpWebRequest.Create(URL);
            req.Method = "HEAD";
            using (var resp = req.GetResponse())
            {
                return resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                           .StartsWith("image/");
            }
        }

        public ActionResult ViewPushpin(int id)

        {
            Dictionary<string, string> rCO = new Dictionary<string, string>();

            var pushpin = pushpinDbContext.Pushpins.SingleOrDefault(p => p.Pid == id);
            if (pushpin == null)
            {
                return HttpNotFound();
            }

            var corkboard = corkboardDbContext.Corkboards.SingleOrDefault(c => c.Cid == pushpin.CorkboardId);
            var allComments = commentDbContext.Comments.OrderByDescending(cb => cb.DateTime).ToList();
            var allLikesList = likeDBContext.Likes.Where(p => p.PushpinId == id).ToList();

            foreach (var eachComments in allComments)
            {
                String eachName = userDbContext.Users.Single(uid => uid.Id == eachComments.UserId).First_Name + " " +
                       userDbContext.Users.Single(uid => uid.Id == eachComments.UserId).Last_Name;
                rCO[eachComments.Comment1] = eachName;
            };

            int likesCount = likeDBContext.Likes.Count(p => p.PushpinId == id);

            if (likesCount <= 6)
            {
                string likesUserName = " ";
                ArrayList likeEmail = new ArrayList();
                var theLikeListToUse = likeDBContext.Likes.Where(p => p.PushpinId == id).OrderByDescending(p => p.DateTime).ToList();

                foreach (var eachLike in theLikeListToUse)
                {
                    likesUserName += (userDbContext.Users.Single(uid => uid.Id == eachLike.UserId).First_Name + " " +
                           userDbContext.Users.Single(uid => uid.Id == eachLike.UserId).Last_Name + ", ");
                    likeEmail.Add(userDbContext.Users.SingleOrDefault(u => u.Id == eachLike.UserId).Email);
                };

                likesUserName += ".";
                var viewModel = new ViewPushpinViewModel
                {
                    userName = userDbContext.Users.Single(uid => uid.Id == corkboard.UserId).First_Name + " " +
                    userDbContext.Users.Single(uid => uid.Id == corkboard.UserId).Last_Name,
                    userId = corkboard.UserId,
                    corkboardId = corkboard.Cid,
                    pushpinId = pushpin.Pid,
                    corkboardTitle = corkboard.Title,
                    pushpinDescription = pushpin.Description,
                    pushpinUrl = pushpin.Url,
                    pushpinTags = pushpin.Tags,
                    lastUpdateTime = pushpin.DateTime,
                    pushpinComments = allComments,
                    commentOwners = rCO,
                    pushpinLikes = allLikesList,
                    allLikes = likesUserName,
                    likesEmail = likeEmail
                    
                };
                return View(viewModel);
            }
            else
            {
                string likesUserName = " ";
                ArrayList likeEmail = new ArrayList();
                var theLikeListToUse = likeDBContext.Likes.Where(p => p.PushpinId == id).OrderByDescending(p => p.DateTime).Take(8).ToList();

                foreach (var eachLike in theLikeListToUse)
                {
                    likesUserName += (userDbContext.Users.Single(uid => uid.Id == eachLike.UserId).First_Name + " " +
                           userDbContext.Users.Single(uid => uid.Id == eachLike.UserId).Last_Name + ", ");
                    likeEmail.Add(userDbContext.Users.SingleOrDefault(u => u.Id == eachLike.UserId).Email);
                };

                likesUserName += "and Others.";

                var viewModel = new ViewPushpinViewModel
                {
                    userName = userDbContext.Users.Single(uid => uid.Id == corkboard.UserId).First_Name + " " +
                    userDbContext.Users.Single(uid => uid.Id == corkboard.UserId).Last_Name,
                    userId = corkboard.UserId,
                    corkboardId = corkboard.Cid,
                    pushpinId = pushpin.Pid,
                    corkboardTitle = corkboard.Title,
                    pushpinDescription = pushpin.Description,
                    pushpinUrl = pushpin.Url,
                    pushpinTags = pushpin.Tags,
                    lastUpdateTime = pushpin.DateTime,
                    pushpinComments = allComments,
                    commentOwners = rCO,
                    pushpinLikes = allLikesList,
                    allLikes = likesUserName,
                    likesEmail = likeEmail

                };
                return View(viewModel);
            }       
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewPushpin(ViewPushpinViewModel commentViewModel , int id)
        {
            if (ModelState.IsValid)
            {
                using (commentDbContext)
                {
                    Comment comment = new Comment();
                    comment.UserId = userDbContext.Users.SingleOrDefault(uid => uid.Email == User.Identity.Name).Id;
                    comment.Comment1 = commentViewModel.commentToAdd;
                    comment.PushpinId = id;
                    comment.DateTime = DateTime.Now;
                   
                    commentDbContext.Comments.Add(comment);
                    commentDbContext.SaveChanges();
                }
                return RedirectToAction("ViewPushpin", "PushPin", new { id = id });
            }
            else
            {
                 ModelState.AddModelError("Failure", "Fill in the right details");
                 return View(commentViewModel);
            }
           
        }

        
        public ActionResult LikeAction(int likeDecision, int pushpinId)
        {
            var pusphpin = pushpinDbContext.Pushpins.SingleOrDefault(p => p.Pid == pushpinId);
            if (pusphpin == null)
            {
                return HttpNotFound();
            }
            if (likeDecision == 0)
            {
                using (likeDBContext)
                {
                    var userId = userDbContext.Users.SingleOrDefault(u => u.Email == User.Identity.Name).Id;
                    var likeToRemove = likeDBContext.Likes.SingleOrDefault(l => l.PushpinId.Equals(pushpinId) && l.UserId.Equals(userId) ) ;

                    likeDBContext.Likes.Remove(likeToRemove);
                    likeDBContext.SaveChanges();
                }
                return RedirectToAction("ViewPushpin", "Pushpin", new { id = pushpinId });
            }
            else if (likeDecision == 1)
            {
                using (likeDBContext)
                {
                    Like like = new Like();
                    like.PushpinId = pushpinId;
                    like.UserId = userDbContext.Users.SingleOrDefault(uid => uid.Email == User.Identity.Name).Id;
                    like.DateTime = DateTime.Now;

                    likeDBContext.Likes.Add(like);
                    likeDBContext.SaveChanges();
                }
                return RedirectToAction("ViewPushpin", "Pushpin", new { id = pushpinId });
            }
            else 
            {
                using(likeDBContext)
                {
                    Like like = new Like();
                    like.PushpinId = pushpinId;
                    like.UserId = userDbContext.Users.SingleOrDefault(uid => uid.Email == User.Identity.Name).Id;
                    like.DateTime = DateTime.Now;

                    likeDBContext.Likes.Add(like);
                    likeDBContext.SaveChanges();
                }
                return RedirectToAction("ViewPushpin", "Pushpin", new { id = pushpinId });
            }            
            //return View(viewModel);
        }
    }
}