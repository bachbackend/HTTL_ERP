using System;
using System.Collections.Generic;

namespace HTTL_ERP.DataAccess;

public partial class Agency
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Location { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public string Phonenumber { get; set; } = null!;

    public string? Comment { get; set; }

    public short? SampleDelivery { get; set; }

    public short? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Type { get; set; }

    public int DistrictId { get; set; }

    public virtual District District { get; set; } = null!;

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();
}
