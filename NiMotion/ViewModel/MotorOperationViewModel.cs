using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using NiMotion.Model;
using System.ComponentModel;

namespace NiMotion.ViewModel
{
    public class MotorOperationViewModel : ViewModelBase 
    {
        public bool IsMotorOpen { get; set; }
        public uint MotorMaster { get; set; }
        public int MotorAddr { get; set; }


        private bool isShowTimer;
        public bool IsShowTimer
        {
            get => isShowTimer;
            set => Set(ref isShowTimer, value);
        }


        private string timing;
        public string Timing
        {
            get => timing;
            set
            {
                timing = value;
                RaisePropertyChanged("Timing");
            }
        }

        private bool isShowSpeedBar;
        public bool IsShowSpeedBar
        {
            get { return isShowSpeedBar; }
            set
            {
                isShowSpeedBar = value;
                RaisePropertyChanged("IsShowSpeedBar");
            }
        }

        private bool isShowLocationBar;
        public bool IsShowLocationBar
        {
            get { return isShowLocationBar; }
            set
            {
                isShowLocationBar = value;
                RaisePropertyChanged("IsShowLocationBar");
            }
        }

        private double motorSpeed;
        public double MotorSpeed
        {
            get { return motorSpeed; }
            set
            {
                motorSpeed = value;
                RaisePropertyChanged("MotorSpeed");
            }
        }

        private double acceleration;
        public double Acceleration
        {
            get { return acceleration; }
            set
            {
                acceleration = value;
                RaisePropertyChanged("Acceleration");
            }
        }

        private double deceleration;
        public double Deceleration
        {
            get { return deceleration; }
            set
            {
                deceleration = value;
                RaisePropertyChanged("Deceleration");
            }
        }

        private int position;
        public int Position
        {
            get { return position; }
            set
            {
                position = value;
                RaisePropertyChanged("Position");
            }
        }

        private int motorPositionSpeed;
        public int MotorPositionSpeed
        {
            get { return motorPositionSpeed; }
            set
            {
                motorPositionSpeed = value;
                RaisePropertyChanged("MotorPositionSpeed");
            }
        }

        private int sec;
        public int Sec
        {
            get { return sec; }
            set
            {
                sec = value;
                RaisePropertyChanged("sec");
            }
        }

        private int min;
        public int Min
        {
            get { return min; }
            set
            {
                min = value;
                RaisePropertyChanged("min");
            }
        }

        private int hour;
        public int Hour
        {
            get { return hour; }
            set
            {
                hour = value;
                RaisePropertyChanged("hour");
            }
        }



        public MotorOperationViewModel()
        {

        }
    }
}
