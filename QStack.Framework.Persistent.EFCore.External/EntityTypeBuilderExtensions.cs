using System.Collections.Generic;
using System.Linq;

using MicroFx.EFCore.RemoveForeignKey;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<T> RemoveForeignKey<T>(this EntityTypeBuilder<T> builder, string name) where T : class
        {
            var tableName = builder.Metadata.FindAnnotation("Relational:TableName").Value.ToString();
            RemoveForeignKeysHelper.RemoveForeignKeys.AddOrUpdate(tableName, new List<string> { name }, (value, values) => {
                values.Add(name);
                return values.Distinct().ToList();
            });
            return builder;
        }
    }
}