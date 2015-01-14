using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LockTextScreen.Resources;
using lockScreen.Utility;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Scheduler;
using Windows.System;
using Microsoft.Phone.Tasks;

namespace LockTextScreen
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
        }
        const string timingTask_Name = "LockAgent";
        const  string lockTextKey = "LockText";
        const  string goalTimeKey = "GoalTime";
        string strTemplate = "距{0}还有{1}";
        bool updateMake = true;
        string intervalDays = string.Empty;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (IsoSettingsHelper.checkKey(lockTextKey))
                {
                    this.tbContent.Text = IsoSettingsHelper.getValueByKey(lockTextKey).ToString();

                    if (IsoSettingsHelper.checkKey(goalTimeKey))
                    {
                        DateTime dt = Convert.ToDateTime(IsoSettingsHelper.getValueByKey(goalTimeKey));
                        updateMake = false;
                        this.datePicker.Value = (DateTime?)dt;
                        updateMake = true;
                        intervalDays = getInterValDays(ref dt);
                        this.tbNeedDays.Text = strTemplate.Replace("{0}", dt.ToShortDateString()).Replace("{1}", intervalDays);
                        toggleSwith.IsChecked = true;
                        datePicker.IsEnabled = true;
                        strTemplate = "距{0}还有{1}";
                    }
                    else
                    {
                        this.tbNeedDays.Text = "距" + DateTime.Now.ToShortDateString() + "还有0天";
                    }
                }
                base.OnNavigatedTo(e);
            }
            catch (Exception ex)
            {
                if (MessageBoxResult.OK == MessageBox.Show("抱歉：程序闹情绪了...是否反馈给作者,帮您修理它？", "温馨提示", MessageBoxButton.OKCancel))
                {
                    this.sendFeedBack(ex.Message);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
        }

        private void btnDefine_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbContent.Text.Trim()))
            {
                ToastPrompt toastPrompt = new ToastPrompt
                {
                    Message = "请输入锁屏内容",
                    MillisecondsUntilHidden = 1200
                };
                toastPrompt.Show();
                return;
            }
            try
            {

                if ((bool)toggleSwith.IsChecked && intervalDays != "0天")
                {
                    PeriodicTask TimingTask = null;
                    TimingTask = ScheduledActionService.Find(timingTask_Name) as PeriodicTask;
                    if (TimingTask != null)
                    {
                        ScheduledActionService.Remove(timingTask_Name);
                    }
                    TimingTask = new PeriodicTask(timingTask_Name);
                    TimingTask.Description = this.tbContent.Text.Trim() + "|" + IsoSettingsHelper.getValueByKey(goalTimeKey);
                    ScheduledActionService.Add(TimingTask);
                }
               
                ShellTile shellTitle = Enumerable.First<ShellTile>(ShellTile.ActiveTiles);
                if (shellTitle != null)
                {
                    StandardTileData standardTitleData = new StandardTileData();

                    if ((bool)toggleSwith.IsChecked && (!string.IsNullOrEmpty(intervalDays) && intervalDays != "0天"))
                        standardTitleData.BackContent = this.tbContent.Text.Trim() + "还有" + intervalDays;
                    else
                    {
                      
                        standardTitleData.BackContent = this.tbContent.Text.Trim().ToString();
                    }
                    shellTitle.Update(standardTitleData);
                }
                IsoSettingsHelper.SavaSetting(lockTextKey, this.tbContent.Text.Trim());

                ToastPrompt toastPrompt = new ToastPrompt
                {
                    Message = "温馨提示：锁屏记事设置成功",
                    MillisecondsUntilHidden = 1200
                };
                toastPrompt.Show();
                #if(DEBUG)
                   ScheduledActionService.LaunchForTest(timingTask_Name, TimeSpan.FromSeconds(20));
                #endif
            }
            catch (InvalidOperationException ie)
            {
                if (ie.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("非常抱歉，本应用的后台计划已被禁用，无法开启倒计时提醒，请查看使用帮助。", "温馨提示", MessageBoxButton.OK);
                }

                if (ie.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    MessageBox.Show("非常抱歉，您的手机定期代理数量达到最大限制。", "温馨提示", MessageBoxButton.OK);
                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
            catch(Exception ex)
            {
                if (MessageBoxResult.OK == MessageBox.Show("抱歉：程序闹情绪了...是否反馈给作者,帮您修理它？", "温馨提示", MessageBoxButton.OKCancel))
                {
                    this.sendFeedBack(ex.Message);
                }
            }
        }

        private async void btnSettings_Click(object sender, EventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
        }

        //删除记事
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (IsoSettingsHelper.checkKey(lockTextKey))
            {
                if (MessageBoxResult.Cancel == MessageBox.Show("您确定要删除当前提醒，[" + IsoSettingsHelper.getValueByKey(lockTextKey).ToString() + "]", "温馨提示", MessageBoxButton.OKCancel))
                {
                    return;
                }
            }
            try
            {
                PeriodicTask TimingTask = null;
                TimingTask = ScheduledActionService.Find(timingTask_Name) as PeriodicTask;
                if (TimingTask != null)
                {
                    ScheduledActionService.Remove(timingTask_Name);
                }
                if (IsoSettingsHelper.checkKey(lockTextKey))
                {   
                    IsoSettingsHelper.DeleSetting(lockTextKey);
                    IsoSettingsHelper.DeleSetting(goalTimeKey);
                    updateMake = false;
                    this.datePicker.Value = (DateTime?)DateTime.Now;
                    this.tbNeedDays.Text = "距" + DateTime.Now.ToShortDateString() + "还有0天";
                    toggleSwith.IsChecked = false;
                    datePicker.IsEnabled = false;
                  //  this.tbContent.Text = "";
                    ShellTile shellTitle = Enumerable.First<ShellTile>(ShellTile.ActiveTiles);
                    if (shellTitle != null)
                    {
                        StandardTileData standardTitleData = new StandardTileData();
                        standardTitleData.BackContent = "";
                        shellTitle.Update(standardTitleData);
                    }
                    updateMake = true;

                    ToastPrompt toastPrompt = new ToastPrompt
                    {
                        Message = "温馨提示：锁屏记事清除成功",
                        MillisecondsUntilHidden = 1200
                    };
                    toastPrompt.Show();
                }
                else
                {
                    ToastPrompt toastPrompt = new ToastPrompt
                    {
                        Message = "温馨提示：当前没有要清除的锁屏记事",
                        MillisecondsUntilHidden = 1200
                    };
                    toastPrompt.Show();
                }
            }
            catch (Exception ex)
            {
                if (MessageBoxResult.OK == MessageBox.Show("抱歉：程序闹情绪了...是否反馈给作者,帮您修理它？", "温馨提示", MessageBoxButton.OKCancel))
                {
                    this.sendFeedBack(ex.Message);
                }
            }
        }

        private void menuMyApps_Click(object sender, EventArgs e)
        {
            MarketplaceSearchTask marketplaceSearchTask = new MarketplaceSearchTask();
            marketplaceSearchTask.ContentType = MarketplaceContentType.Applications;
            marketplaceSearchTask.SearchTerms = "星爵";
            MarketplaceSearchTask marketplaceSearchTask2 = marketplaceSearchTask;
            marketplaceSearchTask2.Show();
        }

        private void menuFeedback_Click(object sender, EventArgs e)
        {
            this.sendFeedBack();
        }

        private void menuReview_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void sendFeedBack(string feedContent="")
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "alei.jason@outlook.com";
            emailComposeTask.Subject = "锁屏倒计时记事用户反馈[V 1.0.2.0]";
            emailComposeTask.Body = feedContent;
            EmailComposeTask emailComposeTask2 = emailComposeTask;
            emailComposeTask2.Show();
        }

        private void toggleSwith_Click(object sender, RoutedEventArgs e)
        {
            ToggleSwitch OpenOrOFF = sender as ToggleSwitch;
            if ((bool)OpenOrOFF.IsChecked)
                datePicker.IsEnabled = true;
            else
                datePicker.IsEnabled = false;
        }

        private void datePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (!updateMake)
                return;

            DateTime date = (DateTime)e.NewDateTime;

            if ((date - DateTime.Now).Days <= 0 && (date - DateTime.Now).Hours <= 0)
            {
                MessageBox.Show("亲，请您重新选择一个将来的时间", "温馨提示", MessageBoxButton.OK);
                return;
            }
            intervalDays = string.Empty;
            intervalDays = getInterValDays(ref date);
            strTemplate = "距{0}还有{1}";
            strTemplate = strTemplate.Replace("{0}", date.ToShortDateString()).Replace("{1}", intervalDays);

            if (intervalDays != "0天")
                IsoSettingsHelper.SavaSetting(goalTimeKey, date.ToShortDateString());
            else
                IsoSettingsHelper.DeleSetting(goalTimeKey);
            this.tbNeedDays.Text = strTemplate;
        }

        public static string getInterValDays(ref DateTime date)
        {
            string days = "0天";
            TimeSpan intervalTime;
            int month = date.Month;
            int day = date.Day;
            if ((DateTime.Now.Year == date.Year && DateTime.Now.Day <= day) || DateTime.Now.Year < date.Year)
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
                date = Convert.ToDateTime(DateTime.Now.Year + "-" + month + "-" + day);
                intervalTime = date - DateTime.Now;
                days = intervalTime.Days.ToString() + "天";
            }
            return days;
        }
    }
}