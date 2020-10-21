using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.Persistent.EFCore.External.MigrationsBackup
{
    public interface IBackupProvider
    {
        Task Backup();
    }
}
