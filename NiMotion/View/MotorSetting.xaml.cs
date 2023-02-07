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
using SDKDemo;

namespace NiMotion.View
{
    /// <summary>
    /// MotorSetting.xaml 的交互逻辑
    /// </summary>
    public partial class MotorSetting
    {
        
        public bool IsMotorOpen { get; set; }
        public int MotorAddr { get; set; }

        private string Section = "MotorParam";
        private Dictionary<string, int> motorSettingDict = null;
        private Dictionary<string, int> motorSettingLength = null;

        public MotorSetting()
        {
            InitializeComponent();

            motorSettingDict = NiMotionRegisterDict.GetMotorSettingKeyDict();
            motorSettingLength = NiMotionRegisterDict.GetMotorSettingLengthDict();
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

        private int ReadParamFromIni(string param, string def)
        {
            int paramId = motorSettingDict[param];
            int paramLength = motorSettingLength[param];
            string readValue = IniFileHelper.IniValue(Section, param, def);
            return Convert.ToInt32(readValue);
        }

        private void MotorParamsInit()
        {
            MotorSettingModel.Segmentation = (Segment)ReadParamFromIni("Segmentation", "3");
            MotorSettingModel.StartEnd = (StopWay)ReadParamFromIni("StartEnd", "1");
            MotorSettingModel.EmergencyStop = (StopWay)ReadParamFromIni("EmergencyStop", "0");
            MotorSettingModel.FaultAction = (StopWay)ReadParamFromIni("FaultAction", "0");
            MotorSettingModel.MaxSpeed = ReadParamFromIni("MaxSpeed", "250");
            MotorSettingModel.MinSpeed = ReadParamFromIni("MinSpeed", "1");
            MotorSettingModel.Acceleration = ReadParamFromIni("Acceleration", "2000");
            MotorSettingModel.Deceleration = ReadParamFromIni("Deceleration", "2000");
            MotorSettingModel.MaxAcceleration = ReadParamFromIni("MaxAcceleration", "2000");
            MotorSettingModel.MaxDeceleration = ReadParamFromIni("MaxDeceleration", "2000");
        }

        public void WriteMotorParam(string param, int value)
        {
            int ret = 0;
            int paramId = motorSettingDict[param];
            int paramLength = motorSettingLength[param];
            if (IsMotorOpen && MotorAddr > 0 && MotorAddr < 248)
            {
                int readValue = 0;
                ret = NiMotionSDK.NiM_readParam(MotorAddr, paramId, paramLength, ref readValue);
                if(0 != ret)
                    throw new Exception(string.Format("NiM_readParam {0} Failed [{1}]", param, ret));
                if(readValue != value) {
                    ret = NiMotionSDK.NiM_writeParam(MotorAddr, paramId, paramLength, value);
                    if (0 != ret)
                        throw new Exception(string.Format("NiM_writeParam {0} Failed [{1}]", param, ret));
                }
            }
            else
            {
                if (!IsMotorOpen)
                {
                    throw new Exception(string.Format("Motor device is not opened"));
                }else
                {
                    throw new Exception(string.Format("Motor address out of range"));
                }
            }
        }

        private void WriteMotorParams()
        {
            try
            {
                WriteMotorParam("Segmentation", Convert.ToInt32(MotorSettingModel.Segmentation));
                WriteMotorParam("StartEnd", Convert.ToInt32(MotorSettingModel.StartEnd));
                WriteMotorParam("EmergencyStop", Convert.ToInt32(MotorSettingModel.EmergencyStop));
                WriteMotorParam("FaultAction", Convert.ToInt32(MotorSettingModel.FaultAction));
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


        private int SyncMotorParam(string param)
        {
            int ret = 0;
            int paramId = motorSettingDict[param];
            int paramLength = motorSettingLength[param];
            if (IsMotorOpen && MotorAddr > 0 && MotorAddr < 248)
            {
                int readValue = 0;
                ret = NiMotionSDK.NiM_readParam(MotorAddr, paramId, paramLength, ref readValue);
                if (0 != ret)
                    throw new Exception(string.Format("NiM_readParam {0} Failed [{1}]", param, ret));

                IniFileHelper.WriteValue(Section, param, Convert.ToString(readValue));
                return readValue;
            }
            else
            {
                if (!IsMotorOpen)
                {
                    throw new Exception(string.Format("Motor Device is not opened"));
                }
                else
                {
                    throw new Exception(string.Format("Motor address out of range"));
                }
            }
        }

        private void SyncMotorParams()
        {
            try
            {
                MotorSettingModel.Segmentation = (Segment)SyncMotorParam("Segmentation");
                MotorSettingModel.StartEnd = (StopWay)SyncMotorParam("StartEnd");
                MotorSettingModel.EmergencyStop = (StopWay)SyncMotorParam("EmergencyStop");
                MotorSettingModel.FaultAction = (StopWay)SyncMotorParam("FaultAction");
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

