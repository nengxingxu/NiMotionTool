using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace NiMotion.Model
{
    public class StepDataModel : ViewModelBase
    {

        
        private string name;
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }
    }
}
