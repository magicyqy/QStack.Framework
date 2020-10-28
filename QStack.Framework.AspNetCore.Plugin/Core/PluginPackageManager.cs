using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QStack.Framework.AspNetCore.Plugin.Contracts;
using QStack.Framework.AspNetCore.Plugin.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using ZipTool = System.IO.Compression.ZipArchive;
namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public class PluginPackageManager
    {
        IHostEnvironment _hostEnvironment;
        PluginOptions _pluginOptions;
        private readonly string _baseDirectory;
        public PluginPackageManager(IHostEnvironment environment, IOptions<PluginOptions> options)
        {
            _hostEnvironment = environment;
            _pluginOptions = options.Value;
            _baseDirectory = AppContext.BaseDirectory;
        }

        public bool UnZipPackage(string zipFile,out PluginInfoDto pluginInfoDto)
        {
            pluginInfoDto = null;
            using (FileStream fs = new FileStream(zipFile, FileMode.Open))
            {
                DirectoryInfo folder = null;
                try
                {
                    ZipTool archive = new ZipTool(fs, ZipArchiveMode.Read);
                    fs.Position = 0;
                    var pluginName = Path.GetFileNameWithoutExtension(zipFile);
                    var destFolder = Path.Combine(_baseDirectory, _pluginOptions.InstallBasePath, $"{pluginName}");


                    archive.ExtractToDirectory(destFolder, true);

                    folder = new DirectoryInfo(destFolder);

                    FileInfo[] files = folder.GetFiles();
                    var entryAssemblyFileName = Path.GetFileNameWithoutExtension(zipFile).ToLower() + ".dll";
                    FileInfo configFile = files.SingleOrDefault(p => p.Name.ToLower() == entryAssemblyFileName);

                    if (configFile == null)
                    {
                        throw new QStack.Framework.Core.ServiceFrameworkException("can not find the entry assembly.the package name must be same as the entry assembly.");
                    }
                    else
                    {
                        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(configFile.FullName);

                        pluginInfoDto = new PluginInfoDto
                        {
                            DisplayName = fileVersionInfo.FileDescription,
                            Version = fileVersionInfo.FileVersion,
                            Name = Path.GetFileNameWithoutExtension(fileVersionInfo.FileName)

                        };

                    }
                    pluginInfoDto.IntallPath = Path.GetRelativePath(_baseDirectory, destFolder);

                    return true;
                }
                catch
                {
                    folder?.Delete(true);
                    return false;
                }
               
            }

            
        }
    }
}
