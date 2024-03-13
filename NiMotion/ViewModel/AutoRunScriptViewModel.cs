using GalaSoft.MvvmLight;
using NiMotion.Model;
using System.Collections.ObjectModel;

namespace NiMotion.ViewModel
{
    class AutoRunScriptViewModel : ViewModelBase
    {
        private ObservableCollection<ListDataModel> dataList;
        public ObservableCollection<ListDataModel> DataList
        {
            get => dataList;
            set => Set(ref dataList, value);
        }

        private ObservableCollection<StepDataModel> stepDataList;
        public ObservableCollection<StepDataModel> StepDataList
        {
            get => stepDataList;
            set => Set(ref stepDataList, value);
        }

        private string matlabRootPath;
        public string MatlabRootPath
        {
            get => matlabRootPath;
            set
            {
                matlabRootPath = value;
                RaisePropertyChanged("MatlabRootPath");
            }
        }

        private string matDataPath;
        public string MatDataPath
        {
            get => matDataPath;
            set
            {
                matDataPath = value;
                RaisePropertyChanged("MatDataPath");
            }
        }

        public AutoRunScriptViewModel()
        {
            dataList = GetDataList();
            stepDataList = GetStepBarDataList();
            matDataPath = string.Empty;
            matlabRootPath = string.Empty;
            LEDList = new ObservableCollection<string>
            {
                "LED460",
                "LED385",
                "LED460and385"
            };
            SelectedIndex = 0;
            Cycles = "2";
            FPS = "60";
            FlushTime = "5";
            LED460Brightness = "50";
            LED385Brightness = "50";
        }

        public void AddListDataItem(string matscript_name, string matscript_path)
        {
            dataList.Add(new ListDataModel {Name = matscript_name, Path = matscript_path });
        }

        public void ClearListData()
        {
            dataList.Clear();
        }

        public void RemoveListDataItem(int index)
        {
            if (index >= 0 && index < GetListDataItemCount())
                dataList.RemoveAt(index);
        }

        public int GetListDataItemCount()
        {
            return dataList.Count;
        }

        public ListDataModel GetListItem(int index)
        {
            return dataList[index];
        }

        private string cycles;
        public string Cycles
        {
            get { return cycles; }
            set
            {
                cycles = value;
                RaisePropertyChanged("Cycles");
            }
        }

        private string fps;
        public string FPS
        {
            get { return fps; }
            set
            {
                fps = value;
                RaisePropertyChanged("FPS");
            }
        }

        private string led460Brightness;
        public string LED460Brightness
        {
            get { return led460Brightness; }
            set
            {
                led460Brightness = value;
                RaisePropertyChanged("LED460Brightness");
            }
        }

        private string led4385Brightness;
        public string LED385Brightness
        {
            get { return led4385Brightness; }
            set
            {
                led4385Brightness = value;
                RaisePropertyChanged("LED385Brightness");
            }
        }


        private string flushTime;
        public string FlushTime
        {
            get { return flushTime; }
            set
            {
                flushTime = value;
                RaisePropertyChanged("FlushTime");
            }
        }

        private ObservableCollection<string> ledList;
        public ObservableCollection<string> LEDList
        {
            get => ledList;
            set => Set(ref ledList, value);
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

        private ObservableCollection<StepDataModel> GetStepBarDataList()
        {
            return new ObservableCollection<StepDataModel>
            {
                new StepDataModel
                {
                    Name = "Start"
                },
                new StepDataModel
                {
                    Name = "PythonEnvironmentInit"
                },
                new StepDataModel
                {
                    Name = "CreatePythonScript"
                },
                new StepDataModel
                {
                    Name = "RunPythonScript"
                },
                new StepDataModel
                {
                    Name = "DLP"
                },
                new StepDataModel
                {
                    Name = "End"
                }
            };
        }



        private ObservableCollection<ListDataModel> GetDataList()
        {
            return new ObservableCollection<ListDataModel>
            {

            };
        }
    }
}
