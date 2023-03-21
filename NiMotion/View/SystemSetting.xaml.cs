using HandyControl.Controls;
using NiMotion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NiMotion.Common;

namespace NiMotion.View
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSetting
    {
        public delegate void UpdateLanguage(string language);
        public event UpdateLanguage UpdateLanguageEvent;
        public delegate void UpdateTheme(string theme);
        public event UpdateTheme UpdateThemeEvent;

        public SystemSetting()
        {
            InitializeComponent();

            SystemSettingModel = new SystemSettingPropertyModel
            {
                EnableLog = true
                
            };
            DataContext = this;
            ReadIni();
        }

        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            "SystemSettingModel", typeof(SystemSettingPropertyModel), typeof(PropertyGrid), new PropertyMetadata(default(SystemSettingPropertyModel)));

        public SystemSettingPropertyModel SystemSettingModel
        {
            get => (SystemSettingPropertyModel)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            UpdateSettings();
        }

        private void WriteIni()
        {
            IniFileHelper.WriteValue("sys", "EnableLog", SystemSettingModel.EnableLog ? "1" : "0");
            IniFileHelper.WriteValue("sys", "RefreshFrequency", SystemSettingModel.RefreshFrequency.ToString());
            IniFileHelper.WriteValue("sys", "Language", SystemSettingModel.Language == SystemSettingPropertyModel.language.Chinese ? "Chinese" : "English");
            IniFileHelper.WriteValue("sys", "Theme", SystemSettingModel.Theme == SystemSettingPropertyModel.theme.White ? "White" : "Dark");
        }

        private void ReadIni()
        {
            SystemSettingModel.EnableLog = IniFileHelper.IniValue("sys", "EnableLog","0") =="0" ? false : true;
            SystemSettingModel.RefreshFrequency = Convert.ToInt32(IniFileHelper.IniValue("sys", "RefreshFrequency", "1"));
            SystemSettingModel.Language = IniFileHelper.IniValue("sys", "Language", "English") == "Chinese" ? 
                SystemSettingPropertyModel.language.Chinese: SystemSettingPropertyModel.language.English;
            SystemSettingModel.Theme = IniFileHelper.IniValue("sys", "Theme", "White") == "White" ? SystemSettingPropertyModel.theme.White: SystemSettingPropertyModel.theme.Dark ;

        }

        public void UpdateSettings()
        {
            if (SystemSettingModel.Language == SystemSettingPropertyModel.language.Chinese)
                UpdateLanguageEvent("zh-CN");
            else
                UpdateLanguageEvent("en-US");

            if (SystemSettingModel.Theme == SystemSettingPropertyModel.theme.White)
                UpdateThemeEvent("White");
            else
                UpdateThemeEvent("Dark");

            WriteIni();
        }

        private void Button_Cancle_Click(object sender, RoutedEventArgs e)
        {
            ReadIni();
        }
    }
}
