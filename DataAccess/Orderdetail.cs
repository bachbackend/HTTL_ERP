using System;
using System.Collections.Generic;

namespace HTTL_ERP.DataAccess;

public partial class Orderdetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int AgencyId { get; set; }

    public int BrickFactoryid { get; set; }

    public int LogisticId { get; set; }

    public virtual Agency Agency { get; set; } = null!;

    public virtual BrickFactory BrickFactory { get; set; } = null!;

    public virtual Logistic Logistic { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
