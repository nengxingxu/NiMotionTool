using NiMotion.ViewModel;
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
using SDKDemo;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NiMotion.View
{
    /// <summary>
    /// TopBar.xaml 的交互逻辑
    /// </summary>
    public partial class TopBar
    {
        private bool motorOpen = false;
        private TopBarViewModel context;

        public delegate void UpdateMotorStatus(bool motorOpen);
        public delegate void UpdateMotorNumber(int num);

        public event UpdateMotorStatus MotorStatusEvent;
        public event UpdateMotorNumber MotorNumberEvent;

        public TopBar()
        {
            InitializeComponent();

            context = new TopBarViewModel();
            DataContext = context;
            BtnCloseDevice.IsEnabled = false;
        }

        private void Button_Open_Click(object sender, RoutedEventArgs e)
        {
            bool isOpenDevice = false;
            try
            {
                if (context.PortList.Count() <= 0)
                {
                    MessageBox.Show("No serial port exist", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string StrConnectMsg = "{\"DeviceName\": \"" + context.PortNum + "\", \"Baudrate\":" + context.BaudRate + ", \"Parity\": \"None\", \"DataBits\": 8, \"StopBits\": 1}";
                int ret = NiMotionSDK.NiM_openDevice(0, StrConnectMsg);
                if (0 != ret) throw new Exception("Call NiM_openDevice Failed");
                isOpenDevice = true;

                ret = NiMotionSDK.NiM_scanMotors(1, 10);
                if (0 != ret) throw new Exception("Call NiM_scanMotors Failed");

                int count = 0;
                IntPtr hglobal = Marshal.AllocHGlobal(100 * 4);

                ret = NiMotionSDK.NiM_getOnlineMotors(hglobal, ref count);
                if (0 != ret) throw new Exception("Call NiM_scanMotors Failed");

                int[] ptrArray = new int[100];
                Array.Clear(ptrArray, 0, ptrArray.Length);
                Marshal.Copy(hglobal, ptrArray, 0, count);
                context.UpdateOnlineMotors(ptrArray, count);
                Marshal.FreeHGlobal(hglobal);
                BtnCloseDevice.IsEnabled = true;
                BtnOpenDevice.IsEnabled = false;
                motorOpen = true;
            }
            catch (Exception ex)
            {
                if(isOpenDevice) NiMotionSDK.NiM_closeDevice();
                motorOpen = false;
                MessageBox.Show(ex.Message);
            }
            MotorStatusEvent(motorOpen);
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (motorOpen)
            {
                int ret = NiMotionSDK.NiM_closeDevice();
                BtnOpenDevice.IsEnabled = true;
                BtnCloseDevice.IsEnabled = false;
                motorOpen = false;
                context.UpdateOnlineMotors(new int[0], 0);
                MotorStatusEvent(motorOpen);
            }
        }

        private void CmbMotorNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox cmb))
            {
                return;
            }
            if(cmb.SelectedItem != null)
            {
                int num = Convert.ToInt32(cmb.SelectedItem.ToString());
                if (num >= 0) MotorNumberEvent(num);
            }
            else
            {
                MotorNumberEvent(0);
            }
        }
    }
}
