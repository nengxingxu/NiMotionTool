using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;

namespace NiMotion.Common
{
    public class IniFileHelper
    {

        public static string GetIniPath(string strRes)
        {
            string strAppPath = AppDomain.CurrentDomain.BaseDirectory;
            string strFile = strAppPath +"\\" + strRes;
            if (File.Exists(strFile))
                return strFile;             
            else
                throw new Exception("Find resurce error" + strRes);
        }

        public static string IniValue(string Section, string IniKey)
        {

            string iniFileName= GetIniPath("NiMotion.ini");
          
            StringBuilder temp = new StringBuilder(500);

            int i = GetPrivateProfileString(Section, IniKey, "", temp, 500, iniFileName);
            return temp.ToString();
        }

        public static string IniValue(string Section, string IniKey,string def)
        {
            ;
            string iniFileName = GetIniPath("NiMotion.ini");

            StringBuilder temp = new StringBuilder(500);
  
            int i = GetPrivateProfileString(Section, IniKey, def, temp, 500, iniFileName);
            return temp.ToString();
        }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        public static long WriteValue(string Section, string IniKey, string NewValue)
        {
            //string Section = "MotorParam";
            string iniFileName = GetIniPath("NiMotion.ini");

            long i = WritePrivateProfileString(Section, IniKey, NewValue, iniFileName);
            return i;
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(
            string section, string key, string val, string filePath);
    }
}