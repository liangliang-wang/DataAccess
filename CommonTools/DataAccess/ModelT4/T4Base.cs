using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ModelT4
{
    public class T4Base
    {
        /// <summary> 只要生成的表</summary>
		public string OnlyTable = string.Empty;

        public T4Base(string templateFilePath, string connectionName)
        {
            string directoryName = Path.GetDirectoryName(templateFilePath);
            string exeConfigFilename = Directory.GetFiles(directoryName, "*.config").FirstOrDefault<string>() ?? Directory.GetParent(directoryName).GetFiles("*.config").FirstOrDefault<FileInfo>().FullName;

        }
    }
}
