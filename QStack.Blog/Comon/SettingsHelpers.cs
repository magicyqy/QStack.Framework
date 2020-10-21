using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.Comon
{
    /// <summary>
    /// 更新appsettings.json
    /// </summary>
    public static class SettingsHelpers
    {
        /// <summary>
        /// 保存设置更改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionPathKey">键路径，格式:a:a_1:a_1_1</param>
        /// <param name="value">要保存的值</param>
        /// <param name="settingFile">设置文件，默认为根目录下的appsettings.json</param>
        public static void AddOrUpdateAppSetting<T>(string sectionPathKey, T value,string settingFile= null)
        {
            try
            {
                var filePath = settingFile?? Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                    SetValueRecursively(sectionPathKey, jsonObj, value);

                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(filePath, output);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing app settings | {0}", ex.Message);
            }
        }

        private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value)
        {
            // split the string at the first ':' character
            var remainingSections = sectionPathKey.Split(":", 2);

            var currentSection = remainingSections[0];
            if (remainingSections.Length > 1)
            {
                // continue with the procress, moving down the tree
                var nextSection = remainingSections[1];
                jsonObj[currentSection] ??= new JObject();
                SetValueRecursively(nextSection, jsonObj[currentSection], value);
            }
            else
            {
                // we've got to the end of the tree, set the value
                jsonObj[currentSection] = value;
            }
        }
    }
}
