using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace NiMotion
{

    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Language = string.IsNullOrEmpty(Language) ? "zh-CN" : Language;
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
