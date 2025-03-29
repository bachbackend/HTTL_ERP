using System;
using System.Collections.Generic;

namespace HTTL_ERP.DataAccess;

public partial class District
{
    public int Id { get; set; }

    public int CityId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Agency> Agencies { get; set; } = new List<Agency>();

    public virtual ICollection<BrickFactory> BrickFactories { get; set; } = new List<BrickFactory>();

    public virtual City City { get; set; } = null!;
}
