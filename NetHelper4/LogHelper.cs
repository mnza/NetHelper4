using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetHelper4
{
    public class LogHelper
    {
        private string _fileName;

        public LogHelper(string filename)
        {
            this._fileName = filename;
        }

        public void Write(string log)
        {
            string directory = Path.GetDirectoryName(_fileName);

            if (string.IsNullOrEmpty(directory))
            {
                throw new Exception("文件路径异常，请检查文件路径是否正确！");
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                if (File.Exists(_fileName))
                {
                    fs = new FileStream(_fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    fs = new FileStream(_fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
                sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
                sw.WriteLine(string.Format("{0}:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), log));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }
    }
}
