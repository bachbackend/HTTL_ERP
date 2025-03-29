using System;
using System.Collections.Generic;

namespace HTTL_ERP.DataAccess;

public partial class Logistic
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Phonenumber { get; set; } = null!;

    public short? Status { get; set; }

    public string? Comment { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();
}
