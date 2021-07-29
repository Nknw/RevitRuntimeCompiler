using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RevitRuntimeCompiler.UI
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _command;

        public RelayCommand(Action<object> command) => _command = command;

        public RelayCommand(Action command) => _command = _ => command();

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _command(parameter);

        public event EventHandler CanExecuteChanged;
    }
}
