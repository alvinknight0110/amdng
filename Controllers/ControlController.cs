using AMDng.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMDng.Controllers
{
    public class ControlController : Controller
    {
        readonly string _DBConn = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        public ActionResult Draw()
        {
            if (Session["AMD"] == null) Response.Redirect("~/Control/Login");
            return View();
        }

        public ActionResult Index()
        {
            if (Session["AMD"] == null) return Redirect("~/Control/Login");
            return PartialView();
        }

        public ActionResult Login()
        {
            if (Session["AMD"] != null) return Redirect("~/Control");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Password)
        {
            if (Password == "AMDVerifier")
            {
                Session["AMD"] = true;
                return Redirect("~/Control");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return Redirect("~/Control/Login");
        }

        /// <summary>
        /// 查詢兌換紀錄
        /// </summary>
        /// <param name="Model">AMD</param>
        /// <param name="SDate">日期開始區間</param>
        /// <param name="EDate">日期結束區間</param>
        /// <returns></returns>
        public ActionResult Query(AMD Model, DateTime? SDate, DateTime? EDate)
        {
            try
            {
                using (SqlConnection dbConn = new SqlConnection(_DBConn))
                {
                    string strSQL = "SELECT * FROM AMD WHERE ID > 0 AND Type=@Type";
                    if (!string.IsNullOrWhiteSpace(Model.UName)) strSQL += " AND UName LIKE N'%'+@UName+'%'";
                    if (!string.IsNullOrWhiteSpace(Model.UPhone)) strSQL += " AND UPhone=@UPhone";
                    if (!string.IsNullOrWhiteSpace(Model.UEmail)) strSQL += " AND UEmail=@UEmail";
                    if (!string.IsNullOrWhiteSpace(Model.UInvoice)) strSQL += " AND UInvoice=@UInvoice";
                    if (!string.IsNullOrWhiteSpace(Model.UCPUType)) strSQL += " AND UCPUType=@UCPUType";
                    if (!string.IsNullOrWhiteSpace(Model.UBuyStore)) strSQL += " AND UBuyStore=@UBuyStore";
                    if (!string.IsNullOrWhiteSpace(Model.USeagate)) strSQL += " AND USeagate=@USeagate";
                    if (!string.IsNullOrWhiteSpace(Model.UTForce)) strSQL += " AND UTForce=@UTForce";
                    if (Model.CState != null) strSQL += " AND CState=@CState";
                    if (SDate != null) strSQL += $" AND InsertDateTime>='{SDate.Value.ToString("yyyy/MM/dd 00:00:00")}'";
                    if (EDate != null) strSQL += $" AND InsertDateTime<'{EDate.Value.AddDays(1).ToString("yyyy/MM/dd 00:00:00")}'";

                    return Json(dbConn.Query<AMD>(strSQL, Model).ToList(), JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new List<AMD>(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 審核
        /// </summary>
        /// <param name="Model">AMD</param>
        /// <returns></returns>
        public ActionResult Verify(AMD Model)
        {
            try
            {
                using (SqlConnection dbConn = new SqlConnection(_DBConn))
                {
                    string strSQL = "UPDATE AMD SET CState=@CState,CReason=@CReason,CAnotherReason=@CAnotherReason WHERE ID=@ID";

                    dbConn.Open();
                    using (SqlTransaction trade = dbConn.BeginTransaction())
                    {
                        dbConn.Execute(strSQL, Model, transaction: trade);
                        trade.Commit();
                    }

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 匯出
        /// </summary>
        /// <param name="Model">AMD</param>
        /// <param name="SDate">日期開始區間</param>
        /// <param name="EDate">日期結束區間</param>
        /// <returns></returns>
        public ActionResult Export(AMD Model, DateTime? SDate, DateTime? EDate)
        {
            try
            {
                using (SqlConnection dbConn = new SqlConnection(_DBConn))
                {
                    string strSQL = "SELECT * FROM AMD WHERE ID > 0 AND Type=@Type";
                    if (!string.IsNullOrWhiteSpace(Model.UName)) strSQL += " AND UName LIKE N'%'+@UName+'%'";
                    if (!string.IsNullOrWhiteSpace(Model.UPhone)) strSQL += " AND UPhone=@UPhone";
                    if (!string.IsNullOrWhiteSpace(Model.UEmail)) strSQL += " AND UEmail=@UEmail";
                    if (!string.IsNullOrWhiteSpace(Model.UInvoice)) strSQL += " AND UInvoice=@UInvoice";
                    if (!string.IsNullOrWhiteSpace(Model.UCPUType)) strSQL += " AND UCPUType=@UCPUType";
                    if (!string.IsNullOrWhiteSpace(Model.UBuyStore)) strSQL += " AND UBuyStore=@UBuyStore";
                    if (!string.IsNullOrWhiteSpace(Model.USeagate)) strSQL += " AND USeagate=@USeagate";
                    if (!string.IsNullOrWhiteSpace(Model.UTForce)) strSQL += " AND UTForce=@UTForce";
                    if (Model.CState != null) strSQL += " AND CState=@CState";
                    if (SDate != null) strSQL += $" AND InsertDateTime>='{SDate.Value.ToString("yyyy/MM/dd 00:00:00")}'";
                    if (EDate != null) strSQL += $" AND InsertDateTime<'{EDate.Value.AddDays(1).ToString("yyyy/MM/dd 00:00:00")}'";

                    List<AMD> result = dbConn.Query<AMD>(strSQL, Model).ToList();

                    string strGuid = Guid.NewGuid() + ".csv";

                    using (var file = new StreamWriter(Path.Combine(Server.MapPath("~/export"), strGuid), false, System.Text.Encoding.UTF8))
                    {
                        file.WriteLine($"姓名,連絡電話,E-mail,發票號碼,CPU序號,CPU型號,購買店家,Seagate SSD全系列,T-Force RGB系列記憶體,申請時間");
                        foreach (var item in result)
                        {
                            file.WriteLine($"{item.UName},=\"{item.UPhone}\",{item.UEmail},{item.UInvoice ?? "無"},{item.UCPUSN ?? "無"},{item.UCPUType ?? "無"},{item.UBuyStore ?? "無"},{item.USeagate ?? "無購買"},{item.UTForce ?? "無購買"},{item.InsertDateTime.ToString("yyyy/MM/dd HH:mm")}");
                        }
                    }

                    return Json("/export/" + strGuid, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}