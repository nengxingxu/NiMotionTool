using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiMotion.Model;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using System.IO.Ports;

namespace NiMotion.ViewModel
{
    public class TopBarViewModel : ViewModelBase
    {
        private ObservableCollection<string> portList;
        public ObservableCollection<string> PortList
        {
            get => portList;
            set => Set(ref portList, value);
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                RaisePropertyChanged("SelectedIndex");
            }
        }

        private ObservableCollection<string> baudList;
        public ObservableCollection<string> BaudList
        {
            get => baudList;
            set => Set(ref baudList, value);
        }

        private int selectedIndexBaud;
        public int SelectedIndexBaud
        {
            get { return selectedIndexBaud; }
            set
            {
                selectedIndexBaud = value;
                RaisePropertyChanged("selectedIndexBaud");
            }
        }

        private ObservableCollection<string> onlineMotorsList;
        public ObservableCollection<string> OnlineMotorsList
        {
            get => onlineMotorsList;
            set => Set(ref onlineMotorsList, value);
        }

        private int selectedIndexolMotors;
        public int SelectedIndexolMotors
        {
            get { return selectedIndexolMotors; }
            set
            {
                selectedIndexolMotors = value;
                RaisePropertyChanged("selectedIndexolMotors");
            }
        }

        public TopBarViewModel()
        {
            selectedIndex = 0;
            selectedIndexBaud = 7;
            selectedIndexolMotors = 0;
            portList = GetPortList();
            baudList = GetBaudList();
            onlineMotorsList = new ObservableCollection<string>();
        }

        private ObservableCollection<string> GetPortList()
        {
            string[] ArryPort = SerialPort.GetPortNames();
            List<string> ListPort = new List<string>();
            //string name = Properties.Lang.ResourceManager.GetString("Button");
            foreach (string str in ArryPort)
            {
                ListPort.Add(str);
            }
            return new ObservableCollection<string>(ListPort);
        }

        private ObservableCollection<string> GetBaudList()
        {
            return new ObservableCollection<string>
            {
                "1200",
                "2400",
                "4800",
                "9600",
                "19200",
                "38400",
                "57600",
                "115200",
                "256000"
            };
        }

        public void UpdateOnlineMotors(int[] array, int len)        {            onlineMotorsList.Clear();            for (int i = 0; i < len; i++)            {                onlineMotorsList.Add(string.Format("{0}", array[i]));            }            selectedIndexolMotors = 0;        }

        public string PortNum
        {
            get
            {
                return portList[selectedIndex];
            }
        }

        public string BaudRate
        {
            get
            {
                return baudList[selectedIndexBaud];
            }
        }

        public int MotorsNumber
        {
            get
            {
                return Convert.ToInt32(onlineMotorsList[selectedIndexolMotors]);
            }

        }
    }
}
