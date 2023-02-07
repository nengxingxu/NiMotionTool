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
using NiMotion.Model;
using NiMotion.View;
using NiMotion.ViewModel;
using SDKDemo;

namespace NiMotion
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HomeWindow
    {
        private bool isMotorOpen = false;
        private int motorNumber = 0;

        private HomeWindowViewModel context;
        private MotorOperation motorOperation = new MotorOperation();
        private MotorSetting motorSetting = new MotorSetting();
        private SystemSetting systemSetting = new SystemSetting();

        public HomeWindow()
        {
            InitializeComponent();
            InitialResourceDictionary();
            InitialDelegate();
            context = new HomeWindowViewModel();
            DataContext = context;
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
            // Language
            ResourceDictionary lang = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Resource/Lang/Lang.zh-CN.xaml")
            };
            Resources.MergedDictionaries.Add(lang);
        }

        private void InitialDelegate()
        {
            topBar.MotorStatusEvent += UpdateMotorStatus;
            topBar.MotorNumberEvent += UpdateMotorNumber;

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (context.SelectedIndex < 0)
            {
                return;
            };
            main_content.Children.Clear();
            string name = (ListBox.SelectedItem as DataModel).Name;
            if (name == "MotorOperation")
            {
                main_content.Children.Add(motorOperation);
            }
            else if (name == "MotorSetting")
            {
                main_content.Children.Add(motorSetting);
            }
            else if (name == "SystemSetting")
            {
                main_content.Children.Add(systemSetting);
            }
        }

        
        private void UpdateMotorStatus(bool motorOpen)
        {
            isMotorOpen = motorOpen;
            motorOperation.Context.IsMotorOpen = motorOpen;
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
    }
}
