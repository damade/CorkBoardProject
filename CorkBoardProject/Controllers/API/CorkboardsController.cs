using CorkBoardProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CorkBoardProject.Controllers.API
{
    public class CorkboardsController : ApiController
    {
        private CorkBoardTemplateEntities2 corkboardDbContext;

        public CorkboardsController()
        {
            corkboardDbContext = new CorkBoardTemplateEntities2();
        }

        // Get api/corkboards
        public IEnumerable<Corkboard> GetCorkboards()
        {
            return corkboardDbContext.Corkboards.ToList();
        }

        // Get api/corkboards/id
        public Corkboard GetCorkboard(int id) 
        {
            var corkboard = corkboardDbContext.Corkboards.SingleOrDefault(cc => cc.Cid == id);

            if(corkboard == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return corkboard;

        }

        //Post api/customers
        [HttpPost]
        public Corkboard CreateCorkboard(Corkboard corkboard)
        {
            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            corkboardDbContext.Corkboards.Add(corkboard);
            corkboardDbContext.SaveChanges();

            return corkboard;
        }

        //DELETE /api/corkboard/id
        [HttpDelete]
        public void DeleteCorkboard(int id)
        {
            var corkboard = corkboardDbContext.Corkboards.SingleOrDefault(cc => cc.Cid == id);

            if (corkboard == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            corkboardDbContext.Corkboards.Remove(corkboard);
            corkboardDbContext.SaveChanges();
        }

    }
}
