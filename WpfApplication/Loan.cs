//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApplication
{
    using System;
    using System.Collections.Generic;
    
    public partial class Loan
    {
        public System.Guid Id { get; set; }
        public System.DateTime DueDate { get; set; }
        public Nullable<System.Guid> PatronId { get; set; }
        public Nullable<System.Guid> BookId { get; set; }
        public System.DateTime LoanDate { get; set; }
    
        public virtual Book Book { get; set; }
        public virtual Patron Patron { get; set; }
    }
}
