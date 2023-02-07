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
using NiMotion.Common;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace NiMotion.View
{
    /// <summary>
    /// MotorOperation.xaml 的交互逻辑
    /// </summary>
    public partial class MotorOperation
    {
        [DllImport("kernel32")]
        static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

        [DllImport("kernel32")]
        static extern bool QueryPerformanceCounter(ref long PerformanceFrequency);

        private bool IsStopTimer = true;
        private string Section = "MotorOperation";
        private Dictionary<string, int> motorSettingDict = null;
        private Dictionary<string, int> motorSettingLength = null;
        private Dictionary<int, int> motorSegment = null;

        public MotorOperationViewModel Context
        {
            get { return context; }
        }

        private MotorOperationViewModel context;
        public MotorOperation()
        {
            InitializeComponent();

            motorSettingDict = NiMotionRegisterDict.GetMotorSettingKeyDict();
            motorSettingLength = NiMotionRegisterDict.GetMotorSettingLengthDict();
            motorSegment = NiMotionRegisterDict.GetMotorSegmentDict();
            context = new MotorOperationViewModel();
            IniInitialize();
            DataContext = context;
        }

        public double ReadFromIni(string Param, string def)
        {
            string readValue = IniFileHelper.IniValue(Section, Param, def);
            return Convert.ToDouble(readValue);
        }

        public void WriteToIni(string Param, string Value)
        {
            IniFileHelper.WriteValue(Section, Param, Value);
        }

        public void IniInitialize()
        {
            context.MotorSpeed = ReadFromIni("Speed", "18.0");
            context.Timing = (int)ReadFromIni("Timer", "10");
        }

        public int ReadParam(string param)
        {
            int readValue = 0;
            int paramId = motorSettingDict[param];
            int paramLength = motorSettingLength[param];
            int ret = NiMotionSDK.NiM_readParam(context.MotorAddr, paramId, paramLength, ref readValue);
            if (0 != ret)
                throw new Exception(string.Format("NiM_readParam {0} Failed [{1}]", param, ret));
            return readValue;
        }

        public int ConvertToStepSpeed(double degreesSpeed)
        {
           // int segment = ReadParam("Segmentation");
           // int pulseNumber = motorSegment[segment];
           if((bool)RBForward.IsChecked)
            return (int)(degreesSpeed  / 0.18 );
           else
            return -1 * (int)(degreesSpeed / 0.18);
        }

        private void RunSpeedMode()
        {
            try
            {
                int ret = NiMotionSDK.NiM_powerOff(context.MotorAddr);
                if (0 != ret) throw new Exception(string.Format("Call NiM_powerOff Failed [{0}]", ret));

                ret = NiMotionSDK.NiM_changeWorkMode(context.MotorAddr, NiMotionSDK.WORK_MODE.VELOCITY_MODE);
                if (0 != ret) throw new Exception(string.Format("Call NiM_changeWorkMode Failed [{0}]", ret));

                int speed = ConvertToStepSpeed(context.MotorSpeed);

                ret = NiMotionSDK.NiM_powerOn(context.MotorAddr);
            
                ret = NiMotionSDK.NiM_moveVelocity(context.MotorAddr, speed);   //电机正转

            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.Message);
            }
        }

        private void RunLocationMode()
        {
            HandyControl.Controls.MessageBox.Show("Not implemented");
        }

        private async void Button_StartUp_Click(object sender, RoutedEventArgs e)
        {
            if (context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                WriteToIni("Speed", Convert.ToString(context.MotorSpeed));
                WriteToIni("Timer", Convert.ToString(context.Timing));
                if ((bool)RBSpeed.IsChecked)
                {
                    if(context.IsShowTimer && !IsStopTimer)
                    {
                        HandyControl.Controls.MessageBox.Show("Pelase Stop motor first");
                        return;
                    }

                    RunSpeedMode();
                    if (context.IsShowTimer)
                    {
                        IsStopTimer = false;
                        await Task.Run(() =>
                        {
                            long startTime = 0;
                            long stopTime = 0;
                            long timerFrequency = 0;
                            MotorOperation.QueryPerformanceFrequency(ref timerFrequency);
                            QueryPerformanceCounter(ref startTime);
                            while (true)
                            {
                                Thread.Sleep(300);
                                MotorOperation.QueryPerformanceCounter(ref stopTime);
                                double time = (double)(stopTime - startTime) / (double)(timerFrequency);
                                if (IsStopTimer)
                                {
                                    return;
                                }
                                if (time >= context.Timing)
                                {
                                    NiMotionSDK.NiM_stop(context.MotorAddr);
                                    IsStopTimer = true;
                                    return;
                                }
                            }                                
                        });
                    }
                }
                else if ((bool)RBPosition.IsChecked)
                {
                    RunLocationMode();
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show("Unkown Mode");
                }
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor device is not opened"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor address out of range"));
                }
            }
        }

        private void Button_Hold_Click(object sender, RoutedEventArgs e)
        {
            if (context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                NiMotionSDK.NiM_powerOn(context.MotorAddr);
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor device is not opened"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor address out of range"));
                }
            }
        }

        private void Button_Offline_Click(object sender, RoutedEventArgs e)
        {
            if(context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                NiMotionSDK.NiM_powerOff(context.MotorAddr);
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor device is not opened"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor address out of range"));
                }
            }
        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                NiMotionSDK.NiM_stop(context.MotorAddr);
                IsStopTimer = true;
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor device is not opened"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor address out of range"));
                }
            }
        }

        private void Button_EmergencyStop_Click(object sender, RoutedEventArgs e)
        {
            if (context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                NiMotionSDK.NiM_fastStop(context.MotorAddr);
                IsStopTimer = true;
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor device is not opened"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor address out of range"));
                }
            }
        }
   
    }
}
