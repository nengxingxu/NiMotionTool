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

        //private uint minSpeed;
        //[Category("Speed/Acceleration")]
        //public uint MinSpeed
        //{
        //    get
        //    {
        //        return minSpeed;
        //    }
        //    set
        //    {
        //        minSpeed = value;
        //        RaisePropertyChanged("MinSpeed");
        //    }
        //}

        private double acceleration;
        [Category("Speed/Acceleration")]
        public double Acceleration
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

        private double deceleration;
        [Category("Speed/Acceleration")]
        public double Deceleration
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


    }
}
