using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOperations
{
    public class DataFiles
    {
        public string DirectoryPath { get; set; }
        public List<string> FileNames { get; set; }

        public DataFiles()
        {
            DirectoryPath = string.Empty;
            FileNames = new List<string>();
        }
        ~DataFiles()
        {
            DirectoryPath = string.Empty;
            FileNames.Clear();
            FileNames = null;
        }

        public void setDirectoryPath(string path)
        {
            DirectoryPath = path;
        }

        public string getDirectoryPath()
        {
            return DirectoryPath;
        }

        public void setFileNames(List<string> files)
        {
            FileNames = files;
        }
        public List<string> getFileNames()
        {
            return FileNames;
        }
        public DataFiles LoadConfiguration(string pat)
        {
            DataFiles data = null;
            string sJsonString = string.Empty;
            if (!string.IsNullOrEmpty(pat) && File.Exists(pat))
            {
                sJsonString = File.ReadAllText(pat);
                if (!string.IsNullOrEmpty(sJsonString))
                {
                    data = System.Text.Json.JsonSerializer.Deserialize<DataFiles>(sJsonString);
                }
            }
            return data;
        }
        public void SaveeConfiguration(DataFiles data, string path)
        {
            string sJsonString = string.Empty;
            if (data != null && !string.IsNullOrEmpty(path))
            {
                sJsonString = System.Text.Json.JsonSerializer.Serialize(data);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                File.WriteAllText(path, sJsonString);
            }
        }
    }
}
