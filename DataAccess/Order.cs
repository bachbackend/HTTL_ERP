using System;
using System.Collections.Generic;

namespace HTTL_ERP.DataAccess;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime DateDelivery { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal? Revenue { get; set; }

    public decimal BasePrice { get; set; }

    public decimal SellPrice { get; set; }

    public decimal? AdditionPrice { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual User User { get; set; } = null!;
}
