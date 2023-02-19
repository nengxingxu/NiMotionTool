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
using SDKDemo;

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
        private Dictionary<string, int> motorSettingDict = null;
        private Dictionary<string, int> motorSettingLength = null;

        public BottomBar()
        {
            InitializeComponent();
            DataContext = context;
            motorSettingDict = NiMotionRegisterDict.GetMotorSettingKeyDict();
            motorSettingLength = NiMotionRegisterDict.GetMotorSettingLengthDict();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }


        public void GetMotorInfo()
        {
            try
            {
                bool isOnline = false;
                int readValue = 0;
                int ret = NiMotionSDK.NiM_isMotorOnline(context.MotorAddr, ref isOnline);
                if(0 == ret)
                {
                    context.Status = string.Format("Status: {0}", isOnline ? "Online":"Offline");
                } else
                {
                    context.Status = string.Format("Status: Error");
                }

                int paramId = motorSettingDict["MotorMode"];
                int paramLength = motorSettingLength["MotorMode"];      
                ret = NiMotionSDK.NiM_readParam(context.MotorAddr, paramId, paramLength, ref readValue);
                if (0 == ret)
                {
                    if (1 == readValue)
                        context.Mode = string.Format("Mode: Position");
                    else if (2 == readValue)
                        context.Mode = string.Format("Mode: Speed");
                    else
                        context.Mode = string.Format("Mode: Unkown");
                }
                else
                {
                    context.Mode = string.Format("Mode: Error");
                }

                paramId = motorSettingDict["CurrentSpeed"];
                paramLength = motorSettingLength["CurrentSpeed"];
                ret = NiMotionSDK.NiM_readParam(context.MotorAddr, paramId, paramLength, ref readValue);
                if (0 == ret)
                    context.Speed = string.Format("Speed: {0} step/s", readValue);
                else
                    context.Speed = string.Format("Speed: Error");

                paramId = motorSettingDict["Acceleration"];
                paramLength = motorSettingLength["Acceleration"];
                ret = NiMotionSDK.NiM_readParam(context.MotorAddr, paramId, paramLength, ref readValue);
                if (0 == ret)
                    context.Acceleration = string.Format("Acceleration: {0} step/s2", readValue);
                else
                    context.Acceleration = string.Format("Acceleration: Error");

                paramId = motorSettingDict["Deceleration"];
                paramLength = motorSettingLength["Deceleration"];
                ret = NiMotionSDK.NiM_readParam(context.MotorAddr, paramId, paramLength, ref readValue);
                if (0 == ret)
                    context.Deceleration = string.Format("Deceleration: {0} step/s2", readValue);
                else
                    context.Deceleration = string.Format("Deceleration: Error");

                ret = NiMotionSDK.NiM_getCurrentPosition(context.MotorAddr, ref readValue);
                if (0 == ret)
                    context.Position = string.Format("Position: {0} ", readValue);
                else
                    context.Position = string.Format("Position: Error");
            }
            catch (Exception ex)
            {

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
                context.Status = "Status: Offline";
                context.Mode = "Mode:    ";
                context.Speed = "Speed:    step/s";
                context.Acceleration = "Acceleration:    step/s2";
                context.Deceleration = "Deceleration:    step/s2";
                context.Position = "Position: ";
            }
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            RefreshMotorInfo();
        }


    }
}
