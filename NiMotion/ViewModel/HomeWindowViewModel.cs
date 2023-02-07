using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiMotion.Model;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace NiMotion.ViewModel
{
    public class HomeWindowViewModel: ViewModelBase
    {
        private ObservableCollection<DataModel> dataList;
        public ObservableCollection<DataModel> DataList
        {
            get => dataList;
            set => Set(ref dataList, value);
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

        public HomeWindowViewModel()
        {
            SelectedIndex = 0;
            DataList = GetDataList();
        }

        private ObservableCollection<DataModel> GetDataList()
        {
            //string name = Properties.Lang.ResourceManager.GetString("Button");
            return new ObservableCollection<DataModel>
            {
                new DataModel{ ImgPath = "pack://application:,,,/Resource/Image/LeftMainContent/MotorOperation.png", Name = "MotorOperation"},
                new DataModel{ ImgPath = "pack://application:,,,/Resource/Image/LeftMainContent/MotorSetting.png", Name = "MotorSetting"},
                new DataModel{ ImgPath = "pack://application:,,,/Resource/Image/LeftMainContent/SystemSetting.png", Name = "SystemSetting"}
            };
        }
    }
}
