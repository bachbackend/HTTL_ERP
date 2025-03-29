using System;
using System.Collections.Generic;

namespace HTTL_ERP.DataAccess;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public short Role { get; set; }

    public DateOnly Dob { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? LoginTime { get; set; }

    public string? VerifyToken { get; set; }

    public DateTime? VerifyTokenExpired { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
