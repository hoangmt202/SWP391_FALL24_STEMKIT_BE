﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public DateOnly CreatedDate { get; set; }

    public string Status { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User User { get; set; }
}