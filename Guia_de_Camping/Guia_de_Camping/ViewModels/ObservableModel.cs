using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Aspbrasil.ViewModels
{
    public class ObservableModel : INotifyPropertyChanged
    {

        public int _idLocal { get; set; }

        private bool _isBusy;

        public int IdLocal
        {
            get { return _idLocal; }
            set { _idLocal = value; }
        }


        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
        public bool IsNotBusy
        {
            get { return !_isBusy; }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
