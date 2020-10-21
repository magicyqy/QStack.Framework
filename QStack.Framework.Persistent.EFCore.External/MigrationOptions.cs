using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Persistent.EFCore.External
{
    public class MigrationOptions
    {
        /// <summary>
        /// 生成migration的路径
        /// </summary>
        public string MigrationPath { get; set; } = "Migrations";

        public bool IsBackup { get; } = true;

        
        public string BackupBasePath { get; set; } = "MigrationsBackup";

        public string PgDumpPath { get; set; }=string.Empty;
    }
}
