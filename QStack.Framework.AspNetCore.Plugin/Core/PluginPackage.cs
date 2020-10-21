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
    public class PluginPackage
    {
       
        private Stream _zipStream = null;
        private string _tempFolderName = string.Empty;
        private string _folderName = string.Empty;
        private string _packagePath = string.Empty;
        private PluginInfoDto _pluginInfo;

        public PluginInfoDto PluginInfo => _pluginInfo;
        private readonly PluginOptions _pluginOptions;

        public PluginPackage(Stream stream,string packagePath,PluginOptions pluginOptions)
        {
            _zipStream = stream;
            _packagePath = packagePath;
            _pluginOptions = pluginOptions;
            Initialize(stream);
        }
       

        public void Initialize(Stream stream)
        {
            _zipStream = stream;
            _tempFolderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{ Guid.NewGuid().ToString()}");
            ZipTool archive = new ZipTool(_zipStream, ZipArchiveMode.Read);

            archive.ExtractToDirectory(_tempFolderName);

            DirectoryInfo folder = new DirectoryInfo(_tempFolderName);

            FileInfo[] files = folder.GetFiles();
            var entryAssemblyFileName = Path.GetFileNameWithoutExtension(_packagePath).ToLower() + ".dll";
            FileInfo configFile = files.SingleOrDefault(p => p.Name.ToLower() == entryAssemblyFileName);

            if (configFile == null)
            {
                throw new QStack.Framework.Core.ServiceFrameworkException("can not find the entry assembly.the package name must be same as the entry assembly.");
            }
            else
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(configFile.FullName);

                _pluginInfo = new PluginInfoDto
                {
                    DisplayName = fileVersionInfo.FileDescription,
                    Version = fileVersionInfo.FileVersion,
                    Name = Path.GetFileNameWithoutExtension(fileVersionInfo.FileName)

                };
                
            }
        }

        public void SetupFolder()
        {
            ZipTool archive = new ZipTool(_zipStream, ZipArchiveMode.Read);
            _zipStream.Position = 0;
            _folderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _pluginOptions.InstallBasePath, $"{_pluginInfo.Name}");

            archive.ExtractToDirectory(_folderName, true);
            _pluginInfo.IntallPath =Path.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, _folderName);
            DirectoryInfo folder = new DirectoryInfo(_tempFolderName);
            folder.Delete(true);
        }

        
    }
}
