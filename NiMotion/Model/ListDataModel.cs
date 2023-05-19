using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace NiMotion.Model
{
    public class ListDataModel : ViewModelBase
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

        public string Path { get; set; }

        public string ImgPath { get; set; }

        public ObservableCollection<ListDataModel> DataList { get; set; }


    }
}
