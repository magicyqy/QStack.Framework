﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Web.Areas.Api.Models
{
    public class RolePermission
    {
        public int RoleId { get; set; }

        public int[] FunctionIds { get; set; }
    }
}
