using AutoMapper;
using CorkBoardProject.Models;
using CorkBoardProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CorkBoardProject.Controllers
{
    public class PushPinController : Controller
    {
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
            var pushpin = pushpinDbContext.Pushpins.SingleOrDefault(p => p.Pid == id);
            if (pushpin == null)
            {
                return HttpNotFound();
            }
            var corkboard = corkboardDbContext.Corkboards.SingleOrDefault(c => c.Cid == pushpin.CorkboardId);
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
                lastUpdateTime = pushpin.DateTime
                
            };
            return View(viewModel);
        }
    }
}