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
    
    public partial class Passport
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public int SerialNumber { get; set; }
        public int Number { get; set; }
        public int DivisionCode { get; set; }
        public int UserID { get; set; }
    
        public virtual User User { get; set; }
    }
}
