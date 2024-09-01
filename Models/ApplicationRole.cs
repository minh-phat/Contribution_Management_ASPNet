﻿using Microsoft.AspNetCore.Identity;

namespace SchoolProject1640.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string? Descriptions { get; set; }
    }
}