using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Persistent.EFCore
{
    /// <summary>
    /// 生成migration的Up操作时不生成外键
    /// </summary>
    public class IgnoreFKCSharpMigrationOperationGenerator: CSharpMigrationOperationGenerator
    {
        public IgnoreFKCSharpMigrationOperationGenerator(CSharpMigrationOperationGeneratorDependencies dependencies):base(dependencies)
        {
           
        }

        //protected override void Generate(AddForeignKeyOperation operation, IndentedStringBuilder builder)
        //{
        //    builder.Append(".GetHashCode()");
        //    //base.Generate(operation, builder);
        //}
        //protected override void Generate(DropForeignKeyOperation operation, IndentedStringBuilder builder)
        //{
        //    builder.Append(".GetHashCode()");
        //    //base.Generate(operation, builder);
        //}
        public override void Generate(
            string builderName,
            IReadOnlyList<MigrationOperation> operations,
            IndentedStringBuilder builder)
        {
            var list = new List<MigrationOperation>();
            foreach (var operation in operations)
            {
                if (operation is AddForeignKeyOperation || operation is DropForeignKeyOperation)
                    continue;
                list.Add(operation);
            }
            base.Generate(builderName, list, builder);
           
        }
        protected override void Generate(CreateTableOperation operation,IndentedStringBuilder builder)
        {
            operation.ForeignKeys.Clear();
            base.Generate(operation, builder);
        }
    }
}
