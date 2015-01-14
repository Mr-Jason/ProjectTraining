using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using System;
using Microsoft.Phone.Shell;
using System.Linq;
using System.IO.IsolatedStorage;

namespace lockScreenTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        const string lockTextKey = "LockText";
        const string goalTimeKey = "GoalTime";
        /// <remarks>
        /// ScheduledAgent 构造函数，初始化 UnhandledException 处理程序
        /// </remarks>
        static ScheduledAgent()
        {
            // 订阅托管的异常处理程序
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// 出现未处理的异常时执行的代码
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // 出现未处理的异常；强行进入调试器
                Debugger.Break();
            }
        }

        /// <summary>
        /// 运行计划任务的代理
        /// </summary>
        /// <param name="task">
        /// 调用的任务
        /// </param>
        /// <remarks>
        /// 调用定期或资源密集型任务时调用此方法
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO:  添加用于在后台执行任务的代码
            //PeriodicTask 短时间内运行的执行任务
            if (task is PeriodicTask)
            {
                PeriodicTask periodtask = task as PeriodicTask;

                string lockText = IsolatedStorageSettings.ApplicationSettings[lockTextKey].ToString();
                string goalTime = IsolatedStorageSettings.ApplicationSettings[goalTimeKey].ToString();
                DateTime intervalDate = DateTime.Parse(goalTime);
                string interValTime = getInterValDays(ref intervalDate);

                if (periodtask != null && interValTime == "0天")
                {
#if(DEBUG)
                    ShellToast toasts = new ShellToast();
                    toasts.Title = "通知";
                    toasts.Content = goalTime;
                    toasts.Show();
#endif
                    updateLockTile(lockText + "[" + goalTime + "]");
                }
                else
                {
                    updateLockTile(lockText + periodtask.Description + interValTime);
                }
            }
#if(DEBUG)
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(10));
#endif
            NotifyComplete();
        }

        public static string getInterValDays(ref DateTime date)
        {
            try
            {
                string days = "0天";
                TimeSpan intervalTime;
                int month = date.Month;
                int day = date.Day;
                if (DateTime.Now.Year == date.Year || DateTime.Now.Year < date.Year)
                {
                    intervalTime = date - DateTime.Now;
                    if (intervalTime.Hours > 0 && intervalTime.Days == 0)
                        days = "1天";
                    else if (intervalTime.Hours > 0 || intervalTime.Minutes > 0)
                        days = (intervalTime.Days + 1).ToString() + "天";
                    else
                        days = "0天";
                }
                else if (DateTime.Now.Year > date.Year)
                {
                    //date = Convert.ToDateTime(DateTime.Now.Year + "-" + month + "-" + day);
                    //intervalTime = date - DateTime.Now;
                   // days = intervalTime.Days.ToString() + "天";
                }
                return days;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void updateLockTile(string lockText)
        {
            string temp = string.Empty;
            IconicTileData tileData = new IconicTileData();
            if (lockText.Length > 17)
            {
                tileData.WideContent1 = lockText.Substring(0, 16);
                temp = lockText.Substring(17, lockText.Length - 17);
                if (temp.Length > 17)
                {
                    tileData.WideContent2 = lockText.Substring(17, 17);
                    tileData.WideContent3 = lockText.Substring(34, lockText.Length - 17);
                }
                else
                {
                    tileData.WideContent2 = temp;
                    tileData.WideContent3 = "";
                }
            }
            else
            {
                tileData.WideContent1 = lockText;
                tileData.WideContent2 = "";
                tileData.WideContent3 = "";
            }

            Uri tile = new Uri("/", UriKind.Relative);
            ShellTile tileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(tile.ToString()));
            tileToFind.Update(tileData);
        }
    }
}