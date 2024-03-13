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
using HandyControl.Controls;
using NiMotion.Common;
using NiMotion.Model;
using NimServoSDK_DLL;


namespace NiMotion.View
{
    /// <summary>
    /// MotorSetting.xaml 的交互逻辑
    /// </summary>
    public partial class MotorSetting
    {
        
        public bool IsMotorOpen { get; set; }
        public uint MotorMaster { get; set; }
        public int MotorAddr { get; set; }

        private string Section = "MotorParam";
        private Dictionary<string, int> motorSettingDict = null;
        private Dictionary<string, int> motorSettingLength = null;

        public MotorSetting()
        {
            InitializeComponent();

            MotorSettingModel = new MotorSettingPropertyModel();
            DataContext = this;
            MotorParamsInit();
        }

        public static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached(
            "MotorSettingModel", typeof(MotorSettingPropertyModel), typeof(PropertyGrid),
            new FrameworkPropertyMetadata(default(MotorSettingPropertyModel))
            {
                BindsTwoWayByDefault = true
            });

        public MotorSettingPropertyModel MotorSettingModel
        {
            get => (MotorSettingPropertyModel)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        private uint ReadUintParamFromIni(string param, string def)
        {
            string readValue = IniFileHelper.IniValue(Section, param, def);
            return Convert.ToUInt32(readValue);
        }

        private double ReadDoubleParamFromIni(string param, string def)
        {
            string readValue = IniFileHelper.IniValue(Section, param, def);
            return Convert.ToDouble(readValue);
        }

        private void MotorParamsInit()
        {

            MotorSettingModel.MaxSpeed = ReadUintParamFromIni("MaxSpeed", "250");
            MotorSettingModel.Acceleration = ReadDoubleParamFromIni("Acceleration", "100");
            MotorSettingModel.Deceleration = ReadDoubleParamFromIni("Deceleration", "100");

        }

        public void WriteMotorUintParam(string param, uint value)
        {
            int ret = 0;

            if (IsMotorOpen && MotorAddr > 0 && MotorAddr < 248)
            {
                uint readValue = 0;
                ret = NimServoSDK.Nim_get_maxMotorSpeed(MotorMaster, MotorAddr, ref readValue);
                if(readValue != MotorSettingModel.MaxSpeed)
                {
                    NimServoSDK.Nim_set_maxMotorSpeed(MotorMaster, MotorAddr, MotorSettingModel.MaxSpeed);
                }

                if (0 != ret)
                    throw new Exception(string.Format("NiM_readParam {0} Failed [{1}]", param, ret));
                if(readValue != value) {
                    ret = NimServoSDK.Nim_set_param_value(MotorMaster, MotorAddr, param, value, 1);
                    if (0 != ret)
                        throw new Exception(string.Format("NiM_writeParam {0} Failed [{1}]", param, ret));
                }
            }
            else
            {
                if (!IsMotorOpen)
                {
                    throw new Exception((string)FindResource("msg2"));
                }else
                {
                    throw new Exception((string)FindResource("msg3"));
                }
            }
        }

        public void WriteMotorDoubleParam(string param, double value)
        {
            int ret = 0;

            if (IsMotorOpen && MotorAddr > 0 && MotorAddr < 248)
            {
                if (param == "Acceleration")
                    NimServoSDK.Nim_set_profileAccel(MotorMaster, MotorAddr, MotorSettingModel.Acceleration);
                else if(param == "Deceleration")
                    NimServoSDK.Nim_set_profileDecel(MotorMaster, MotorAddr, MotorSettingModel.Deceleration);

                if (0 != ret)
                    throw new Exception(string.Format("NiM_readParam {0} Failed [{1}]", param, ret));
            }
            else
            {
                if (!IsMotorOpen)
                {
                    throw new Exception((string)FindResource("msg2"));
                }
                else
                {
                    throw new Exception((string)FindResource("msg3"));
                }
            }
        }

        private void WriteMotorParams()
        {
            try
            {
                //WriteMotorUintParam("MaxSpeed", MotorSettingModel.MaxSpeed);
                WriteMotorDoubleParam("Acceleration", MotorSettingModel.Acceleration);
                WriteMotorDoubleParam("Deceleration", MotorSettingModel.Deceleration);

            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private double SyncMotorDoubleParam(string param)
        {
            int ret = 0;
            if (IsMotorOpen && MotorAddr > 0 && MotorAddr < 248)
            {
                double readValue = 0;
                if(param == "Acceleration")
                    ret = NimServoSDK.Nim_get_profileAccel(MotorMaster, MotorAddr, ref readValue);
                else if (param == "Deceleration")
                    ret = NimServoSDK.Nim_get_profileDecel(MotorMaster, MotorAddr, ref readValue);
                if (0 != ret)
                    throw new Exception(string.Format("Nim_get_param_value {0} Failed [{1}]", param, ret));

                IniFileHelper.WriteValue(Section, param, Convert.ToString(readValue));
                return readValue;
            }
            else
            {
                if (!IsMotorOpen)
                {
                    throw new Exception((string)FindResource("msg2"));
                }
                else
                {
                    throw new Exception((string)FindResource("msg3"));
                }
            }
        }


        private uint SyncMotorUintParam(string param)
        {
            int ret = 0;
            if (IsMotorOpen && MotorAddr > 0 && MotorAddr < 248)
            {
                uint readValue = 0;
                if (param == "MaxSpeed")
                    ret = NimServoSDK.Nim_get_maxMotorSpeed(MotorMaster, MotorAddr, ref readValue);
                if (0 != ret)
                    throw new Exception(string.Format("Nim_get_param_value {0} Failed [{1}]", param, ret));

                IniFileHelper.WriteValue(Section, param, Convert.ToString(readValue));
                return readValue;
            }
            else
            {
                if (!IsMotorOpen)
                {
                    throw new Exception((string)FindResource("msg2"));
                }
                else
                {
                    throw new Exception((string)FindResource("msg3"));
                }
            }
        }

        private void SyncMotorParams()
        {
            try
            {
                //MotorSettingModel.MaxSpeed = SyncMotorUintParam("MaxSpeed");
                //MotorSettingModel.MinSpeed = SyncMotorParam("MinSpeed");
                MotorSettingModel.Acceleration = SyncMotorDoubleParam("Acceleration");
                MotorSettingModel.Deceleration = SyncMotorDoubleParam("Deceleration");

                
                
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            pg.DataContext = this;
        }

        private void Button_WriteParams_Click(object sender, RoutedEventArgs e)
        {
            WriteMotorParams();
            SyncMotorParams();
        }

        private void Button_ReadParams_Click(object sender, RoutedEventArgs e)
        {
            SyncMotorParams();
        }

    }
 
}

