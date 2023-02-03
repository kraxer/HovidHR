using System;
using System.Collections.Generic;

namespace HovidHR.Models;

public partial class User
{
    public long UserId { get; set; }

    public string? UserName { get; set; }

    public int? UserNo { get; set; }

    public DateTime CreateDate { get; set; }
}
