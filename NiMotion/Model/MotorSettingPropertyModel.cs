using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using SDKDemo;

namespace NiMotion.Model
{
    public class MotorSettingPropertyModel : NotificationBinding
    {
        private Segment segmentation;
        [Description("Segmentation"), Category("Common")]
        public Segment Segmentation
        {
            get
            {
                return segmentation;
            }
            set
            {
                segmentation = value;
                RaisePropertyChanged("Segmentation");
            }
        }

        private StopWay startEnd;
        [Description("StartEnd"), Category("Common")]
        public StopWay StartEnd
        {
            get
            {
                return startEnd;
            }
            set
            {
                startEnd = value;
                RaisePropertyChanged("StartEnd");
            }
        }

        private StopWay emergencyStop;
        [Category("Common")]
        public StopWay EmergencyStop
        {
            get
            {
                return emergencyStop;
            }
            set
            {
                emergencyStop = value;
                RaisePropertyChanged("EmergencyStop");
            }
        }

        private StopWay faultAction;
        [Category("Common")]
        public StopWay FaultAction
        {
            get
            {
                return faultAction;
            }
            set
            {
                faultAction = value;
                RaisePropertyChanged("FaultAction");
            }
        }

        private int maxSpeed;
        [Category("Speed/Acceleration")]
        public int MaxSpeed
        {
            get
            {
                return maxSpeed;
            }
            set
            {
                maxSpeed = value;
                RaisePropertyChanged("MaxSpeed");
            }
        }

        private int minSpeed;
        [Category("Speed/Acceleration")]
        public int MinSpeed
        {
            get
            {
                return minSpeed;
            }
            set
            {
                minSpeed = value;
                RaisePropertyChanged("MinSpeed");
            }
        }

        private int acceleration;
        [Category("Speed/Acceleration")]
        public int Acceleration
        {
            get
            {
                return acceleration;
            }
            set
            {
                acceleration = value;
                RaisePropertyChanged("Acceleration");
            }
        }

        private int deceleration;
        [Category("Speed/Acceleration")]
        public int Deceleration
        {
            get
            {
                return deceleration;
            }
            set
            {
                deceleration = value;
                RaisePropertyChanged("Deceleration");
            }
        }

        private int maxAcceleration;
        [Category("Speed/Acceleration")]
        public int MaxAcceleration
        {
            get
            {
                return maxAcceleration;
            }
            set
            {
                maxAcceleration = value;
                RaisePropertyChanged("MaxAcceleration");
            }
        }


        private int maxDeceleration;
        [Category("Speed/Acceleration")]
        public int MaxDeceleration
        {
            get
            {
                return maxDeceleration;
            }
            set
            {
                maxDeceleration = value;
                RaisePropertyChanged("MaxDeceleration");
            }
        }
    }

    public enum Segment
    {
        step_full,
        step_half,
        step_1_4,
        step_1_8,
        step_1_16
    }

    public enum StopWay
    {
        NoDecelerationShutdown,
        ShutdownWithCertainDeceleration
    }
}
