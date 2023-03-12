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

namespace NiMotion.View
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class SystemSetting
    {
        public delegate void UpdateLanguage(string language);
        public event UpdateLanguage UpdateLanguageEvent;

        public SystemSetting()
        {
            InitializeComponent();

            SystemSettingModel = new SystemSettingPropertyModel
            {
                EnableLog = true
                
            };
            DataContext = this;

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
            if (SystemSettingModel.Language == SystemSettingPropertyModel.language.Chinese)
                UpdateLanguageEvent("zh-CN");
            else
                UpdateLanguageEvent("en-US");
        }

        private void Button_Cancle_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
