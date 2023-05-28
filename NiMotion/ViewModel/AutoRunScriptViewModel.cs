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
                    Name = "CompositeVideo"
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
