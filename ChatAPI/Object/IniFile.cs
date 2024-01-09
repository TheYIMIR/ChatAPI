using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatAPI.Object
{
    public class IniFile
    {
        private readonly string filePath;

        /// <summary>
        /// IniFile Object.
        /// </summary>
        /// <param name="filePath"></param>
        public IniFile(string filePath)
        {
            this.filePath = filePath;

            if (!File.Exists(this.filePath))
            {
                CreateDeafaultIniFile();
            }
        }

        /// <summary>
        /// Get data from a specific section.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetSection(string section)
        {
            Dictionary<string, string> sectionValues = new Dictionary<string, string>();
            bool isInTargetSection = false;

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (line.Trim().StartsWith($"[{section}]"))
                {
                    isInTargetSection = true;
                    continue;
                }

                if (isInTargetSection)
                {
                    if (Regex.Match(line, @"^\[.*\]$").Success)
                    {
                        isInTargetSection = false;
                        break;
                    }

                    var keyValueMatch = Regex.Match(line, @"^\s*([^=]+)\s*=\s*([^;]+)");
                    if (keyValueMatch.Success)
                    {
                        string key = keyValueMatch.Groups[1].Value.Trim();
                        string value = keyValueMatch.Groups[2].Value.Trim();
                        sectionValues[key] = value;
                    }
                }
            }

            return sectionValues;
        }

        private void CreateDeafaultIniFile()
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("[Server]");
                sw.WriteLine("IP=127.0.0.1");
                sw.WriteLine("Port=8888");
            }
        }
    }
}