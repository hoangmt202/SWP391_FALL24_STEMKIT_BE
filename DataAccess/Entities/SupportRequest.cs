﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class SupportRequest
{
    public int SupportId { get; set; }

    public int SupportInstance { get; set; }

    public int UserId { get; set; }

    public int OrderDetailId { get; set; }

    public virtual OrderDetail OrderDetail { get; set; }

    public virtual User User { get; set; }
}