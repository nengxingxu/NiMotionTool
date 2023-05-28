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

        public HomeWindowViewModel(Dictionary<string, string> name_dict)
        {
            SelectedIndex = 0;
            DataList = GetDataList(name_dict);
        }

        private ObservableCollection<DataModel> GetDataList(Dictionary<string, string> name_dict)
        {
            return new ObservableCollection<DataModel>
            {
                new DataModel{ ImgPath = "pack://application:,,,/Resource/Image/LeftMainContent/MotorOperation.png", Name = name_dict["MotorOperation"]},
                new DataModel{ ImgPath = "pack://application:,,,/Resource/Image/LeftMainContent/MotorSetting.png", Name = name_dict["MotorSetting"]},
                new DataModel{ ImgPath = "pack://application:,,,/Resource/Image/LeftMainContent/AutoRunScript.png", Name = name_dict["AutoRunScript"]},
                new DataModel{ ImgPath = "pack://application:,,,/Resource/Image/LeftMainContent/SystemSetting.png", Name = name_dict["SystemSetting"]}
            };
        }
    }
}
