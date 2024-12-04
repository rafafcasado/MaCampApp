using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MaCamp.ViewModels
{
    public class ObservableModel : INotifyPropertyChanged
    {
        public int _idLocal { get; set; }
        private bool _isBusy;

        public int IdLocal
        {
            get => _idLocal;
            set => _idLocal = value;
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy => !_isBusy;
        public virtual event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}