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
        public int MotorAddr { get; set; }


        private bool isShowTimer;
        public bool IsShowTimer
        {
            get => isShowTimer;
            set => Set(ref isShowTimer, value);
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

        private string acceleration;
        public string Acceleration
        {
            get { return acceleration; }
            set
            {
                acceleration = value;
                RaisePropertyChanged("Acceleration");
            }
        }

        private string deceleration;
        public string Deceleration
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

        private int timing;
        public int Timing
        {
            get { return timing; }
            set
            {
                timing = value;
                RaisePropertyChanged("Timing");
            }
        }

        public MotorOperationViewModel()
        {

        }
    }
}
