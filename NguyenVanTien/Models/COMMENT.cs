//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NguyenVanTien.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class COMMENT
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string MaSach { get; set; }
        public Nullable<int> MaKH { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> Rate { get; set; }
    
        public virtual KHACHHANG KHACHHANG { get; set; }
    }
}
