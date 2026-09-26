using Prelims.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using Prelims.Models.Reports;

namespace Prelims.Controllers
{
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        private FntEntities db = new FntEntities();
        public ActionResult Index()
        {
            USERINFO userInfo = (USERINFO)Session["UserInfo"];
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Account");
            }

            HomeViewModel homeViewModel = new HomeViewModel();

            homeViewModel.InprogressAudits = db.Audits.AsQueryable().Include(x => x.AuditRequestType)
                .Include(x => x.AuditStatus)
                .Include(x => x.AuditTask)
                .Include(x => x.AuditTimeEntries)
                .Include(x => x.AuditSender)
                .Include(x => x.CRN).Where(x => x.AuditTimeEntries.Any(y => y.IsInprogress)).ToList();


            List<LiveStatusReportViewModel> liveStatusItems = new List<LiveStatusReportViewModel>();

            var groupByCrn = (from k in db.Audits.Where(x => x.StatusId == 3 && x.StatusId != 4 && x.StatusId != 5)
                              group k by k.CrnId into grp
                              select new { key = grp.Key, Count = grp.Count() }).ToList();



            foreach (var crn in db.CRNs.Where(x => x.IsVisibleInMainApplication == true))
            {
                LiveStatusReportViewModel liveStatusViewModel = new LiveStatusReportViewModel();

                liveStatusViewModel.OfficeName = crn.CRNNAME;
                liveStatusViewModel.GroupName = crn.GroupName;

                var grpItem = groupByCrn.FirstOrDefault(x => x.key == crn.CRNID);
                if (grpItem != null)
                {
                    liveStatusViewModel.HoldCount = grpItem.Count;
                }else
                {
                    liveStatusViewModel.HoldCount = 0;
                }

                liveStatusViewModel.TaxesBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 6 && x.RequestTypeId == 4);
                liveStatusViewModel.TaxesBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 6 && x.RequestTypeId == 2);
                liveStatusViewModel.TaxesSellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 6 && x.RequestTypeId == 1);
                liveStatusViewModel.TaxesCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 6);

                liveStatusViewModel.LVBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId==crn.CRNID && x.TaskId == 1 && x.RequestTypeId == 4);
                liveStatusViewModel.LVBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 1 && x.RequestTypeId == 2);
                liveStatusViewModel.LVSellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 1 && x.RequestTypeId == 1);
                liveStatusViewModel.LVCount= db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 1);

                liveStatusViewModel.PIBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 2 && x.RequestTypeId == 4);
                liveStatusViewModel.PIBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 2 && x.RequestTypeId == 2);
                liveStatusViewModel.PISellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 2 && x.RequestTypeId == 1);
                liveStatusViewModel.PICount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 2);

                liveStatusViewModel.GIBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 3 && x.RequestTypeId == 4);
                liveStatusViewModel.GIBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 3 && x.RequestTypeId == 2);
                liveStatusViewModel.GISellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 3 && x.RequestTypeId == 1);
                liveStatusViewModel.GICount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 3);

                liveStatusViewModel.StarterBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 4 && x.RequestTypeId == 4);
                liveStatusViewModel.StarterBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 4 && x.RequestTypeId == 2);
                liveStatusViewModel.StarterSellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 4 && x.RequestTypeId == 1);
                liveStatusViewModel.StarterCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 4);

                liveStatusViewModel.NotesBothCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 5 && x.RequestTypeId == 4);
                liveStatusViewModel.NotesBuyerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 5 && x.RequestTypeId == 2);
                liveStatusViewModel.NotesSellerCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 5 && x.RequestTypeId == 1);
                liveStatusViewModel.NotesCount = db.Audits.Count(x => (x.StatusId == 1 || x.StatusId == 2) && x.CrnId == crn.CRNID && x.TaskId == 5);

                liveStatusItems.Add(liveStatusViewModel);
            }
            homeViewModel.LiveStatusItems = liveStatusItems;

            var inprogressTitleOrderIds = db.AuditTimeEntries.Where(x => x.IsInprogress).Select(x => x.AuditId).ToList();

            int? userID = userInfo.USERID;
            var myOrderList = (from k in db.Audits
                               where k.StatusId != 3 && k.StatusId != 4 && k.StatusId != 5 && !inprogressTitleOrderIds.Any(x => x == k.Id) && k.UserAssigned == userID
                               select new MyOrder
                               {
                                   COUNTYNAME = k.County,
                                   CRNDISPLAYNAME = k.CRN.CRNDISPLAYNAME,
                                   TitleOrderRequestTypeName = k.AuditRequestType.Name,
                                   IsRushOrder = k.IsRushOrder,
                                   TitleOrderTaskName = k.AuditTask.Name,
                                   ReceivedDateTime = k.DateCreated,
                                   OrderNo = k.OrderNo,
                                   Id = k.Id
                               }).OrderByDescending(x => x.IsRushOrder).ToList();

            homeViewModel.MyOrders = myOrderList.ToList();

            AuditTimeEntry timeEntry = db.AuditTimeEntries.FirstOrDefault(x => x.IsInprogress && x.UserId == userInfo.USERID);

            if (timeEntry != null)
            {
                ViewBag.InprogressTitleOrderId = timeEntry.AuditId;
            }

            return View(homeViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}