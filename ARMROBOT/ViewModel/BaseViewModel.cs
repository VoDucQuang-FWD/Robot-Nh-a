using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ARMROBOT.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual bool SetProperty<T>(ref T storge, T value, [CallerMemberName] string propertyName = null) 
        {
            if (Equals(storge, value)) return false;
            storge = value;
            OnPropertyChanged(propertyName);

            return true;
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    class RelayCommand<T> : ICommand
    {
        //public event EventHandler CanExecuteChanged;
        private readonly Predicate<T> _canExcute;
        private readonly Action<T> _exeCute;

        public RelayCommand(Predicate<T> canExcute, Action<T> execute) 
        {
            _canExcute = canExcute;
            _exeCute = execute;
        }
        public event EventHandler CanExecuteChanged 
        {
            add 
            {
                CommandManager.RequerySuggested += value;
            }
            remove 
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        public bool CanExecute(object parameter)
        {
            try
            {
                return _canExcute == null ? true : _canExcute((T)parameter);
            }
            catch
            {

                return true;
            }
        }

        public void Execute(object parameter)
        {
            _exeCute((T)parameter);
        }
    }
}
