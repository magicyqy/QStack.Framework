using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Persistent.EFCore
{
    /// <summary>
    /// 生成migration的Snapshot时不生成外键
    /// </summary>
    public class IgnoreFKCSharpSnapshotGenerator : CSharpSnapshotGenerator
    {
        public IgnoreFKCSharpSnapshotGenerator(CSharpSnapshotGeneratorDependencies dependencies):base(dependencies)
        {
            
        }

        protected override void GenerateForeignKey(string builderName, IForeignKey foreignKey, IndentedStringBuilder stringBuilder)
        {
            //base.GenerateForeignKey(builderName, foreignKey, stringBuilder);
        }
    }
}
