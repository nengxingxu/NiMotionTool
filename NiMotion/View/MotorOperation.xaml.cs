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

        private bool IsStop = true;
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
            context = new MotorOperationViewModel()
            {
                IsShowSpeedBar = false,
                IsShowLocationBar = false
            
            };
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
            context.Timing = (int)ReadFromIni("Position", "10");
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
            // one step /s  == 0.18 degree/s
            if ((bool)RBForward.IsChecked)
            return (int)(degreesSpeed  / 0.18 );
           else
            return -1 * (int)(degreesSpeed / 0.18);
        }

        private void RunSpeedMode()
        {
            try
            {
                context.IsShowSpeedBar = true;
                int ret = NiMotionSDK.NiM_powerOff(context.MotorAddr);
                if (0 != ret) throw new Exception(string.Format("Call NiM_powerOff Failed [{0}]", ret));

                ret = NiMotionSDK.NiM_changeWorkMode(context.MotorAddr, NiMotionSDK.WORK_MODE.VELOCITY_MODE);
                if (0 != ret) throw new Exception(string.Format("Call NiM_changeWorkMode VELOCITY_MODE Failed [{0}]", ret));

                int speed = ConvertToStepSpeed(context.MotorSpeed);

                ret = NiMotionSDK.NiM_powerOn(context.MotorAddr);
                if (0 != ret) throw new Exception(string.Format("Call NiM_powerOn Failed [{0}]", ret));

                ret = NiMotionSDK.NiM_moveVelocity(context.MotorAddr, speed);
                if (0 != ret) throw new Exception(string.Format("Call NiM_moveVelocity Failed [{0}]", ret));
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunPositionMode()
        {
            try
            {
                context.IsShowLocationBar = true;
                IsStop = false;
                int ret = NiMotionSDK.NiM_powerOff(context.MotorAddr);
                if (0 != ret) throw new Exception(string.Format("Call NiM_powerOff Failed [{0}]", ret));

                ret = NiMotionSDK.NiM_changeWorkMode(context.MotorAddr, NiMotionSDK.WORK_MODE.POSITION_MODE);
                if (0 != ret) throw new Exception(string.Format("Call NiM_changeWorkMode POSITION_MODE Failed [{0}]", ret));

                ret = NiMotionSDK.NiM_powerOn(context.MotorAddr);
                if (0 != ret) throw new Exception(string.Format("Call NiM_powerOn Failed [{0}]", ret));

                if ((bool)CBOrigin.IsChecked)
                {
                    ret = NiMotionSDK.NiM_saveAsHome(context.MotorAddr);
                    if (0 != ret) throw new Exception(string.Format("Call NiM_saveAsHome Failed [{0}]", ret));
                }
                if ((bool)CBRBZero.IsChecked)
                {
                    ret = NiMotionSDK.NiM_saveAsZero(context.MotorAddr);
                    if (0 != ret) throw new Exception(string.Format("Call NiM_saveAsZero Failed [{0}]", ret));
                }

                int fromPosition = 0;
                ret = NiMotionSDK.NiM_getCurrentPosition(context.MotorAddr, ref fromPosition);
                if (0 != ret) throw new Exception(string.Format("Call NiM_getCurrentPosition Failed [{0}]", ret));

                if ((bool)RBAbsolute.IsChecked)  //绝对位置
                {
                    ret = NiMotionSDK.NiM_moveAbsolute(context.MotorAddr, context.Position);
                    if (0 != ret) throw new Exception(string.Format("Call NiM_moveAbsolute Failed [{0}]", ret));
                }
                else if ((bool)RBRelative.IsChecked) //相对位置
                {
                    ret = NiMotionSDK.NiM_moveRelative(context.MotorAddr, context.Position);
                    if (0 != ret) throw new Exception(string.Format("Call NiM_moveRelative Failed [{0}]", ret));
                }

                bool isAbsolute = (bool)RBAbsolute.IsChecked;

                Task.Run(() =>
                {
                    int pos = context.Position;

                    while (true)
                    {
                        Thread.Sleep(200);
                        if (IsStop)
                        {
                            context.IsShowLocationBar = false;
                            return;
                        }

                        int readValue = 0;
                        int paramId = motorSettingDict["CurrentSpeed"];
                        int paramLength = motorSettingLength["CurrentSpeed"];
                        ret = NiMotionSDK.NiM_readParam(context.MotorAddr, paramId, paramLength, ref readValue);
                        if (0 != ret) return;
                        if (0 == readValue)  // speed = 0step/s
                        {
                            int curPosition = 0;
                            ret = NiMotionSDK.NiM_getCurrentPosition(context.MotorAddr, ref curPosition);
                            int dif = 0;
                            if (isAbsolute)
                                dif = System.Math.Abs(curPosition - pos);
                            else
                                dif = System.Math.Abs(fromPosition + pos - curPosition);

                            if (dif == 0)
                            {
                                context.IsShowLocationBar = false;
                                IsStop = true;
                                return;
                            }

                            if (isAbsolute)
                            {
                                ret = NiMotionSDK.NiM_moveAbsolute(context.MotorAddr, pos);
                                if (0 != ret) return;
                            } else
                            {
                                ret = NiMotionSDK.NiM_moveAbsolute(context.MotorAddr, fromPosition + pos);
                                if (0 != ret) return;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           // context.IsShowLocationBar = false;
        }

        private async void Button_StartUp_Click(object sender, RoutedEventArgs e)
        {
            if (context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                WriteToIni("Speed", Convert.ToString(context.MotorSpeed));
                WriteToIni("Timer", Convert.ToString(context.Timing));

                if (!IsStop)
                {
                    HandyControl.Controls.MessageBox.Show("Pelase Stop motor first");
                    return;
                }
                if ((bool)RBSpeed.IsChecked)
                {
                    RunSpeedMode();
                    if (context.IsShowTimer)
                    {
                        IsStop = false;
                        await Task.Run(() =>
                        {
                            long startTime = 0;
                            long stopTime = 0;
                            long timerFrequency = 0;
                            Context.Second = Context.Timing;
                            MotorOperation.QueryPerformanceFrequency(ref timerFrequency);
                            QueryPerformanceCounter(ref startTime);
                            while (true)
                            {
                                Thread.Sleep(200);
                                MotorOperation.QueryPerformanceCounter(ref stopTime);
                                double time = (double)(stopTime - startTime) / (double)(timerFrequency);
                                Context.Second = context.Timing - (int)time;
                                if (IsStop)
                                {
                                    context.IsShowSpeedBar = false;
                                    return;
                                }
                                if (time >= context.Timing)
                                {
                                    NiMotionSDK.NiM_stop(context.MotorAddr);
                                    context.IsShowSpeedBar = false;
                                    IsStop = true;
                                    return;
                                }
                            }                                
                        });
                    }
                }
                else if ((bool)RBPosition.IsChecked)
                {
                    RunPositionMode();
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
                    HandyControl.Controls.MessageBox.Show(string.Format("Please select the motor machine number"));
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
                    HandyControl.Controls.MessageBox.Show(string.Format("Please select the motor machine number"));
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
                    HandyControl.Controls.MessageBox.Show(string.Format("Please select the motor machine number"));
                }
            }
        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                NiMotionSDK.NiM_stop(context.MotorAddr);
                IsStop = true;
                context.IsShowSpeedBar = false;
                context.IsShowLocationBar = false;
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor device is not opened"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Please select the motor machine number"));
                }
            }
        }

        private void Button_EmergencyStop_Click(object sender, RoutedEventArgs e)
        {
            if (context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                NiMotionSDK.NiM_fastStop(context.MotorAddr);
                IsStop = true;
                context.IsShowSpeedBar = false;
                context.IsShowLocationBar = false;
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Motor device is not opened"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show(string.Format("Please select the motor machine number"));
                }
            }
        }
   
    }
}
