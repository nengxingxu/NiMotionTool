using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace NiMotion.Model
{
    public class MotorSettingPropertyModel : NotificationBinding
    {

        private uint maxSpeed;
        [Category("Speed/Acceleration")]
        public uint MaxSpeed
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

        private uint minSpeed;
        [Category("Speed/Acceleration")]
        public uint MinSpeed
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

        private uint acceleration;
        [Category("Speed/Acceleration")]
        public uint Acceleration
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

        private uint deceleration;
        [Category("Speed/Acceleration")]
        public uint Deceleration
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

        private uint maxAcceleration;
        [Category("Speed/Acceleration")]
        public uint MaxAcceleration
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


        private uint maxDeceleration;
        [Category("Speed/Acceleration")]
        public uint MaxDeceleration
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
}
