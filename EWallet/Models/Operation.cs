//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EWallet.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Operation
    {
        public int ID { get; set; }
        public int Number { get; set; }
        public System.DateTime Date { get; set; }
        public double Sum { get; set; }
        public int UserID { get; set; }
        public int ToUserID { get; set; }
        public int ServiceID { get; set; }
    
        public virtual Service Service { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
