using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace RiseupHelperXP
{
    #region add_classes
    public enum WorkerStatus
    {
        Start = 0,
        Process = 1,
        Complete = 2,
        NotComplete = 3,
        FatalError = 4,
        CompleteAll = 5,
        Wait=6
    }
    public class WorkerStatusEventArgs : EventArgs
    {
        public string Message { get; set; }
        public WorkerStatus Status { get; set; }
    }
    #endregion
    public class MainWorker
    {
        public delegate void OnStatusChanged(object sender, WorkerStatusEventArgs e);

        public event OnStatusChanged StatusChanged;

        private SendRequest Downloader = null;
        private NetSettings netSettings = null;
        
        private string JSONFile = "provider.json";
        private string CACertFile = "cacert.pem";
        private string UserKeyFile = "openvpn.pem";
        
        private string APIURI = "";
        private string APIVer = "";
        private string APIBase = "";
        private string CACertURI = "";
        private string CACertFingerprint = "";
        private string CACertAlg = "";

        private WorkerStatus CurrentStatus = WorkerStatus.Process;

        public string Workdir { get; private set; }
        public string VPNProvider { get; private set; }        

        public MainWorker(string vpnprovider)
        {
            VPNProvider = vpnprovider;
            Workdir = CommonFunctions.SettingsPath + VPNProvider + "\\";
        }

        private void StatusChange(WorkerStatus Status, string stMessage)
        {
            WorkerStatusEventArgs e = new WorkerStatusEventArgs();
            e.Status = Status;
            e.Message = stMessage;
            CurrentStatus = Status;
            if (StatusChanged != null) StatusChanged(this, e);
        }        

        public bool WorkerInit()
        {
            netSettings = new NetSettings(CommonFunctions.SettingsPath +
                CommonFunctions.NetSettingsFile);            

            if (netSettings.LoadConfig()!=NetConfigStatus.OK)
            {
                StatusChange(WorkerStatus.FatalError, netSettings.ConfigError);
                return false;
            }            

            try
            {
                Directory.CreateDirectory(Workdir);
            }
            catch (Exception ex)
            {
                StatusChange(WorkerStatus.FatalError, ex.Message);
                return false;
            }

            Downloader = new SendRequest("https://"+VPNProvider+"/"+JSONFile,
                Workdir + JSONFile);
            Downloader.ConnectionTimeout = netSettings.ConnectionTimeout;
            Downloader.ConnectionType = netSettings.ConnectionType;
            Downloader.ProxyAddress = netSettings.ProxyAddress;
            Downloader.ProxyPassword = netSettings.ProxyPassword;
            Downloader.ProxyPort = netSettings.ProxyPort;
            Downloader.ProxyUser = netSettings.ProxyUser;

            return true;
        }

        private string GetJSONValue(JObject JSONObj, string ParamName)
        {
            JToken tok;

            bool res = JSONObj.TryGetValue(ParamName,out tok);
            if (res)
            {
                return tok.ToString();
            }
            else return null;            
        }

        private bool LoadJSONData()
        {
            StatusChange(WorkerStatus.Process, "Обработка данных...");
            
            string JSONBuf = "";
            try
            {
                JSONBuf = File.ReadAllText(Workdir + JSONFile);
            }
            catch (Exception ex)
            {
                StatusChange(WorkerStatus.FatalError, ex.Message);
                return false;
            }

            JObject JSONObj = null;
            try
            {
                JSONObj = JObject.Parse(JSONBuf);
            }
            catch (Exception ex)
            {
                StatusChange(WorkerStatus.FatalError, ex.Message);
            }
            
            APIURI = GetJSONValue(JSONObj, "api_uri");
            APIVer = GetJSONValue(JSONObj, "api_version");
            APIBase = APIURI+"/"+APIVer;
            CACertURI = GetJSONValue(JSONObj, "ca_cert_uri");
            CACertFingerprint = GetJSONValue(JSONObj, "ca_cert_fingerprint");

            if (!string.IsNullOrEmpty(CACertFingerprint))
            {
                int startfp = CACertFingerprint.IndexOf(':');
                CACertAlg = CACertFingerprint.Substring(0, startfp);
                CACertFingerprint = CACertFingerprint.Substring(startfp + 1).Trim();                
            }
            else
            {
                StatusChange(WorkerStatus.NotComplete, 
                    "Невозможно получить отпечаток сертификата!");
            }

            StatusChange(WorkerStatus.Complete, "API URI: " + APIURI);
            StatusChange(WorkerStatus.Complete, "API Version: " + APIVer);
            StatusChange(WorkerStatus.Complete, "API Base: " + APIBase);
            StatusChange(WorkerStatus.Complete, "Certificate URI: " + CACertURI);
            StatusChange(WorkerStatus.Complete, "Fingerprint: " + CACertFingerprint);
            StatusChange(WorkerStatus.Complete, "Algorithm: " + CACertAlg);

            if (string.IsNullOrEmpty(APIURI) || string.IsNullOrEmpty(APIVer) ||
                string.IsNullOrEmpty(CACertURI))
            {
                StatusChange(WorkerStatus.FatalError, "Отсутствуют важные данные:");
                StatusChange(WorkerStatus.FatalError, "[API URI, API Version, Certificate URI]");
                return false;
            }

            return true;
        }

        private string GetCertFingerprint(string CertFile)
        {
            if (!File.Exists(CertFile))
            {
                StatusChange(WorkerStatus.NotComplete,
                    "Сертификат " + CertFile + " не найден!");
                return null;
            }

            X509Certificate X509 = null;
            try
            {
                X509 = new X509Certificate(CertFile);
            }
            catch (Exception ex)
            {
                StatusChange(WorkerStatus.NotComplete,
                    ex.Message);
                return null;
            }

            return X509.GetCertHashString();
        }

        private string GetCertFingerprint(string CertFile, string AlgName)
        {
            if (!File.Exists(CertFile))
            {
                StatusChange(WorkerStatus.NotComplete,
                    "Сертификат " + CertFile + " не найден!");
                return null;
            }

            X509Certificate2 X509 = null;
            try
            {
                X509 = new X509Certificate2(CertFile);
            }
            catch (Exception ex)
            {
                StatusChange(WorkerStatus.NotComplete,
                    ex.Message);
                return null;
            }

            byte[] cert  = X509.GetRawCertData();

            HashAlgorithm alg = null;
            switch (AlgName.ToUpperInvariant())
            {
                case "MD5": alg = MD5.Create(); break;
                case "SHA1": alg = SHA1.Create(); break;
                case "SHA256": alg = SHA256.Create(); break;
                case "SHA384": alg = SHA384.Create(); break;
                case "SHA512": alg = SHA512.Create(); break;
                default:
                    {
                        StatusChange(WorkerStatus.NotComplete,
                            "Неизвестный алгоритм (" + AlgName + ")");
                        return null;
                    }
            }

            byte[] hash = alg.ComputeHash(cert);
            string hex = BitConverter.ToString(hash).ToLowerInvariant().
                Replace("-","");

            return hex;
            
        }

        private bool CheckCertFingerprint()
        {
            //проверка сертификата
            StatusChange(WorkerStatus.Process, "Проверка сертификата...");
            string certfp = GetCertFingerprint(Workdir + CACertFile, CACertAlg);
            StatusChange(WorkerStatus.Complete, "File fingerprint: " + certfp);

            if (string.IsNullOrEmpty(CACertFingerprint) || string.IsNullOrEmpty(certfp))
            {
                StatusChange(WorkerStatus.NotComplete, "Не удалось проверить отпечаток сертификата!");
                return false;
            }
            else
            {
                if (CACertFingerprint == certfp)
                {
                    StatusChange(WorkerStatus.Complete, "Fingerprint checked...OK");
                    return true;
                }
                else
                {
                    StatusChange(WorkerStatus.NotComplete, "Отпечаток сертификата не совпадает с данными провайдера!");
                    return false;
                }
            }
        }

        private void GetProviderKey()
        {
            StatusChange(WorkerStatus.Start, "Получаю данные VPN-провайдера...");
            //получаем provider.json
            StatusChange(WorkerStatus.Process, "Загрузка: " + 
                "https://" + VPNProvider + "/" + JSONFile);
            if (!Downloader.CreateRequest())
            {
                StatusChange(WorkerStatus.FatalError, Downloader.ErrorMessage);
                return;
            }
            else
            {
                if (!Downloader.Send())
                {
                    StatusChange(WorkerStatus.FatalError, Downloader.ErrorMessage);
                    return;
                }
                else
                {
                    StatusChange(WorkerStatus.Complete, "OK");
                }
            }
            
            if (!LoadJSONData()) return;

            //получаем сертификат провайдера
            StatusChange(WorkerStatus.Process, "Загрузка: " +
                CACertURI);
            Downloader.URL = CACertURI;
            Downloader.OutputFile = Workdir + CACertFile;
            if (!Downloader.CreateRequest())
            {
                StatusChange(WorkerStatus.FatalError, Downloader.ErrorMessage);
                return;
            }
            else
            {
                if (!Downloader.Send())
                {
                    StatusChange(WorkerStatus.FatalError, Downloader.ErrorMessage);
                    return;
                }
                else
                {
                    StatusChange(WorkerStatus.Complete, "OK");
                }
            }

            CheckCertFingerprint();

            StatusChange(WorkerStatus.CompleteAll, "Сертификат провайдера получен!");
        }

        private void GetUserKey()
        {
            StatusChange(WorkerStatus.Start, "Проверка данных провайдера...");

            //смотрим, есть ли provider.json
            //есть - загружаем данные, нет, запускаем загрузку данных провайдера
            if (File.Exists(Workdir + JSONFile))
            {
                StatusChange(WorkerStatus.Process, JSONFile + " найден.");
                if (!LoadJSONData())
                {
                    GetProviderKey();
                    if (CurrentStatus == WorkerStatus.FatalError) return;
                }
                else
                {
                    //смотрим, есть ли сертификат провайдера
                    //есть - проверяем fingerprint, 
                    //нет - загружаем данные провайдера
                    if (File.Exists(Workdir + CACertFile))
                    {
                        StatusChange(WorkerStatus.Process, CACertFile + " найден.");

                        if (!CheckCertFingerprint())
                        {
                            StatusChange(WorkerStatus.NotComplete, 
                                "Ошибка проверки сертификата! " +
                            "Пытаюсь обновить сертификат провайдера.");
                            GetProviderKey();
                            if (CurrentStatus == WorkerStatus.FatalError) return;
                        }
                    }
                    else
                    {
                        StatusChange(WorkerStatus.Process, CACertFile + " не найден.");
                        GetProviderKey();
                        if (CurrentStatus == WorkerStatus.FatalError) return;
                    }
                }
            }
            else
            {
                StatusChange(WorkerStatus.Process, JSONFile + " не найден.");
                GetProviderKey();
                if (CurrentStatus == WorkerStatus.FatalError) return;
            }

            StatusChange(WorkerStatus.Start, "Получаю ключ пользователя...");
            //получаем ключ пользователя
            StatusChange(WorkerStatus.Process, "Загрузка: " +
                APIBase + "/cert");
            Downloader.URL = APIBase + "/cert";
            Downloader.OutputFile = Workdir + UserKeyFile;
            Downloader.Method = "POST";
            Downloader.EnableIgnoreCertError();

            if (!Downloader.CreateRequest())
            {
                StatusChange(WorkerStatus.FatalError, Downloader.ErrorMessage);
                return;
            }
            else
            {
                if (!Downloader.Send())
                {
                    StatusChange(WorkerStatus.FatalError, Downloader.ErrorMessage);
                    return;
                }
                else
                {
                    StatusChange(WorkerStatus.Complete, "OK");
                }
            }
            Downloader.DisableIgnoreCertError();

            StatusChange(WorkerStatus.CompleteAll, "Ключ пользователя получен!");
        }

        private void DeleteDir(string Dir)
        {
            if (!Directory.Exists(Dir)) return;

            string[] files = Directory.GetFiles(Dir, "*.*", 
                SearchOption.AllDirectories);
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                    StatusChange(WorkerStatus.Complete, 
                        file+" удален.");
                }
                catch (Exception ex)
                {
                    StatusChange(WorkerStatus.NotComplete,
                        ex.Message);
                }
            }

            string[] dirs = Directory.GetDirectories(Dir, "*.*", 
                SearchOption.AllDirectories);
            foreach (string dir in dirs)
            {
                try
                {
                    Directory.Delete(dir);
                    StatusChange(WorkerStatus.Complete,
                        dir + " удалена.");
                }
                catch (Exception ex)
                {
                    StatusChange(WorkerStatus.NotComplete,
                        ex.Message);
                }
            }

            try
            {
                if (Directory.Exists(Dir))
                {
                    Directory.Delete(Dir);
                    StatusChange(WorkerStatus.Complete,
                        Dir + " удалена.");
                }
            }
            catch (Exception ex)
            {
                StatusChange(WorkerStatus.NotComplete,
                    ex.Message);
            }
            
        }

        private void RemoveWorkdir()
        {
            StatusChange(WorkerStatus.Start, "Удаление " + Workdir);
            DeleteDir(Workdir);
            StatusChange(WorkerStatus.CompleteAll, "Завершено.");
        }

        private void RemoveAllProviderData()
        {
            StatusChange(WorkerStatus.Start, "Удаление всех данных...");
            foreach (string provider in CommonFunctions.ProvidersList)
            {
                string Dir = CommonFunctions.SettingsPath + provider + "\\";
                if (Directory.Exists(Dir))
                {
                    DeleteDir(Dir);
                }
            }
            StatusChange(WorkerStatus.CompleteAll, "Завершено.");
        }
        
        public void StartGetProviderKey()
        {
            System.Threading.Thread workThread =
                new System.Threading.Thread(GetProviderKey);
            workThread.Start();
        }

        public void StartGetUserKey()
        {
            System.Threading.Thread workThread =
                new System.Threading.Thread(GetUserKey);
            workThread.Start();
        }

        public void StartRemoveWorkdir()
        {
            System.Threading.Thread workThread =
                new System.Threading.Thread(RemoveWorkdir);
            workThread.Start();
        }

        public void StartRemoveAllProviderData()
        {
            System.Threading.Thread workThread =
                new System.Threading.Thread(RemoveAllProviderData);
            workThread.Start();
        }
    }
}
