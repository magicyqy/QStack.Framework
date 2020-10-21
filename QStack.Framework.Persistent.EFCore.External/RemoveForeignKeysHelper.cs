using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace MicroFx.EFCore.RemoveForeignKey
{
    public class RemoveForeignKeysHelper
    {
        internal static ConcurrentDictionary<string, List<string>> RemoveForeignKeys = new ConcurrentDictionary<string, List<string>>();


        public static void ExecuForeignKeys(CreateTableOperation operation)
        {
            if (RemoveForeignKeys.TryGetValue(operation.Name, out List<string> columns))
            {
                operation.ForeignKeys
                    .Where(item => item.Columns.Intersect(columns).Count() > 0)
                    .ToList()
                    .ForEach(item => operation.ForeignKeys.Remove(item));
            }
        }
    }
}