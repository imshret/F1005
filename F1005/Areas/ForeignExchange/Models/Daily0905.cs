//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace F1005.Areas.ForeignExchange.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Daily0905
    {
        public int ID { get; set; }
        public string 幣別 { get; set; }
        public string 現金買入 { get; set; }
        public string 現金賣出 { get; set; }
        public string 即期買入 { get; set; }
        public string 即期賣出 { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    }
}