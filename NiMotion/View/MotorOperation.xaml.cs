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
using NiMotion.Common;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using NimServoSDK_DLL;
using HandyControl.Data;

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


        public MotorOperationViewModel Context
        {
            get { return context; }
        }

        public bool IsMotorOpen
        {
            set
            {
                context.IsMotorOpen = value;
                if(!context.IsMotorOpen)
                {
                    context.IsShowSpeedBar = false;
                    context.IsShowLocationBar = false;
                }
            }
        }

        private MotorOperationViewModel context;
        public MotorOperation()
        {
            InitializeComponent();

            context = new MotorOperationViewModel()
            {
                IsShowSpeedBar = false,
                IsShowLocationBar = false
            
            };
            IniInitialize();
            DataContext = context;
            tHour.VerifyFunc = str =>
            {
                if(int.TryParse(str, out int v))
                {
                    if (v >= 0)
                    {
                        return OperationResult.Success();
                    } else {
                        return OperationResult.Failed((string)FindResource("OutOfRange"));
                    }
                }
                else
                {
                    return OperationResult.Failed((string)FindResource("Error"));
                }       
            }; 

        }


        public string ReadFromIni(string Param, string def)
        {
            string readValue = IniFileHelper.IniValue(Section, Param, def);
            return readValue;
        }

        public void WriteToIni(string Param, string Value)
        {
            IniFileHelper.WriteValue(Section, Param, Value);
        }

        public void IniInitialize()
        {
            context.MotorSpeed = Convert.ToDouble(ReadFromIni("Speed", "18.0"));
            context.IsShowTimer = ReadFromIni("ShowTimer", "0") == "1" ? true : false;
            context.Hour = Convert.ToInt32(ReadFromIni("TimerHour", "0"));
            context.Min = Convert.ToInt32(ReadFromIni("TimerMin", "0"));
            context.Sec = Convert.ToInt32(ReadFromIni("TimerSec", "0"));
            if (ReadFromIni("Rotation", "Forward") == "Forward")
            {
                RBForward.IsChecked = true;
            }
            else
            {
                RBReverse.IsChecked = false;
            }
            context.Position = Convert.ToInt32(ReadFromIni("Position", "0"));
            context.MotorPositionSpeed = Convert.ToInt32(ReadFromIni("PositionSpeed", "0"));
            CBOrigin.IsChecked = ReadFromIni("Origin", "0") != "0" ? true : false;
            if(ReadFromIni("LocationMode", "Absolute") == "Absolute")
            {
                RBAbsolute.IsChecked = true;
            } else
            {
                RBRelative.IsChecked = true;
            }
        }

        public void WriteValueToIni()
        {
            WriteToIni("Speed", context.MotorSpeed.ToString());
            WriteToIni("ShowTimer", context.IsShowTimer ? "1" : "0");
            WriteToIni("TimerHour", context.Hour.ToString());
            WriteToIni("TimerMin", context.Min.ToString());
            WriteToIni("TimerSec", context.Sec.ToString());
            WriteToIni("Rotation", (bool)RBForward.IsChecked ? "Forward" : "Reverse");
            WriteToIni("Position", context.Position.ToString());
            WriteToIni("PositionSpeed", context.MotorPositionSpeed.ToString());
            WriteToIni("Origin", (bool)CBOrigin.IsChecked ? "1" : "0");
            WriteToIni("LocationMode", (bool)RBAbsolute.IsChecked ? "Absolute" : "Relative");
        }

        public uint ReadParam(string param)
        {
            uint readValue = 0;
            int ret = NimServoSDK.Nim_get_param_value(context.MotorMaster,context.MotorAddr, param, ref readValue, 1);
            if (0 != ret)
                throw new Exception(string.Format("NiM_readParam {0} Failed [{1}]", param, ret));
            return readValue;
        }

        public double ConvertToStepSpeed(double degreesSpeed)
        {
            //用户单位 / 10000 *  60 * 360 / 60 = 度 /s
            //转速比是10
            return (double)(degreesSpeed * 10000.0 * 10 / 360);
        }

        private void RunSpeedMode()
        {
            try
            {
                context.IsShowSpeedBar = true;
                int ret = NimServoSDK.Nim_power_off(context.MotorMaster, context.MotorAddr, 1);
                if (0 != ret) throw new Exception(string.Format("Call NiM_powerOff Failed [{0}]", ret));
                Thread.Sleep(200);
                ret = NimServoSDK.Nim_set_workMode(context.MotorMaster, context.MotorAddr, (int)ServoWorkMode.SERVO_PV_MODE, 1);
                if (0 != ret) throw new Exception(string.Format("Call Nim_set_workMode VELOCITY_MODE Failed [{0}]", ret));
                Thread.Sleep(200);
           
                ret = NimServoSDK.Nim_power_on(context.MotorMaster, context.MotorAddr, 1);
                if (0 != ret) throw new Exception(string.Format("Call Nim_power_on Failed [{0}]", ret));

                ret = NimServoSDK.Nim_set_profileAccel(context.MotorMaster, context.MotorAddr, 10000);
                ret = NimServoSDK.Nim_set_profileDecel(context.MotorMaster, context.MotorAddr, 10000);
                double speed = ConvertToStepSpeed(context.MotorSpeed);
                if ((bool)RBForward.IsChecked)
                    ret = NimServoSDK.Nim_forward(context.MotorMaster, context.MotorAddr, speed, 1);
                else
                    ret = NimServoSDK.Nim_backward(context.MotorMaster, context.MotorAddr, speed, 1);
                if (0 != ret) throw new Exception(string.Format("Call Nim_forward Failed [{0}]", ret));
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
                int ret = NimServoSDK.Nim_power_off(context.MotorMaster, context.MotorAddr, 1);
                if (0 != ret) throw new Exception(string.Format("Call Nim_power_off Failed [{0}]", ret));
                Thread.Sleep(200);

                ret = NimServoSDK.Nim_set_workMode(context.MotorMaster, context.MotorAddr, (int)ServoWorkMode.SERVO_PP_MODE, 1);
                if (0 != ret) throw new Exception(string.Format("Call Nim_set_workMode VELOCITY_MODE Failed [{0}]", ret));
                Thread.Sleep(200);

                if ((bool)CBOrigin.IsChecked)
                {
                    ret = NimServoSDK.Nim_set_param_value(context.MotorMaster, context.MotorAddr, "I2017-1", 33,  1);
                    Thread.Sleep(200);
                    ret = NimServoSDK.Nim_set_param_value(context.MotorMaster, context.MotorAddr, "I2017-2", 0, 1);
                    Thread.Sleep(200);
                }

                ret = NimServoSDK.Nim_power_on(context.MotorMaster, context.MotorAddr, 1);
                if (0 != ret) throw new Exception(string.Format("Call NiM_powerOn Failed [{0}]", ret));

                Thread.Sleep(200);

                if ((bool)CBOrigin.IsChecked)
                {
                    ret = NimServoSDK.Nim_set_param_value(context.MotorMaster, context.MotorAddr, "I2031-1", 0, 1);
                    Thread.Sleep(200);
                    ret = NimServoSDK.Nim_set_param_value(context.MotorMaster, context.MotorAddr, "I2031-1", 1, 1);
                } else
                {
                    ret = NimServoSDK.Nim_set_param_value(context.MotorMaster, context.MotorAddr, "I2031-1", 0, 1);
                    Thread.Sleep(200);
                }


                ret = NimServoSDK.Nim_power_off(context.MotorMaster, context.MotorAddr, 1);
                if (0 != ret) throw new Exception(string.Format("Call NiM_powerOff Failed [{0}]", ret));
                Thread.Sleep(200);
                ret = NimServoSDK.Nim_power_on(context.MotorMaster, context.MotorAddr, 1);
                if (0 != ret) throw new Exception(string.Format("Call NiM_powerOn Failed [{0}]", ret));

                NimServoSDK.Nim_set_profileVelocity(context.MotorMaster, context.MotorAddr, 
                    ConvertToStepSpeed(context.MotorPositionSpeed));

                double fromPosition = 0;
                ret = NimServoSDK.Nim_get_currentPosition(context.MotorMaster, context.MotorAddr, ref fromPosition, 1);
                if (0 != ret) throw new Exception(string.Format("Call NiM_getCurrentPosition Failed [{0}]", ret));
                Thread.Sleep(200);
                if ((bool)RBAbsolute.IsChecked)  //绝对位置
                {
                    ret = NimServoSDK.Nim_moveAbsolute(context.MotorMaster, context.MotorAddr, (double)context.Position * 10000, 0, 1);
                    if (0 != ret) throw new Exception(string.Format("Call NiM_moveAbsolute Failed [{0}]", ret));
                }
                else if ((bool)RBRelative.IsChecked) //相对位置
                {
                    ret = NimServoSDK.Nim_moveRelative(context.MotorMaster, context.MotorAddr, (double)context.Position * 10000, 0, 1);
                    if (0 != ret) throw new Exception(string.Format("Call NiM_moveRelative Failed [{0}]", ret));
                }
                Thread.Sleep(200);
                bool isAbsolute = (bool)RBAbsolute.IsChecked;
                Task.Run(() =>
                {
                    int pos = context.Position * 10000;

                    while (true)
                    {
                        Thread.Sleep(200);
                        if (IsStop)
                        {
                            context.IsShowLocationBar = false;
                            return;
                        }

                        int readValue = 0;

                        ret = NimServoSDK.Nim_get_currentMotorSpeed(context.MotorMaster, context.MotorAddr, ref readValue, 1);
                        if (0 != ret) return;
                        if (0 == readValue)  // speed = 0step/s
                        {
                            double curPosition = 0;
                            ret = NimServoSDK.Nim_get_currentPosition(context.MotorMaster, context.MotorAddr, ref curPosition, 1);
                            if (0 != ret) return;
                            double dif = 0;
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
                                ret = NimServoSDK.Nim_moveAbsolute(context.MotorMaster, context.MotorAddr, pos, 0, 1);
                                if (0 != ret) return;
                            }
                            else
                            {
                                ret = NimServoSDK.Nim_moveAbsolute(context.MotorMaster, context.MotorAddr, fromPosition + pos, 0, 1);
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
                WriteValueToIni();
                if (!IsStop)
                {
                    HandyControl.Controls.MessageBox.Show((string)FindResource("msg1"));
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
                            Context.Timing = "00:00:00";

                            MotorOperation.QueryPerformanceFrequency(ref timerFrequency);
                            QueryPerformanceCounter(ref startTime);
                            while (true)
                            {
                                Thread.Sleep(200);
                                MotorOperation.QueryPerformanceCounter(ref stopTime);
                                long time = (long)(stopTime - startTime) / (long)(timerFrequency);
                                //Context.Second = context.Timing - (int)time;
                                TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(time));
                                string str = "";
                                if (ts.Hours > 0)
                                {
                                    str = String.Format("{0:00}", ts.Hours) + ":" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
                                }
                                if (ts.Hours == 0 && ts.Minutes > 0)
                                {
                                    str = "00:" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
                                }
                                if (ts.Hours == 0 && ts.Minutes == 0)
                                {
                                    str = "00:00:" + String.Format("{0:00}", ts.Seconds);

                                }
                                context.Timing = str;
                                if (IsStop)
                                {
                                    context.IsShowSpeedBar = false;
                                    return;
                                }
                                long timing = context.Hour * 3600 + context.Min * 60 + context.Sec;
                                if (time >= timing)
                                {
                                    NimServoSDK.Nim_fastStop(context.MotorMaster, context.MotorAddr, 1);
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
                    HandyControl.Controls.MessageBox.Show((string)FindResource("msg5"));
                }
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show((string)FindResource("msg2"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show((string)FindResource("msg3"));
                }
            }
        }

        private void Button_Hold_Click(object sender, RoutedEventArgs e)
        {
            if (context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                NimServoSDK.Nim_power_on(context.MotorMaster, context.MotorAddr, 1); 
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show((string)FindResource("msg2"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show((string)FindResource("msg3"));
                }
            }
        }

        private void Button_Offline_Click(object sender, RoutedEventArgs e)
        {
            if(context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                NimServoSDK.Nim_power_off(context.MotorMaster, context.MotorAddr, 1);
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show((string)FindResource("msg2"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show((string)FindResource("msg3"));
                }
            }
        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (context.IsMotorOpen && context.MotorAddr > 0 && context.MotorAddr < 249)
            {
                NimServoSDK.Nim_fastStop(context.MotorMaster, context.MotorAddr, 1);
                IsStop = true;
                context.IsShowSpeedBar = false;
                context.IsShowLocationBar = false;
            }
            else
            {
                if (!context.IsMotorOpen)
                {
                    HandyControl.Controls.MessageBox.Show((string)FindResource("msg2"));
                }
                else
                {
                    HandyControl.Controls.MessageBox.Show((string)FindResource("msg3"));
                }
            }
        }

        private void ToggleButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
