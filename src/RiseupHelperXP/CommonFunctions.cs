using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RiseupHelperXP
{
    public static class CommonFunctions
    {
        public static string SettingsPath = CommonFunctions.AddSlash(
            Application.StartupPath)+"data\\";
        public static string NetSettingsFile = "network.xml";
        public static string ProvidersFile = "providers.txt";
        public static List<string> ProvidersList = new List<string>();

        public static bool Preload()
        {
            if (!Directory.Exists(SettingsPath))
            {
                try
                {
                    Directory.CreateDirectory(SettingsPath);
                }
                catch (Exception ex)
                {
                    ErrMessage(ex.Message);
                    return false;
                }
            }

            if (!File.Exists(SettingsPath + ProvidersFile))
            {
                try
                {
                    File.WriteAllText(SettingsPath + ProvidersFile,
                        Properties.Resources.providers);
                }
                catch (Exception ex)
                {
                    ErrMessage(ex.Message);
                    return false;
                }
            }

            string[] buf = null;
            try
            {
                buf = File.ReadAllLines(SettingsPath + ProvidersFile);
            }
            catch (Exception ex)
            {
                ErrMessage(ex.Message);
                return false;
            }

            ProvidersList.Clear();
            foreach (string s in buf)
            {
                if (s.Trim() != string.Empty)
                {
                    ProvidersList.Add(s.Trim());
                }
            }

            if (ProvidersList.Count == 0)
            {
                ErrMessage("Провайдеры не обнаружены \n" +
                    "Добавьте хотя-бы одного провайдера в файл " +
                    SettingsPath + ProvidersFile + "\n" +
                    "По адресу (без https:\\\\, http:\\\\ и www) на строку\n\n" +
                    "Например:\n" +
                    "calyx.net\n" +
                    "riseup.net");
                return false;
            }
            
            return true;
        }

        public static string AddSlash(string st)
        {
            if (st.EndsWith("\\"))
            {
                return st;
            }

            return st + "\\";
        }
        
        public static void ErrMessage(string stMessage)
        {
            MessageBox.Show(stMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
