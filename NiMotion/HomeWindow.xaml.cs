using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HandyControl.Tools;
using NiMotion.Model;
using NiMotion.View;
using NiMotion.ViewModel;

namespace NiMotion
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HomeWindow
    {
        private bool isMotorOpen = false;
        private int motorNumber = 0;
        private uint motorMaster = 0;

        private HomeWindowViewModel context;
        private MotorOperation motorOperation = new MotorOperation();
        private MotorSetting motorSetting = new MotorSetting();
        private SystemSetting systemSetting = new SystemSetting();
        private AutoRunScript autoRunScript = new AutoRunScript();



        public HomeWindow()
        {
            InitializeComponent();
            InitialResourceDictionary();
            InitialDelegate();
            Dictionary<string, string> name_dict = new Dictionary<string, string>();
            name_dict.Add("MotorOperation", (string)FindResource("MotorOperation"));
            name_dict.Add("MotorSetting", (string)FindResource("MotorSetting"));
            name_dict.Add("SystemSetting", (string)FindResource("SystemSetting"));
            name_dict.Add("AutoRunScript", (string)FindResource("AutoRunScript"));
            context = new HomeWindowViewModel(name_dict);
            DataContext = context;
            systemSetting.UpdateSettings();
        }

        // Ititial ResourceDictionary
        private void InitialResourceDictionary()
        {
            ResourceDictionary theme = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Resource/Style/Theme/BaseLight.xaml")
            };
            Resources.MergedDictionaries.Add(theme);
            // Color
            ResourceDictionary color = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Resource/Style/Primary/Primary.xaml")
            };
            Resources.MergedDictionaries.Add(color);

        }

        private void InitialDelegate()
        {
            topBar.MotorStatusEvent += UpdateMotorStatus;
            topBar.MotorNumberEvent += UpdateMotorNumber;
            topBar.MotorMasterEvent += UpdateMotorMaster;
            systemSetting.UpdateLanguageEvent += UpdateLanguage;
            systemSetting.UpdateThemeEvent += UpdateTheme;
            autoRunScript.MotorStartEvent += motorOperation.MotorStart;
            autoRunScript.MotorStopEvent += motorOperation.MotorStop;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (context.SelectedIndex < 0)
            {
                return;
            };
            main_content.Children.Clear();
            string name = (ListBox.SelectedItem as DataModel).Name;
            if (name == "MotorOperation" || name == "电机操作")
            {
                main_content.Children.Add(motorOperation);
            }
            else if (name == "MotorSetting" || name == "电机参数设置")
            {
                main_content.Children.Add(motorSetting);
            }
            else if (name == "AutoRunScript" || name == "脚本自动化")
            {
                main_content.Children.Add(autoRunScript);
            }
            else if (name == "SystemSetting" || name == "系统参数设置")
            {
                main_content.Children.Add(systemSetting);
            }
        }


        private void UpdataLanguage(string language)
        {
            int len = context.DataList.Count;

            for (int i = 0; i < len; i++)
            {
                if (context.DataList[i].Name == "MotorOperation" || context.DataList[i].Name == "电机操作")
                {
                    context.DataList[i].Name = (string)FindResource("MotorOperation");
                }
                else if (context.DataList[i].Name == "MotorSetting" || context.DataList[i].Name == "电机参数设置")
                {
                    context.DataList[i].Name = (string)FindResource("MotorSetting");
                }
                else if (context.DataList[i].Name == "AutoRunScript" || context.DataList[i].Name == "脚本自动化")
                {
                    context.DataList[i].Name = (string)FindResource("AutoRunScript");
                }
                else if (context.DataList[i].Name == "SystemSetting" || context.DataList[i].Name == "系统参数设置")
                {
                    context.DataList[i].Name = (string)FindResource("SystemSetting");
                }
            }
        }

        private void UpdateMotorStatus(bool motorOpen)
        {
            isMotorOpen = motorOpen;
            motorOperation.IsMotorOpen = motorOpen;
            motorSetting.IsMotorOpen = motorOpen;
            bottomBar.Context.IsMotorOpen = motorOpen;
        }

        private void UpdateMotorNumber(int addr)
        {
            motorNumber = addr;
            motorOperation.Context.MotorAddr = addr;
            motorSetting.MotorAddr = addr;
            bottomBar.Context.MotorAddr = addr;
        }

        private void UpdateMotorMaster(uint master)
        {
            motorMaster = master;
            motorOperation.Context.MotorMaster = master;
            motorSetting.MotorMaster = master;
            bottomBar.Context.MotorMaster = master;

        }

        private void UpdateLanguage(string language)
        {
            App.Language = language;
            System.Threading.Thread.Sleep(100);
            UpdataLanguage(language);
        }


        // Updata ResourceDictionary
        private void UpdataResourceDictionary(string resourceStr, int pos)
        {
            if (pos < 0 || pos > 2)
            {
                return;
            }
            ResourceDictionary resource = new ResourceDictionary
            {
                Source = new Uri(resourceStr)
            };
            Resources.MergedDictionaries.RemoveAt(pos);
            Resources.MergedDictionaries.Insert(pos, resource);
        }

        private void UpdateTheme(string theme)
        {
            string resourceStr = "pack://application:,,,/Resource/Style/Theme/BaseLight.xaml";
            if (theme == "Dark")
            {
                resourceStr = "pack://application:,,,/Resource/Style/Theme/BaseDark.xaml";
            }
            UpdataResourceDictionary(resourceStr, 0);

        }
    }
}
