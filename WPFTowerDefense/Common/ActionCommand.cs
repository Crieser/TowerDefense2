using System;
using System.Windows.Input;

namespace WPFTowerDefense.Common
{
    public class ActionCommand : ICommand
    {
        private readonly Action<object> handlerExecute;
        private readonly Func<object, bool> handlerCanExecute;

        public ActionCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            handlerExecute = execute ?? throw new ArgumentNullException("Execute cannot be null");
            handlerCanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            handlerExecute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (handlerCanExecute == null)
            {
                return true;
            }

            return handlerCanExecute(parameter);
        }
    }
}
