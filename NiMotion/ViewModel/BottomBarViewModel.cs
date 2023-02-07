using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using NiMotion.Model;
using SDKDemo;

namespace NiMotion.ViewModel
{
    public class BottomBarViewModel : ViewModelBase
    {
        public bool IsMotorOpen { get; set; }
        public int MotorAddr { get; set; }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                RaisePropertyChanged("Status");
            }
        }

        private string mode;
        public string Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                RaisePropertyChanged("Mode");
            }
        }

        private string speed;
        public string Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                RaisePropertyChanged("Speed");
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

        private string position;
        public string Position
        {
            get { return position; }
            set
            {
                position = value;
                RaisePropertyChanged("Position");
            }
        }


        public BottomBarViewModel()
        {
            status = "Status: Offline";
            mode = "Mode:    ";
            speed = "Speed:    step/s";
            acceleration = "Acceleration:    step/s2";
            deceleration = "Deceleration:    step/s2";
            position = "Position:    ";
        }
    }
}
