using NiMotion.ViewModel;
using System.Windows.Threading;
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
using NimServoSDK_DLL;

namespace NiMotion.View
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class BottomBar
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public BottomBarViewModel Context
        {
            get { return context; }
        }
        private BottomBarViewModel context = new BottomBarViewModel();


        public BottomBar()
        {
            InitializeComponent();
            DataContext = context;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }


        public void GetMotorInfo()
        {
            try
            {
                int readValue = 0;
                double doValue = 0.0;
                int ret = NimServoSDK.Nim_is_online(context.MotorMaster, context.MotorAddr);
                context.Status = string.Format("{0}: {1}", FindResource("Status"), ret != 0 ? FindResource("Online") : FindResource("Off-line"));


                ret = NimServoSDK.Nim_get_workModeDisplay(context.MotorMaster, context.MotorAddr, ref readValue, 1);
                if (0 == ret)
                {
                    if (1 == readValue)
                        context.Mode = string.Format("{0}: {1}", FindResource("Mode"), FindResource("Position"));
                    else if (3 == readValue)
                        context.Mode = string.Format("{0}: {1}", FindResource("Mode"), FindResource("Speed"));
                    else
                        context.Mode = string.Format("{0}: {1}", FindResource("Mode"), FindResource("Unkown"));
                }
                else
                {
                    context.Mode = string.Format("{0}: {1}", FindResource("Mode"), FindResource("Error"));
                }

                ret = NimServoSDK.Nim_get_currentMotorSpeed(context.MotorMaster, context.MotorAddr, ref readValue, 1);
                if (0 == ret)
                {
                    // convert rpm to rad/s
                    int rads = readValue * 360 / 60 / 10;
                    context.Speed = string.Format("{0}: {1} rad/s", FindResource("Speed"), rads);
                }
                else
                {
                    context.Speed = string.Format("Speed: Error");
                }

                ret = NimServoSDK.Nim_get_profileAccel(context.MotorMaster, context.MotorAddr, ref doValue);
                if (0 == ret)
                {
                    // convert rpm to rad/s
                    double rads = doValue * 360 / 60 / 10;
                    context.Acceleration = string.Format("{0}: {1} rad/s2", FindResource("Acceleration"), doValue);
                }
                else
                {
                    context.Acceleration = string.Format("{0}: {1}", FindResource("Acceleration"), FindResource("Error"));
                }

                ret = NimServoSDK.Nim_get_profileDecel(context.MotorMaster, context.MotorAddr, ref doValue);
                if (0 == ret)
                {
                    // convert rpm to rad/s
                    double rads = doValue * 360 / 60 / 10;
                    context.Deceleration = string.Format("{0}: {1} rad/s2", FindResource("Deceleration"), doValue);
                }
                else
                {
                    context.Deceleration = string.Format("{0}: {1}", FindResource("Deceleration"), FindResource("Error"));
                }

                ret = NimServoSDK.Nim_get_currentPosition(context.MotorMaster, context.MotorAddr, ref doValue, 1);
                {
                    if (0 == ret)
                    {
                        context.Position = string.Format("{0}: {1}", FindResource("Position"), doValue);
                    }
                    else
                    {
                        context.Position = string.Format("{0}: {1}", FindResource("Position"), FindResource("Error"));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RefreshMotorInfo()
        {
            if (context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                GetMotorInfo();
            }
            else
            {
                context.Status = string.Format("{0}: {1}", FindResource("Status"), FindResource("Offline"));
                context.Mode = string.Format("{0}: ", FindResource("Mode"));
                context.Speed = string.Format("{0}: rad/s2", FindResource("Speed"));
                context.Acceleration = string.Format("{0}: rad/s2", FindResource("Acceleration"));
                context.Deceleration = string.Format("{0}: rad/s2", FindResource("Deceleration"));
                context.Position = string.Format("{0}: ", FindResource("Position"));
            }
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            RefreshMotorInfo();
        }


    }
}
