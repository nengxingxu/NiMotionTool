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
using NimServoSDK_DLL;
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
        private uint hMaster = 0;
        private int motorAddr = 0;

        public delegate void UpdateMotorStatus(bool motorOpen);
        public delegate void UpdateMotorMaster(uint master);
        public delegate void UpdateMotorNumber(int num);

        public event UpdateMotorStatus MotorStatusEvent;
        public event UpdateMotorMaster MotorMasterEvent;
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
                    MessageBox.Show((string)FindResource("msg4"), "error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string path = System.Environment.CurrentDirectory;
                int ret = NimServoSDK.Nim_init(path);
                if (0 != ret) throw new Exception("Nim_init Failed");

                ret = NimServoSDK.Nim_create_master(2, ref hMaster);
                if(0 != ret) throw new Exception("Nim_create_master Failed");

                string StrConnectMsg = "{\"SerialPort\": \"" + context.PortNum + "\", \"Baudrate\": " + context.BaudRate + ","
                                               + " \"Parity\": \"N\", \"DataBits\": 8, \"StopBits\": 1,"
                                               + " \"PDOIntervalMS\": 10, \"SyncIntervalMS\": 0}";

                ret = NimServoSDK.Nim_master_run(hMaster, StrConnectMsg);
                if (0 != ret) throw new Exception("Nim_master_run Failed");
                int[] ptrArray = new int[100];
                int count = 0;
                NimServoSDK.Nim_scan_nodes(hMaster, 1, 10);
                for (int i = 0; i < 10; i++)
                {
                    if (0 != NimServoSDK.Nim_is_online(hMaster, i))
                    {
                        ptrArray[count] = i;
                        count++;
                    }
                }
                //ret = NimServoSDK.Nim_set_param_value(hMaster, 1, "H6063", 0, 1);
                //MotorMasterEvent(hMaster);
                context.UpdateOnlineMotors(ptrArray, count);
                BtnCloseDevice.IsEnabled = true;
                BtnOpenDevice.IsEnabled = false;
                motorOpen = true;
            }
            catch (Exception ex)
            {
                if (isOpenDevice)
                {
                    NimServoSDK.Nim_master_stop(hMaster);
                    NimServoSDK.Nim_destroy_master(hMaster);
                }
                motorOpen = false;
                MessageBox.Show(ex.Message);
            }
            MotorStatusEvent(motorOpen);
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (motorOpen)
            {
                try
                {
                    int ret = NimServoSDK.Nim_master_stop(hMaster);
                    if (ret != 0) throw new Exception("Nim_master_stop Failed");
                    ret = NimServoSDK.Nim_destroy_master(hMaster);
                    if (ret != 0) throw new Exception("Nim_destroy_master Failed");
                    NimServoSDK.Nim_clean();
                }
                catch(Exception ex)
                {
                    motorOpen = false;
                    MessageBox.Show(ex.Message);
                }

                BtnOpenDevice.IsEnabled = true;
                BtnCloseDevice.IsEnabled = false;
                motorOpen = false;
                context.UpdateOnlineMotors(new int[0], 0);
                MotorMasterEvent(0);
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
                motorAddr = num;
                if(motorAddr > 0)
                {
                    try
                    {
                        int ret = NimServoSDK.Nim_load_params(hMaster, motorAddr, "BLMxx_modbus_zh.db");
                        if (0 != ret) throw new Exception("Nim_load_params Failed");

                        // 读取电机PDO配置
                        ret = NimServoSDK.Nim_read_PDOConfig(hMaster, motorAddr);
                        if (0 != ret) throw new Exception("Nim_load_params Failed");

                        ret = NimServoSDK.Nim_set_unitsFactor(hMaster, motorAddr, 1);
                        if (0 != ret) throw new Exception("Nim_set_unitsFactor Failed");
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MotorNumberEvent(0);
            }
        }
    }
}
