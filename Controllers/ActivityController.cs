using AMDng.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMDng.Controllers
{
    /// <summary>
    /// 活動
    /// </summary>
    public class ActivityController : Controller
    {
        readonly string _DBConn = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;

        /// <summary>
        /// 繪製首頁 (ng-view)
        /// </summary>
        /// <returns></returns>
        public ActionResult Draw()
        {
            return View();
        }

        /// <summary>
        /// 活動首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return PartialView();
        }

        /// <summary>
        /// XBox 兌換
        /// </summary>
        /// <returns></returns>
        public ActionResult XBox()
        {
            return PartialView();
        }

        /// <summary>
        /// Game Bundle 兌換
        /// </summary>
        /// <returns></returns>
        public ActionResult GB()
        {
            return PartialView();
        }

        /// <summary>
        /// 再香一次
        /// </summary>
        /// <returns></returns>
        public ActionResult Again()
        {
            return PartialView();
        }

        /// <summary>
        /// 開學信仰鼠不盡
        /// </summary>
        /// <returns></returns>
        public ActionResult Faith()
        {
            return PartialView();
        }

        /// <summary>
        /// 開學好禮鼠不盡
        /// </summary>
        /// <returns></returns>
        public ActionResult Gifts()
        {
            return PartialView();
        }

        /// <summary>
        /// 進度與查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult Progress()
        {
            return PartialView();
        }

        public ActionResult Query(AMD Model)
        {
            try
            {
                using (SqlConnection dbConn = new SqlConnection(_DBConn))
                {
                    string strSQL = "SELECT TOP 1 * FROM AMD WHERE ID>0 AND Type=@Type AND UName=@UName AND UPhone=@UPhone ORDER BY ID DESC";
                    return Json(dbConn.QueryFirstOrDefault<AMD>(strSQL, Model), JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new List<AMD>(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 申請兌換
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Apply(AMD Model, HttpPostedFileBase CPUFile, HttpPostedFileBase InvoiceFile)
        {
            try
            {
                if (CPUFile != null || InvoiceFile != null)
                {
                    if (CPUFile != null && CPUFile.ContentLength > 0)
                    {
                        string newFileName = "cpu" + DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();
                        string tempFileName = "tempcpu" + newFileName;
                        string extName = Path.GetExtension(CPUFile.FileName);
                        string tempPath = Path.Combine(Server.MapPath("~/upload"), tempFileName + extName);
                        string newFilePath = Path.Combine(Server.MapPath("~/upload"), newFileName + extName);
                        CPUFile.SaveAs(tempPath);
                        SimpleCompressImage(tempPath, Path.Combine(Server.MapPath("~/upload"), newFilePath));
                        System.IO.File.Delete(tempPath);
                        Model.UCPUPhoto = newFileName + extName;
                    }

                    if (InvoiceFile != null && InvoiceFile.ContentLength > 0)
                    {
                        string newFileName = "inv" + DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();
                        string tempFileName = "tempinv" + newFileName;
                        string extName = Path.GetExtension(InvoiceFile.FileName);
                        string tempPath = Path.Combine(Server.MapPath("~/upload"), tempFileName + extName);
                        string newFilePath = Path.Combine(Server.MapPath("~/upload"), newFileName + extName);
                        InvoiceFile.SaveAs(tempPath);
                        SimpleCompressImage(tempPath, Path.Combine(Server.MapPath("~/upload"), newFilePath));
                        System.IO.File.Delete(tempPath);
                        Model.UInvoicePhoto = newFileName + extName;
                    }
                    else return Json(false, JsonRequestBehavior.AllowGet);
                }
                //else
                //{
                //    return Json(false, JsonRequestBehavior.AllowGet);
                //}

                using (SqlConnection dbConn = new SqlConnection(_DBConn))
                {
                    Model.CState = 0;
                    Model.CReason = "其他";
                    Model.InsertDateTime = DateTime.Now;
                    Model.UInvoice = Model.UInvoice?.ToUpper();
                    string strSQL = "INSERT INTO AMD " +
                         "(UName, UPhone, UEmail, UAddress, UBuyStore, UInvoice," +
                         " UInvoicePhoto, UCPUSN, UCPUType, UCPUPhoto," +
                         " USeagate, UTForce, CState, Type, InsertDateTime)" +
                         " VALUES " +
                         "(@UName, @UPhone, @UEmail, @UAddress, @UBuyStore, @UInvoice," +
                         " @UInvoicePhoto, @UCPUSN, @UCPUType, @UCPUPhoto," +
                         " @USeagate, @UTForce, @CState, @Type, @InsertDateTime)";

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
        /// 等比壓縮圖片
        /// </summary>
        /// <param name="SourceFile">壓縮來源檔案路徑</param>
        /// <param name="SavePath">儲存檔案路徑</param>
        /// <returns>處理結果</returns>
        public static bool SimpleCompressImage(string SourceFile, string SavePath)
        {
            int iMaxPixel = 1024;
            int iFixWidth;
            int iFixHeight;
            Image image = Image.FromFile(SourceFile);
            if (image.Width > iMaxPixel || image.Height > iMaxPixel) // 如果圖片的寬大於最大值或高大於最大值就往下執行
            {
                // 圖片的寬大於圖片的高
                if (image.Width >= image.Height)
                {
                    iFixWidth = iMaxPixel; // 設定修改後的圖寬
                    iFixHeight = Convert.ToInt32((Convert.ToDouble(iFixWidth) / Convert.ToDouble(image.Width)) * Convert.ToDouble(image.Height)); // 設定修改後的圖高

                }
                else
                {
                    iFixHeight = iMaxPixel; // 設定修改後的圖高
                    iFixWidth = Convert.ToInt32((Convert.ToDouble(iFixHeight) / Convert.ToDouble(image.Height)) * Convert.ToDouble(image.Width)); // 設定修改後的圖寬
                }
            }
            else
            {
                // 圖片沒有超過設定值，不執行縮圖
                iFixHeight = image.Height;
                iFixWidth = image.Width;
            }
            Bitmap imageOutput = new Bitmap(image, iFixWidth, iFixHeight);

            try
            {
                //判斷資料夾是否存在
                string strTempPath = SavePath.Substring(0, SavePath.LastIndexOf(@"\"));
                bool boolIsExists = Directory.Exists(strTempPath);
                if (boolIsExists == false) { Directory.CreateDirectory(strTempPath); }
                string fileExtension = Path.GetExtension(SourceFile).ToLower();

                //按原图片类型保存缩略图片,不按原格式图片会出现模糊,锯齿等问题.  
                switch (fileExtension)
                {
                    case ".gif": imageOutput.Save(SavePath, ImageFormat.Gif); break;
                    case ".jpeg":
                    case ".jpg": SaveAsJPEG(imageOutput, SavePath, 95); break;
                    case ".bmp": imageOutput.Save(SavePath, ImageFormat.Bmp); break;
                    case ".png": imageOutput.Save(SavePath, ImageFormat.Png); break;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                imageOutput.Dispose();
                image.Dispose();
                GC.Collect();
            }
            return true;
        }

        /// <summary>
        /// 儲存圖片
        /// </summary>
        /// <param name="m_bmp">圖片文件</param>
        /// <param name="m_strFileName">儲存文件名</param>
        /// <param name="m_iQty">圖片品質</param>
        /// <returns>是否成功</returns>
        private static bool SaveAsJPEG(Bitmap m_bmp, string m_strFileName, int m_iQty)
        {
            try
            {
                EncoderParameter p;
                EncoderParameters ps;

                ps = new EncoderParameters(1);

                p = new EncoderParameter(Encoder.Quality, m_iQty);
                ps.Param[0] = p;

                m_bmp.Save(m_strFileName, GetCodecInfo("image/jpeg"), ps);

                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 取得 JPG, JPEG 檔案編碼
        /// </summary>
        /// <param name="MimeType">MimeType</param>
        /// <returns>得到指定 MimeType 的 ImageCodecInfo</returns>
        private static ImageCodecInfo GetCodecInfo(string MimeType)
        {
            ImageCodecInfo[] codecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in codecInfo)
            {
                if (ici.MimeType == MimeType) return ici;
            }
            return null;
        }
    }
}