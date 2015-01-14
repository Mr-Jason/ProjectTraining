using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using lockScreen.Utility;
using Microsoft.Phone.Tasks;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Scheduler;
using Windows.System;
using LockTextScreen.Utility;
using LockTextScreen.Model;
using System.Windows.Media;
using Windows.Phone.System.UserProfile;
using System.ComponentModel;
using System.Windows.Data;
using LockTextScreen.Resources;
using System.Reflection;
using Microsoft.Phone.Info;

namespace LockTextScreen
{
    public partial class LockScreenPage : PhoneApplicationPage
    {
        const string timingTask_Name = "LockAgent";
        const string lockTextKey = "LockText";
        const string goalTimeKey = "GoalTime";
        string strTemplate = AppResources.LockTipTemplate;
        bool updateMake = true;
        string intervalDays = "0";
        bool isExit = false;
        bool isOpen = true;
        TileTemplate tile;
        largeTileTemplate largeTile;

        //private ApplicationBarIconButton selectButton;
        private ApplicationBarIconButton acceptButton;
        private ApplicationBarIconButton cancelButton;
        private ApplicationBarIconButton sureButton;
        private ApplicationBarIconButton settingButton;
        private ApplicationBarIconButton deleteButton;

        private ApplicationBarMenuItem feedBackMenuItem;
        private ApplicationBarMenuItem reviewMenuItem;
        private ApplicationBarMenuItem otherAppMenuItem;
        private ApplicationBarMenuItem goaiyingyongMenuItem;

        private ApplicationBarMenuItem selectAllMenuItem;
        private ApplicationBarMenuItem unselectAllMenuItem;

        //ICollectionView thingsCollectionView;

        private readonly TileHelper _tileHelper = new TileHelper();
        public LockScreenPage()
        {
            InitializeComponent();
            isExit = false;
            ApplicationBar = new ApplicationBar();
            Color c = new Color();
            c.A = 255;
            c.B = 14;
            c.G = 192;
            c.R = 104;
            ApplicationBar.BackgroundColor = c;
            Loaded += LockScreenPage_Loaded;

            BuildLocalizedApplicationBar();
        }

        void LockScreenPage_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

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
                        remind.IsChecked = true;
                        datePicker.IsEnabled = true;
                        strTemplate = AppResources.LockTipTemplate;
                    }
                    else
                    {
                        //if (!string.IsNullOrEmpty(this.tbNeedDays.Text))
                            //this.tbNeedDays.Text = AppResources.LockTipTemplate.Replace("{0}", DateTime.Now.ToShortDateString()).Replace("{1}","0");//"距" + DateTime.Now.ToShortDateString() + "还有0天";
                    }
                }
                this.MyAdItem.start();
                base.OnNavigatedTo(e);
            }
            catch (Exception ex)
            {
                if (MessageBoxResult.OK == MessageBox.Show(AppResources.ErrorTip, AppResources.MessageTitle, MessageBoxButton.OKCancel))
                {
                    this.sendFeedBack(ex.Message);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.MyAdItem.stop();
            base.OnNavigatedFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (!isOpen)
            {
                isOpen = true;
                e.Cancel = true;
            }
            if (this.lbThingsHistory.IsSelectionEnabled)
            {
                this.lbThingsHistory.IsSelectionEnabled = false;
                e.Cancel = true;
                return;
            }
            if (!isExit && isOpen)
            {
                isExit = true;
                double width = 190;
                if (AppResources.ExitTip.IndexOf("to") > 0)
                {
                    width = 210;
                }
                var toast = new ToastPrompt
                {
                    Message = AppResources.ExitTip,
                    Margin=new Thickness(ActualWidth/2-220,ActualHeight/2-40,0,0),
                    Width=width,
                    VerticalContentAlignment=VerticalAlignment.Center,
                    HorizontalContentAlignment=HorizontalAlignment.Stretch,
                    MillisecondsUntilHidden = 1500
                };
                lockPivot.IsEnabled = false;
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
                toast.Completed += (o, ex) =>
                {
                    isExit = false;
                    if (ApplicationBar.Buttons.Count > 0)
                        ApplicationBar.Mode = ApplicationBarMode.Default;
                    lockPivot.IsEnabled = true;
                };
                toast.Show();
                e.Cancel = true;
            }
            else
            {
                isOpen = true;
                NavigationService.RemoveBackEntry(); 
            }
        }

        //设置锁屏记事
        private void btnDefine_Click(object sender, EventArgs e)
        {
            #region 非空判断
            if (string.IsNullOrEmpty(this.tbContent.Text.Trim()))
            {
                ToastPrompt toastPrompt = new ToastPrompt
                {
                    Message = AppResources.InputTip,
                    MillisecondsUntilHidden = 1200
                };
                toastPrompt.Show();
                return;
            }
            #endregion

            
            string intervalDate=string.Empty;
            try
            {
                #region 代理
                PeriodicTask TimingTask = null;
                TimingTask = ScheduledActionService.Find(timingTask_Name) as PeriodicTask;
                if (TimingTask != null)
                {
                    ScheduledActionService.Remove(timingTask_Name);
                }
                if ((bool)remind.IsChecked && intervalDays != "0")
                {
                    TimingTask = new PeriodicTask(timingTask_Name);
                    TimingTask.Description = AppResources.StillDay;
                    ScheduledActionService.Add(TimingTask);
                    intervalDate = IsoSettingsHelper.getValueByKey(goalTimeKey).ToString();
                }
                #endregion

                #region 设置锁屏显示文本
                    #region 动态磁贴方式创建
                    //ShellTile shellTitle = Enumerable.First<ShellTile>(ShellTile.ActiveTiles);

                    //if (shellTitle != null)
                    //{
                    //    StandardTileData standardTitleData = new StandardTileData();

                    //    if ((bool)remind.IsChecked && (!string.IsNullOrEmpty(intervalDays) && intervalDays != "0天"))
                    //    {
                    //        standardTitleData.BackContent = this.tbContent.Text.Trim() + "还有" + intervalDays;
                    //    }
                    //    else
                    //    {
                    //        IsoSettingsHelper.DeleSetting(goalTimeKey);
                    //        standardTitleData.BackContent = this.tbContent.Text.Trim().ToString();
                    //    }
                    //    shellTitle.Update(standardTitleData);
                    //}
                    #endregion

                    #region IconicTileData
                        //string showText = this.tbContent.Text.Trim();
                        //if (showText.Substring(showText.Length - 2) == "还有" || showText.Substring(showText.Length - 2) == "還有")
                        //{
                        //    showText = showText.Substring(0,showText.Length-2);
                        //}
                        if ((bool)remind.IsChecked && (!string.IsNullOrEmpty(intervalDays) && intervalDays != "0"))
                        {
                            _tileHelper.SetLockScreenText(this.tbContent.Text.Trim() + AppResources.StillDay + intervalDays + AppResources.Day);
                        }
                        else
                        {
                            IsoSettingsHelper.DeleSetting(goalTimeKey);
                            _tileHelper.SetLockScreenText(this.tbContent.Text.Trim().ToString());
                        }
                    #endregion
                #endregion

                localDataBaseHelper.Add(this.tbContent.Text.Trim(), intervalDate,AppResources.SetDate);
                IsoSettingsHelper.SavaSetting(lockTextKey, this.tbContent.Text.Trim());

                ToastPrompt toastPrompt = new ToastPrompt
                {
                    Message = AppResources.SetSuccessTip,
                    MillisecondsUntilHidden = 1200
                };
                toastPrompt.Show();

                #region
                if ((bool)tsFixStartScreen.IsChecked)
                {
                    createTile(this.tbContent.Text, intervalDate);
                   
                }
                #endregion

                #if(DEBUG)
                    ScheduledActionService.LaunchForTest(timingTask_Name, TimeSpan.FromSeconds(20));
                #endif
            }
            catch (InvalidOperationException ie)
            {
                if (ie.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show(AppResources.SystemErrorOne, AppResources.MessageTitle, MessageBoxButton.OK);
                }

                if (ie.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    MessageBox.Show(AppResources.SystemErrorTwo,  AppResources.MessageTitle, MessageBoxButton.OK);
                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
            catch (Exception ex)
            {
                this.SendFeedBack(ex);
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
                if (MessageBoxResult.Cancel == MessageBox.Show(AppResources.RemoveTip.Replace("{0}", IsoSettingsHelper.getValueByKey(lockTextKey).ToString()), AppResources.MessageTitle, MessageBoxButton.OKCancel))
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
                    intervalDays = "0";
                    this.tbNeedDays.Text = "";
                    remind.IsChecked = false;
                    tsFixStartScreen.IsChecked = false;
                    datePicker.IsEnabled = false;
                    //  this.tbContent.Text = "";

                    #region 删除锁屏文本
                        _tileHelper.DeleteLockScreenText();
                    #endregion
                    updateMake = true;

                    #region 删除创建的磁贴
                    ShellTile newTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DyncTile.png"));
                    if (newTile != null)
                    {
                        newTile.Delete();
                    }
                    #endregion

                    ToastPrompt toastPrompt = new ToastPrompt
                    {
                        Message = AppResources.RemoveSuccessTip,
                        MillisecondsUntilHidden = 1000
                    };
                    toastPrompt.Show();
                }
                else
                {
                    ToastPrompt toastPrompt = new ToastPrompt
                    {
                        Message = AppResources.NoLockContentTip,
                        MillisecondsUntilHidden = 1000
                    };
                    toastPrompt.Show();
                }
            }
            catch (Exception ex)
            {
                if (MessageBoxResult.OK == MessageBox.Show(AppResources.ErrorTip, AppResources.MessageTitle, MessageBoxButton.OKCancel))
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

        private void sendFeedBack(string feedContent = "")
        {
            try
            {
                string fullName = Assembly.GetExecutingAssembly().FullName;
                string appVersion = fullName.Split('=')[1].Split(',')[0];
                string body = string.Format(AppResources.FeedBackBody,
                                                            DeviceStatus.DeviceName,
                                                            DeviceStatus.DeviceManufacturer,
                                                            DeviceStatus.DeviceFirmwareVersion,
                                                            DeviceStatus.DeviceHardwareVersion
                                                        );
                EmailComposeTask emailComposeTask = new EmailComposeTask();
                emailComposeTask.To = "alei.jason@outlook.com";
                emailComposeTask.Subject = AppResources.SendFeedBack.Replace("{0}", appVersion);
                emailComposeTask.Body = feedContent + Environment.NewLine + body;
                EmailComposeTask emailComposeTask2 = emailComposeTask;
                emailComposeTask2.Show();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void toggleSwith_Click(object sender, RoutedEventArgs e)
        {
            ToggleSwitch OpenOrOFF = sender as ToggleSwitch;
            if ((bool)OpenOrOFF.IsChecked)
                datePicker.IsEnabled = true;
            else
            {
                    intervalDays = "0";
                    datePicker.IsEnabled = false;
            }
        }

        private void datePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            if (!updateMake)
                return;

            DateTime date = (DateTime)e.NewDateTime;

            if ((date - DateTime.Now).Days <= 0 && (date - DateTime.Now).Hours <= 0 && (date - DateTime.Now).Minutes <= 0)
            {
                MessageBox.Show(AppResources.chooseDateTip, AppResources.MessageTitle, MessageBoxButton.OK);
                this.tbNeedDays.Text = string.Empty;
                intervalDays = "0";
                return;
            }
            intervalDays = string.Empty;
            intervalDays = getInterValDays(ref date);
            strTemplate = AppResources.LockTipTemplate;
            strTemplate = strTemplate.Replace("{0}", date.ToShortDateString()).Replace("{1}", intervalDays);

            if (intervalDays != "0")
                IsoSettingsHelper.SavaSetting(goalTimeKey, date.ToShortDateString());
            else
                IsoSettingsHelper.DeleSetting(goalTimeKey);
            this.tbNeedDays.Text = strTemplate;
        }

        public static string getInterValDays(ref DateTime date)
        {
            string days = "0";
            TimeSpan intervalTime;
            int month = date.Month;
            int day = date.Day;
            if (DateTime.Now.Year == date.Year || DateTime.Now.Year < date.Year)
            {
                intervalTime = date - DateTime.Now;
                if (intervalTime.Hours > 0 && intervalTime.Days == 0)
                    days = "1";
                else if (intervalTime.Hours > 0 || intervalTime.Minutes > 0)
                    days = (intervalTime.Days + 1).ToString();
                else
                    days = "0";
            }
            else if (DateTime.Now.Year > date.Year)
            {
                date = Convert.ToDateTime(DateTime.Now.Year + "-" + month + "-" + day);
                intervalTime = date - DateTime.Now;
                days = intervalTime.Days.ToString();
            }
            return days;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int selectedIndex = ((Pivot)sender).SelectedIndex;
                if (selectedIndex == 2)
                {
                    lbThingsHistory.Width = ActualWidth - 20;
                    this.BindHistoryList();
                }

                if (selectedIndex == 0)
                {
                    ShowDefaultAppBar();
                }
                else
                {
                    ShowNormalStateAppBar(false);
                }
            }
            catch (Exception ex)
            {
                this.SendFeedBack(ex);
            }
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Things thing = ((LockTextScreen.Model.Things)((sender as MenuItem).DataContext)) as Things;
                int id = thing.Id;
                if (MessageBoxResult.OK == MessageBox.Show(AppResources.DeleteTip, AppResources.MessageTitle, MessageBoxButton.OKCancel))
                {
                    localDataBaseHelper.Delete(id);
                    this.BindHistoryList();
                }
                isOpen = true;
            }
            catch (Exception ex)
            {
                this.SendFeedBack(ex);
            }
        }

        /// <summary>
        /// 将当前历史记录添加到当前锁屏提醒内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Things thing = ((LockTextScreen.Model.Things)((sender as MenuItem).DataContext)) as Things;
                Clipboard.SetText(thing.Content);
                isOpen = true;
            }
            catch (Exception ex)
            {
                this.SendFeedBack(ex);
            }
        }

        private void BindHistoryList()
        {  
            List<Things> things = localDataBaseHelper.Query().ToList<Things>();
            //CollectionViewSource thingsOptionSource = new CollectionViewSource();
            //thingsOptionSource.Source = things;
            //this.thingsCollectionView = thingsOptionSource.View;
            this.lbThingsHistory.ItemsSource = things;

            if (things.Count > 0)
            {
                this.tbEmptyTip.Visibility = System.Windows.Visibility.Collapsed;
                this.tbhistoryCount.Text = AppResources.HistoryCountTemplate.Replace("{0}", things.Count.ToString());
                this.tbhistoryCount.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.tbEmptyTip.Visibility = System.Windows.Visibility.Visible;
                this.tbhistoryCount.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private async void goaiyingyong_Click(object sender, EventArgs e)
        {
            var success = await Windows.System.Launcher.LaunchUriAsync(new Uri("xfaiyingyong:;1;d56def9b-254d-4502-85e5-e6308cd6055f;;;"));
        }

        private void SendFeedBack(Exception ex)
        {
            if (MessageBoxResult.OK == MessageBox.Show(AppResources.ErrorTip, AppResources.MessageTitle, MessageBoxButton.OKCancel))
            {
                this.sendFeedBack(ex.Message);
            }
        }

        private async void tsFixStartScreen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleSwitch OpenOrOFF = sender as ToggleSwitch;
                if ((bool)OpenOrOFF.IsChecked)
                {
                    await _tileHelper.BuildFolder();
                }
            }
            catch (Exception ex)
            {
                sendFeedBack(ex.Message);
            }
        }

        /// <summary>
        /// 159*159 Tile
        /// </summary>
        /// <param name="content">显示的内容</param>
        /// <param name="date">显示的日期</param>
        private void createTile(string content,string date)
        {
            try
            {
                if (content.Length <=28)
                {
                    #region 159*159
                    tile = new TileTemplate();
                    tile.tbLockContent.Text = content;
                    if (!string.IsNullOrEmpty(date))
                        tile.tbDate.Text = "日期：" + date;

                    tile.Height = 159;
                    tile.Width = 159;
                    tile.Margin = new Thickness(0, -1000, 0, 0);
                    tile.Measure(new Size(159, 159));
                    tile.UpdateLayout();
                    #endregion
                }
                else
                {
                    #region 336*691
                    largeTile = new largeTileTemplate();
                    largeTile.tbLockContent.Text = content;
                    if (!string.IsNullOrEmpty(date))
                        largeTile.tbDate.Text = "日期：" + date;
                    largeTile.Height = 336;
                   // largeTile.Width = 159;
                    largeTile.Margin = new Thickness(0, -2000, 0, 0);
                   // largeTile.Measure(new Size(159, 159));
                    largeTile.UpdateLayout();
                    #endregion
                }

                if (tile!=null && !LayoutRoot.Children.Contains(tile))
                {
                    LayoutRoot.Children.Add(tile);
                }
                if (largeTile!=null && !LayoutRoot.Children.Contains(largeTile))
                {
                    LayoutRoot.Children.Add(largeTile);
                }
                LayoutRoot.UpdateLayout();

                if (tile != null)
                    _tileHelper.AddTile(tile,false);
                else
                    _tileHelper.AddTile(largeTile,true);
            }
            catch (Exception ex)
            {
                sendFeedBack(ex.Message);
            }
        }

        private void GestureListener_Hold(object sender, GestureEventArgs e)
        {
            isOpen = false;
        }

        //private void lbThingsHistory_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    if (this.lbThingsHistory.IsSelectionEnabled)
        //    {
        //        return;
        //    }

        //    DependencyObject tappedElement = e.OriginalSource as UIElement;
        //    // find parent UI element of type MultiselectItem
        //    MultiselectItem tappedItem = this.FindParentOfType<MultiselectItem>(tappedElement);
        //    Things selectedItem = null;
        //    if (tappedItem != null)
        //    {
        //        selectedItem = tappedItem.DataContext as Things;
        //    }

        //    // if selected data item is not null
        //    if (selectedItem != null)
        //    {
        //        MessageBox.Show(string.Format("Item with title '{0}' was tapped", selectedItem.Id));
        //    }
        //}

        private T FindParentOfType<T>(DependencyObject element) where T : DependencyObject
        {
            T result = null;
            DependencyObject currentElement = element;
            while (currentElement != null)
            {
                result = currentElement as T;
                if (result != null)
                {
                    break;
                }
                currentElement = VisualTreeHelper.GetParent(currentElement);
            }

            return result;
        }

        private bool updateSelectedState = true;
        private void lbThingsHistory_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.lbThingsHistory.IsSelectionEnabled)
            {
                //if (this.updateSelectedState)
                //{
                //    //IEnumerable<Things> thingOptions = this.thingsCollectionView.SourceCollection as IEnumerable<Things>;
                //    //this.SetOptionsSelected(thingOptions, true, (thing) => thing.IsSelected);
                //}
                this.ShowSelectionStateAppBar();
            }
            else
            {
                this.ShowNormalStateAppBar(true);
            }
        }

        /// <summary>
        /// 取消多选状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cancelButton_Click(object sender, EventArgs e)
        {
            this.lbThingsHistory.IsSelectionEnabled = false;
        }

        /// <summary>
        /// 删除所有历史记事
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void acceptButton_Click(object sender, EventArgs e)
        {
            if (MessageBoxResult.OK == MessageBox.Show(AppResources.DeleteAllTip, AppResources.MessageTitle, MessageBoxButton.OKCancel))
            {
                foreach (object item in this.lbThingsHistory.SelectedItems)
                {
                    Things thing = item as Things;
                    localDataBaseHelper.Delete(thing.Id);
                }
                this.lbThingsHistory.IsSelectionEnabled = false;
                this.BindHistoryList();
            }       
        }

        //取消全选
        void unselectAllMenuItem_Click(object sender, EventArgs e)
        {
            //IEnumerable<Things> thingOptions = this.thingsCollectionView.SourceCollection as IEnumerable<Things>;
            //this.SetOptionsSelected(thingOptions, false, null);
            this.SetOptionsSelected(false);
        }

        //全部选中
        void selectAllMenuItem_Click(object sender, EventArgs e)
        {
            //IEnumerable<Things> thingOptions = this.thingsCollectionView.SourceCollection as IEnumerable<Things>;
            this.SetOptionsSelected(true);
        }

        private void SetOptionsSelected(bool selected)
        {
            //if (thingOptions == null)
            //{
            //    return;
            //}

            //if (predicate == null)
            //{
            //    predicate = (thing) => true;
            //}
            ItemCollection ic = this.lbThingsHistory.Items;
            ItemContainerGenerator itemContainerGenerator = this.lbThingsHistory.ItemContainerGenerator;
            foreach (Things thing in ic)
            {
                //if (Option != null && predicate(Option))
                //{
                    DependencyObject visualItem = itemContainerGenerator.ContainerFromItem(thing);
                    MultiselectItem multiselectItem = visualItem as MultiselectItem;
                    if (multiselectItem != null)
                    {
                        // NOTE: this will also add an item to the SelectedItems collection
                        multiselectItem.IsSelected = selected;
                    }
                //}
            }
        }

        private void ShowNormalStateAppBar(bool isClear)
        {
            this.ApplicationBar.Mode = ApplicationBarMode.Minimized;
            this.ApplicationBar.Buttons.Clear();
            if (isClear)
            {
                this.ApplicationBar.MenuItems.Clear();
                ShowMenuItem();
            }
        }

        private void ShowSelectionStateAppBar()
        {
            this.ApplicationBar.Mode = ApplicationBarMode.Default;
            this.ApplicationBar.Buttons.Clear();
            this.ApplicationBar.MenuItems.Clear();

            this.ApplicationBar.Buttons.Add(this.acceptButton);
            this.ApplicationBar.Buttons.Add(this.cancelButton);

            this.ApplicationBar.MenuItems.Add(this.selectAllMenuItem);
            this.ApplicationBar.MenuItems.Add(this.unselectAllMenuItem);
        }

        private void BuildLocalizedApplicationBar()
        {
            #region
            this.sureButton = new ApplicationBarIconButton();
            this.sureButton.IconUri = new Uri("Assets/AppBarIcon/check.png", UriKind.RelativeOrAbsolute);
            this.sureButton.Text = AppResources.AppBarSureBtnText;
            this.sureButton.Click += new EventHandler(btnDefine_Click);

            this.settingButton = new ApplicationBarIconButton();
            this.settingButton.IconUri = new Uri("Assets/AppBarIcon/settings.png", UriKind.RelativeOrAbsolute);
            this.settingButton.Text = AppResources.AppBarSettingBtnText;
            this.settingButton.Click += new EventHandler(btnSettings_Click);

            this.deleteButton = new ApplicationBarIconButton();
            this.deleteButton.IconUri = new Uri("Assets/AppBarIcon/delete.png", UriKind.RelativeOrAbsolute);
            this.deleteButton.Text = AppResources.AppBarDeleteBtnText;
            this.deleteButton.Click += new EventHandler(btnDelete_Click);

            this.feedBackMenuItem = new ApplicationBarMenuItem();
            this.feedBackMenuItem.Text = AppResources.AppBarFeedBack;
            this.feedBackMenuItem.Click += new EventHandler(menuFeedback_Click);

            this.reviewMenuItem = new ApplicationBarMenuItem();
            this.reviewMenuItem.Text = AppResources.AppBarEvaluation;
            this.reviewMenuItem.Click += new EventHandler(menuReview_Click);

            this.otherAppMenuItem = new ApplicationBarMenuItem();
            this.otherAppMenuItem.Text = AppResources.AppBarOtherApp;
            this.otherAppMenuItem.Click += new EventHandler(menuMyApps_Click);

            this.goaiyingyongMenuItem = new ApplicationBarMenuItem();
            this.goaiyingyongMenuItem.Text = AppResources.AppBaraiyingyong;
            this.goaiyingyongMenuItem.Click += new EventHandler(goaiyingyong_Click);

            this.acceptButton = new ApplicationBarIconButton();
            this.acceptButton.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Delete.png", UriKind.RelativeOrAbsolute);
            this.acceptButton.Text = AppResources.DeleteMenuItem;
            this.acceptButton.Click += new EventHandler(acceptButton_Click);

            this.cancelButton = new ApplicationBarIconButton();
            this.cancelButton.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.RelativeOrAbsolute);
            this.cancelButton.Text = AppResources.CancelMenuItem;
            this.cancelButton.Click += new EventHandler(cancelButton_Click);

            this.selectAllMenuItem = new ApplicationBarMenuItem();
            this.selectAllMenuItem.Text = AppResources.AllSelectMenuItem;
            this.selectAllMenuItem.Click += new EventHandler(selectAllMenuItem_Click);

            this.unselectAllMenuItem = new ApplicationBarMenuItem();
            this.unselectAllMenuItem.Text = AppResources.AllCancelMenuItem;
            this.unselectAllMenuItem.Click += new EventHandler(unselectAllMenuItem_Click);
            #endregion
        }

        private void ShowDefaultAppBar()
        {
            this.ApplicationBar.Mode = ApplicationBarMode.Default;
            this.ApplicationBar.Buttons.Clear();
            this.ApplicationBar.Buttons.Add(this.sureButton);
            this.ApplicationBar.Buttons.Add(this.settingButton);
            this.ApplicationBar.Buttons.Add(this.deleteButton);
            if (ApplicationBar.MenuItems.Count == 0)
                ShowMenuItem();
        }

        private void ShowMenuItem()
        {
            this.ApplicationBar.MenuItems.Add(this.feedBackMenuItem);
            this.ApplicationBar.MenuItems.Add(this.reviewMenuItem);
            this.ApplicationBar.MenuItems.Add(this.otherAppMenuItem);
            this.ApplicationBar.MenuItems.Add(this.goaiyingyongMenuItem);
        }
    }
}