using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace CoupletTask
{
    public sealed class CoupletTask:IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var def = taskInstance.GetDeferral();
           
        }
    }
}
