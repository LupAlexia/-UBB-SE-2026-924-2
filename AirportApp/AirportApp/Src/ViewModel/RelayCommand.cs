using System;
using System.Windows.Input;

namespace AirportApp.Src.ViewModel
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object>? canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<object> execute, Predicate<object>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter!);
        }

        public void Execute(object? parameter)
        {
            execute(parameter!);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
