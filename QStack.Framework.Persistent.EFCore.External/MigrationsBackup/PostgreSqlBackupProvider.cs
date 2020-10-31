using QStack.Framework.Core;
using QStack.Framework.Core.Persistent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.Persistent.EFCore.External.MigrationsBackup
{
    public class PostgreSqlBackupProvider : IBackupProvider
    {
        static string Set = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "set " : "export ";
        protected DaoFactoryOption _daoFactoryOption;
        protected readonly MigrationOptions _migrationOption;
        public PostgreSqlBackupProvider(DaoFactoryOption daoFactoryOption,MigrationOptions migrationOptions) 
        {
           
            _migrationOption = migrationOptions;
            _daoFactoryOption = daoFactoryOption;
        }
       
        public  async Task Backup()
        {
            var connectionInfo = GetInfoFromConnectionString();

            var outFile = Path.Combine(AppContext.BaseDirectory, _migrationOption.BackupBasePath, connectionInfo.DataBase, $"{connectionInfo.DataBase}_backup_{DateTimeOffset.Now.ToString("yyyyMMddHHmmss")}.dmp");
           
            var outDir = Directory.GetParent(outFile).FullName;
            if(!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            await PostgreSqlDump(outFile,
                connectionInfo.Host,
                connectionInfo.Port.ToString(),
                connectionInfo.DataBase,
                connectionInfo.UserID,
                connectionInfo.Password);
        } 

        private async Task PostgreSqlDump(
           string outFile,
           string host,
           string port,
           string database,
           string user,
           string password)
        {
            var pgDump = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"\"{_migrationOption.PgDumpPath.TrimEnd('\\')}\\pg_dump\"" : "pg_dump";
            var codePageCmd = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"chcp 65001{ Environment.NewLine}" : "";
            string dumpCommand =
                 $"{codePageCmd}{Set}PGPASSWORD={password}{Environment.NewLine}" +
                    pgDump + " -Fc" + " -h " + host + " -p " + port + " -d " + database + " -U " + user + "";
           
            string batchContent = "" + dumpCommand + " -f " + "\"" + outFile + "\"" + "\n";
            //在容器内安装 sudo apt install postgresql-client 为替换
            //if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
            //{
            //    //docker容器内运行宿主机命令nsenter
            //    //必须预先添加容器参数-v /proc:/host/proc --privileged
            //    batchContent = $"nsenter--mount =/host/proc/1/ns/mnt sh -c \"{batchContent}\"";
            //}
            if (File.Exists(outFile)) File.Delete(outFile);

            await Execute(batchContent);
        }
        private Task Execute(string dumpCommand)
        {
            return Task.Run(() =>
            {

                string batFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}." + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bat" : "sh"));
                try
                {
                    string batchContent = "";
                    batchContent += $"{dumpCommand}";
                    Debug.WriteLine($"backup command: {batchContent}");
                    File.WriteAllText(batFilePath, batchContent, new UTF8Encoding(false));

                    ProcessStartInfo info = ProcessInfoByOS(batFilePath);
                    Console.InputEncoding = new UTF8Encoding(false);
                    using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(info))
                    {
               
                
                        process.EnableRaisingEvents = true;
                        process.BeginOutputReadLine();

                        process.OutputDataReceived += (s, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine(e.Data);
                        };
                        using (StreamReader reader = process.StandardError)
                        {
                            string result = reader.ReadToEnd();
                            Console.WriteLine(result);

                        }
                        
                        process.WaitForExit();
                    }
                }
                catch (Exception e)
                {
                    throw new ServiceFrameworkException($"failed to backup.", e);

                }
                finally
                {
                    //if (File.Exists(batFilePath)) File.Delete(batFilePath);
                }
            });
        }

        private static ProcessStartInfo ProcessInfoByOS(string batFilePath)
        {
            ProcessStartInfo info;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                info = new ProcessStartInfo(batFilePath)
                {
                };
            }
            else
            {
                info = new ProcessStartInfo("sh")
                {
                    Arguments = $"{batFilePath}"
                };
            }
 
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            info.RedirectStandardOutput = true;//由调用程序获取输出信息
            info.RedirectStandardError = true;//重定向标准错误输出
            info.RedirectStandardInput = true;//接受来自调用程序的输入信息
         
            return info;
        }
        public (string Host, int Port, string DataBase, string UserID, string Password) GetInfoFromConnectionString()
        {

            var type= Type.GetType("Npgsql.NpgsqlConnectionStringBuilder,Npgsql");
            var constuctor=type.GetConstructor(new Type[] { typeof(string)});
            var connectionStringBuilder= constuctor.Invoke(new object[] { _daoFactoryOption.ConnectionString });
            var host = type.GetProperty("Host").GetValue(connectionStringBuilder)?.ToString();
            var ips=  Dns.GetHostAddresses(host);
            host = ips.Length > 0 ? ips[0].ToString() : host;
            var port=Convert.ToInt32(type.GetProperty("Port").GetValue(connectionStringBuilder));
            var dataBase = type.GetProperty("Database").GetValue(connectionStringBuilder)?.ToString();
            var userId = type.GetProperty("Username").GetValue(connectionStringBuilder)?.ToString();
            var password = type.GetProperty("Password").GetValue(connectionStringBuilder)?.ToString();
            return (host, port, dataBase, userId, password);
        }

        public async Task PostgreSqlRestore(
           string inputFile,
           string host,
           string port,
           string database,
           string user,
           string password)
        {
            string dumpCommand = $"{Set}PGPASSWORD={password}\n" +
                                 $"psql -h {host} -p {port} -U {user} -d {database} -c \"select pg_terminate_backend(pid) from pg_stat_activity where datname = '{database}'\"\n" +
                                 $"dropdb -h " + host + " -p " + port + " -U " + user + $" {database}\n" +
                                 $"createdb -h " + host + " -p " + port + " -U " + user + $" {database}\n" +
                                 "pg_restore -h " + host + " -p " + port + " -d " + database + " -U " + user + "";

            //psql command disconnect database
            //dropdb and createdb  remove database and create.
            //pg_restore restore database with file create with pg_dump command
            dumpCommand = $"{dumpCommand} {inputFile}";

            await Execute(dumpCommand);
        }
    }
}
