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

        private uint ReadParamFromIni(string param, string def)
        {
            int paramId = motorSettingDict[param];
            int paramLength = motorSettingLength[param];
            string readValue = IniFileHelper.IniValue(Section, param, def);
            return Convert.ToUInt32(readValue);
        }

        private void MotorParamsInit()
        {

            MotorSettingModel.MaxSpeed = 250; // ReadParamFromIni("MaxSpeed", "250");
            MotorSettingModel.MinSpeed = 0; // ReadParamFromIni("MinSpeed", "1");
            MotorSettingModel.Acceleration = 2000; // ReadParamFromIni("Acceleration", "2000");
            MotorSettingModel.Deceleration = 2000; // ReadParamFromIni("Deceleration", "2000");
            MotorSettingModel.MaxAcceleration = 2000;  // ReadParamFromIni("MaxAcceleration", "2000");
            MotorSettingModel.MaxDeceleration = 2000; // ReadParamFromIni("MaxDeceleration", "2000");
        }

        public void WriteMotorParam(string param, uint value)
        {
            int ret = 0;
            int paramId = motorSettingDict[param];
            int paramLength = motorSettingLength[param];
            if (IsMotorOpen && MotorAddr > 0 && MotorAddr < 248)
            {
                uint readValue = 0;
                ret = NimServoSDK.Nim_get_param_value(MotorMaster, MotorAddr, param, ref readValue, 1);
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

        private void WriteMotorParams()
        {
            try
            {
                WriteMotorParam("MaxSpeed", MotorSettingModel.MaxSpeed);
                WriteMotorParam("MinSpeed", MotorSettingModel.MinSpeed);
                WriteMotorParam("Acceleration", MotorSettingModel.Acceleration);
                WriteMotorParam("Deceleration", MotorSettingModel.Deceleration);
                WriteMotorParam("MaxAcceleration", MotorSettingModel.MaxAcceleration);
                WriteMotorParam("MaxDeceleration", MotorSettingModel.MaxDeceleration);
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private uint SyncMotorParam(string param)
        {
            int ret = 0;
            int paramId = motorSettingDict[param];
            int paramLength = motorSettingLength[param];
            if (IsMotorOpen && MotorAddr > 0 && MotorAddr < 248)
            {
                uint readValue = 0;
                ret = NimServoSDK.Nim_get_param_value(MotorMaster, MotorAddr, param, ref readValue, 1);
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
                MotorSettingModel.MaxSpeed = SyncMotorParam("MaxSpeed");
                MotorSettingModel.MinSpeed = SyncMotorParam("MinSpeed");
                MotorSettingModel.Acceleration = SyncMotorParam("Acceleration");
                MotorSettingModel.Deceleration = SyncMotorParam("Deceleration");
                MotorSettingModel.MaxAcceleration = SyncMotorParam("MaxAcceleration");
                MotorSettingModel.MaxDeceleration = SyncMotorParam("MaxDeceleration");
                
                
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

