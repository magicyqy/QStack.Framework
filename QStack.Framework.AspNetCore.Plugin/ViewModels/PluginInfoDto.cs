﻿using QStack.Framework.Basic.ViewModel;
using QStack.Framework.Core.Model;
using System;

namespace QStack.Framework.AspNetCore.Plugin.ViewModels
{
    public class PluginInfoDto : BaseDto
    {
       

        public virtual string UniqueKey { get; set; }

        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }

        public string Version { get; set; }

        public virtual bool IsEnable { get; set; }

        public virtual string TestUrl { get; set; }
        public virtual string IntallPath { get; set; }
        /// <summary>
        /// 路由区域名
        /// </summary>
        public virtual string RouteArea { get; set; }
        /// <summary>
        /// 是否使用运行时efcore migration<br/>
        /// 
        /// </summary>
        public virtual bool IsMigration { get; set; }
    }
}
