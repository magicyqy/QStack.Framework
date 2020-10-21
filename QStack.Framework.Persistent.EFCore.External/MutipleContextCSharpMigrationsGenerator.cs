using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Persistent.EFCore.External
{
    /// <summary>
    /// 支持多上下同时migration时，可能生成的多个modelsnapshot类命名空间会相同
    /// 原因：某个上下文生成时，会在migractionassebmly中查找上一次的
    /// modelsnapshot类所在的命名空间，而当migractionassebmly为空null时，就在当前运行程序集查找，
    /// 而如果当前程序已存在另一个dbcontext生成的modelsnapshot，就会导致前者生成命名空间错误
    /// </summary>
    public class MutipleContextCSharpMigrationsGenerator: CSharpMigrationsGenerator
    {
        public MutipleContextCSharpMigrationsGenerator(MigrationsCodeGeneratorDependencies dependencies,
            CSharpMigrationsGeneratorDependencies csharpDependencies):base(dependencies, csharpDependencies)
        {

        }

        public string ModelSnapshotNamespace { get; set; }
        public override string GenerateSnapshot(
             string modelSnapshotNamespace,
             Type contextType,
             string modelSnapshotName,
             IModel model)
        {
            if (!string.IsNullOrWhiteSpace(ModelSnapshotNamespace))
                modelSnapshotNamespace = ModelSnapshotNamespace;
            return base.GenerateSnapshot(modelSnapshotNamespace, contextType, modelSnapshotName, model);
        }
    }
}
