using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReferenceVisualizer.WpfApp
{
    public class FuncCommand : ICommand
    {   
        private readonly Action executeAction;
        private bool canExecute;

        public FuncCommand(Action executeAction, bool canExecute = true)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            executeAction();
        }
        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void ChangeCanExecuteChanged(bool newValue)
        {
            canExecute = newValue;

            if(CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty); 
        }
    }
}
