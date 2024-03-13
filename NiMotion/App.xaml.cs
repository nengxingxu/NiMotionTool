using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace NiMotion
{

    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private Mutex singleInstanceMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            singleInstanceMutex = new Mutex(true, "YourUniqueMutexName", out createdNew);

            if (!createdNew)
            {
                // 如果互斥体已存在，说明已经有一个实例在运行
                // 在此可以处理重复实例的逻辑，例如激活已存在的窗口
                // 或显示一条消息
                MessageBox.Show("An instance of the application is already running.");
                Shutdown();
            }
            Language = string.IsNullOrEmpty(Language) ? "zh-CN" : Language;
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            singleInstanceMutex.ReleaseMutex();
            singleInstanceMutex.Close();
            base.OnExit(e);
        }
        public bool IsOpen { get; set; }

        private static string language;

        public static string Language
        {
            get { return language; }
            set
            {
                if (language != value)
                {
                    language = value;
                    List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
                    foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
                    {
                        dictionaryList.Add(dictionary);
                    }
                    string requestedLanguage = string.Format(@"Resource/Lang/Lang.{0}.xaml", Language);
                    ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedLanguage));
                    if (resourceDictionary == null)
                    {
                        requestedLanguage = @"Resource/Lang/Lang.en-US.xaml";
                        resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedLanguage));
                    }
                    if (resourceDictionary != null)
                    {
                        Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                        Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                    }
                }
            }
        }


    }
  
}
