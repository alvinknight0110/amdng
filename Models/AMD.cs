using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMDng.Models
{
    public class AMD
    {
        /// <summary>
        /// ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string UName { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        public string UPhone { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        public string UEmail { get; set; }

        /// <summary>
        /// 購買店家
        /// </summary>
        public string UBuyStore { get; set; }

        /// <summary>
        /// CPU 序號
        /// </summary>
        public string UCPUSN { get; set; }

        /// <summary>
        /// CPU 型號
        /// </summary>
        public string UCPUType { get; set; }

        /// <summary>
        /// CPU 照片檔案路徑
        /// </summary>
        public string UCPUPhoto { get; set; }

        /// <summary>
        /// 發票號碼
        /// </summary>
        public string UInvoice { get; set; }

        /// <summary>
        /// 發票照片檔案路徑
        /// </summary>
        public string UInvoicePhoto { get; set; }

        /// <summary>
        /// 希捷
        /// </summary>
        public string USeagate { get; set; }

        /// <summary>
        /// T-Force
        /// </summary>
        public string UTForce { get; set; }

        /// <summary>
        /// 處理狀態
        /// 0: 審核中, 1: 審核通過, 2: 審核未過
        /// </summary>
        public byte? CState { get; set; }

        /// <summary>
        /// 未過原因
        /// </summary>
        public string CReason { get; set; }

        /// <summary>
        /// 其他原因
        /// </summary>
        public string CAnotherReason { get; set; }

        /// <summary>
        /// 1: XBox, 2: Game Bundle
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 申請時間
        /// </summary>
        public DateTime InsertDateTime { get; set; }
    }
}