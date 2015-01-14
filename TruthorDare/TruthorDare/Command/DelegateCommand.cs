using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TruthorDare.Command
{
   public class DelegateCommand : ICommand
    {
        public Action<object> ExcuteCommand = null;
        public Func<object, bool> CanExecuteCommand = null;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (CanExecuteCommand != null)
            {
                return this.CanExecuteCommand(parameter);
            }
            else
            {
                return true;
            }
        }

        //执行命令
        public void Execute(object parameter)
        {
            if (this.ExcuteCommand != null)
            {
                this.ExcuteCommand(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
