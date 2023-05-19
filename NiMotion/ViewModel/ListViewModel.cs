using GalaSoft.MvvmLight;
using NiMotion.Model;
using System.Collections.ObjectModel;

namespace NiMotion.ViewModel
{
    class ListViewModel : ViewModelBase
    {
        private ObservableCollection<ListDataModel> dataList;
        public ObservableCollection<ListDataModel> DataList
        {
            get => dataList;
            set => Set(ref dataList, value);
        }

        public ListViewModel()
        {
            DataList = GetDataList();
        }

        public void AddListDataItem(string matscript_name, string matscript_path)
        {
            dataList.Add(new ListDataModel {Name = matscript_name, Path = matscript_path });
        }

        public void ClearListData()
        {
            dataList.Clear();
        }

        public int GetListDataItemCount()
        {
            return dataList.Count;
        }

        public ListDataModel GetListItem(int index)
        {
            return dataList[index];
        }

        private ObservableCollection<ListDataModel> GetDataList()
        {
            return new ObservableCollection<ListDataModel>
            {

            };
        }
    }
}
