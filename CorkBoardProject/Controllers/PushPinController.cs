using AutoMapper;
using CorkBoardProject.Extra_Classes;
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
using System.Web.WebPages;
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
            ViewBag.Message = "New Pushpin has been added saved";
            return RedirectToAction("ViewCorkboard", "CorkBoard", new { id = id });
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
            var allComments = commentDbContext.Comments.Where(cb => cb.PushpinId == id).OrderByDescending(cb => cb.DateTime).ToList();
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

        
        public ActionResult SearchPushpin(string whatToSearch) {
            if (whatToSearch.IsEmpty()) 
            {
                return HttpNotFound();
            }
            List<Pushpin> searchResult = new List<Pushpin>();
            var allPushpins = pushpinDbContext.Pushpins.ToList();
            var allCorkboards = corkboardDbContext.Corkboards.ToList();

            Dictionary<string, string> corkboardDict = new Dictionary<string, string>();

            Dictionary<string, string> pushpinOwnersDict = new Dictionary<string, string>();

            foreach (var eachpushpin in allPushpins) 
            {
                if (eachpushpin.Description.Contains(whatToSearch) || eachpushpin.Tags.Contains(whatToSearch))
                {
                    searchResult.Add(eachpushpin);
                }
            }

            foreach (var eachcorkboard in allCorkboards)
            {
                string theCorkboardCategory = categoryDbContext.CorkboardCategories.SingleOrDefault(cc => cc.Id == eachcorkboard.CategoryId).Category.ToLower();
                if (theCorkboardCategory.Contains(whatToSearch))
                {
                    foreach (var eachpushpin in allPushpins)
                    {
                        if (eachpushpin.CorkboardId == eachcorkboard.Cid && !searchResult.Contains(eachpushpin))
                        {
                            searchResult.Add(eachpushpin);
                        }
                    }        
                }
            }

            foreach (var eachpushpinResult in searchResult)
            {
                var eachCorkboard = corkboardDbContext.Corkboards.SingleOrDefault(cc => cc.Cid == eachpushpinResult.CorkboardId);
                String eachCorkboardTitle = eachCorkboard.Title;

                String eachName = userDbContext.Users.Single(uid => uid.Id == eachCorkboard.UserId).First_Name + " " +
                       userDbContext.Users.Single(uid => uid.Id == eachCorkboard.UserId).Last_Name;

                pushpinOwnersDict[eachpushpinResult.Description] = eachName;
                corkboardDict[eachpushpinResult.Description] = eachCorkboardTitle;
            };

            List<Pushpin> sortedSearchResult = searchResult.OrderBy(s => s.Description).ToList();

            var viewModel = new SearchPageViewModel
            {
                allSearchResult = sortedSearchResult,
                corkboardResult = corkboardDict,
                pushpinOwners = pushpinOwnersDict
            };

            return View(viewModel);
        }

        public ActionResult CorkboardStat()
        {
           
            List<Stat> statResult = new List<Stat>();

            var allUsers = userDbContext.Users.ToList();

            foreach (var eachUser in allUsers)
            {
                String eachName = userDbContext.Users.Single(uid => uid.Id == eachUser.Id).First_Name + " " +
                       userDbContext.Users.Single(uid => uid.Id == eachUser.Id).Last_Name;

                var pubCork = corkboardDbContext.Corkboards.Where(cb => cb.UserId.Equals(eachUser.Id) && cb.VisibilityId.Equals(0)).ToList();

                var priCork = corkboardDbContext.Corkboards.Where(cb => cb.UserId.Equals(eachUser.Id) && cb.VisibilityId.Equals(1)).ToList();

                int puc = corkboardDbContext.Corkboards.Count(cb => cb.UserId.Equals(eachUser.Id) && cb.VisibilityId.Equals(0));

                int prc = corkboardDbContext.Corkboards.Count(cb => cb.UserId.Equals(eachUser.Id) && cb.VisibilityId.Equals(1));

                int cpup = 0;

                int cprp = 0;

                foreach (var eachpubCork in pubCork)
                {
                    cpup += pushpinDbContext.Pushpins.Count(pp => pp.CorkboardId == eachpubCork.Cid);
                }

                foreach (var eachpubCork in priCork)
                {
                    cprp += pushpinDbContext.Pushpins.Count(pp => pp.CorkboardId == eachpubCork.Cid);
                }

                Stat stat = new Stat();

                stat.userName = eachName;
                stat.pubCbCount = puc;
                stat.priCbCount = prc;
                stat.pubPinCount = cpup;
                stat.priPinCount = cprp;

                statResult.Add(stat);

            }

            List<Stat> sortedStatResult = statResult.OrderByDescending(s => s.pubCbCount).ThenByDescending(s => s.priCbCount).ToList();
            //searchResult.OrderBy(s => s.Description).ToList();

            var viewModel = new CorkboardStatViewModel
            {
                allStatResult = sortedStatResult
            };

            return View(viewModel);
        }

        public ActionResult PopularSite()
        {
            Dictionary<string, int> siteCount = new Dictionary<string, int>();

            var allPushpins = pushpinDbContext.Pushpins.ToList();

            foreach (var eachpushpin in allPushpins)
            {
                var uri = new Uri(eachpushpin.Url);
                string hostSite = uri.Host;
                if (siteCount.ContainsKey(hostSite))
                {
                    int currentCount = siteCount[hostSite];
                    siteCount[hostSite] = (currentCount += 1);
                }
                else
                {
                    siteCount[hostSite] = 1;
                }
            }

            var newSiteCount = siteCount.OrderByDescending(sc => sc.Value);

            var viewModel = new PopularSiteViewModel
            {
                websiteCount = newSiteCount
            };

            return View(viewModel);
        }

        public ActionResult PopularTags()
        {
            Dictionary<string, int> siteCount = new Dictionary<string, int>();

            List<TagCount> tagsResult = new List<TagCount>();

            ArrayList allTags = new ArrayList();

            var allPushpins = pushpinDbContext.Pushpins.ToList();

            foreach (var eachpushpin in allPushpins)
            {
                string theTagForEachPushpin = eachpushpin.Tags;
                var splitedTag = theTagForEachPushpin.Split(',',' ');
                foreach (var item in splitedTag)
                {
                    if (!allTags.Contains(item)) 
                    {
                        allTags.Add(item);
                    }
                } 
            }

            var newSiteCount = siteCount.OrderByDescending(sc => sc.Value);

            var viewModel = new PopularTagViewModel
            {
                tagSearchResult = tagsResult
            };

            return View(viewModel);
        }


    }
}
