using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace NiMotion.Model
{
    public class DataModel : ViewModelBase
    {
        public int Index { get; set; }

        //public string Name { get; set; }
        private string name;
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }


        public bool IsSelected { get; set; }

        public string Remark { get; set; }

        public string ImgPath { get; set; }

        public ObservableCollection<DataModel> DataList { get; set; }

    }
}
