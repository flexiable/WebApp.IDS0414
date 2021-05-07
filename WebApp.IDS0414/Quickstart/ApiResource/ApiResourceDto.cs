﻿using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.IDS0414.Controllers.ApiResource
{
    public class ApiResourceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string UserClaims { get; set; }
        public string Scopes { get; set; } = string.Empty;
        public object StandardScopes { get; internal set; }
    }
}
