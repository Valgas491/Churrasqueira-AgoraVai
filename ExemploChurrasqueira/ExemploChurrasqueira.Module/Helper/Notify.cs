using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExemploChurrasqueira.Module.Helper
{
    public class Notify : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetPropertyValue<T>(string propertyName, ref T propertyValueHolder, T newValue)
        {
            bool locPropertyChanged = false;
            if (!EqualityComparer<T>.Default.Equals(propertyValueHolder, newValue))
            {
                propertyValueHolder = newValue;
                OnPropertyChanged(propertyName);
                locPropertyChanged = true;
            }
            return locPropertyChanged;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }
}
