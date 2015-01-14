using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruthorDare.Command;
using TruthorDare.Model;

namespace TruthorDare.ViewModel
{
    class TurntableViewModel
    {
        public DelegateCommand ShowCommand { get; set; }
        public TurntableContent turntableContent { get; set; }

        public TurntableViewModel()
        {
            turntableContent = new TurntableContent();
            ShowCommand = new DelegateCommand();
            ShowCommand.ExcuteCommand = new Action<object>(showturntableContent);
        }

        private void showturntableContent(object obj)
        {

        }
    }
}
