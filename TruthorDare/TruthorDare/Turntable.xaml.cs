using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TruthorDare.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace TruthorDare
{
    public sealed partial class Turntable : UserControl
    {
        /// <summary>
        /// 保存八个角度
        /// </summary>
        List<int> _ListAngle = new List<int>();
        /// <summary>
        /// 产生随机数
        /// </summary>
        Random _Random = new Random();
        int _Index = 0;
        int _OldAngle = 0;
        public Turntable()
        {
            this.InitializeComponent();
            this.Loaded += Turntable_Loaded;
           // this.DataContext = new TurntableViewModel();
        }

        void Turntable_Loaded(object sender, RoutedEventArgs e)
        {
            this.gdTurntable.Width = ActualWidth - 10;
            this.gdTurntable.Height = ActualWidth - 10;

            int angle = 5040;
            for (int i = 0; i < 8; i++)
            {
                angle += 45;
                _ListAngle.Add(angle);
            }
        }

        private void btnStartTurn_Click(object sender, RoutedEventArgs e)
        {
            this.btnStartTurn.IsEnabled = false;
            _Index = _Random.Next(0, 8);

            ((SplineDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)this.storyBoardturn.Children[0]).KeyFrames[0]).Value = _OldAngle;
            ((SplineDoubleKeyFrame)((DoubleAnimationUsingKeyFrames)this.storyBoardturn.Children[0]).KeyFrames[3]).Value = _ListAngle[_Index];
            storyBoardturn.Begin();
        }

        private void storyBoardturn_Completed(object sender, object e)
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(0.3);
            dt.Tick += delegate
            {
                dt.Stop();
                _OldAngle = (_ListAngle[_Index] % 360);
                this.btnStartTurn.IsEnabled = true;
                AwardProcess(GetAward(_ListAngle[_Index]));
            };
            dt.Start();
        }

        public delegate void AwardDelegate(Award award);

        /// <summary>
        /// 返回转到的奖项信息
        /// </summary>
        public event AwardDelegate AwardProcess;

        private Award GetAward(int angle)
        {

            Award result = Award.谢谢参与;
            switch (angle)
            {
                case 5085:
                    result = Award.大冒险;
                    break;
                case 5130:
                    //result = "谢谢参与";
                    break;
                case 5175:
                    result = Award.真心话;
                    break;
                case 5220:
                    //result = "谢谢参与";
                    break;
                case 5265:
                    result = Award.大冒险;
                    break;
                case 5310:
                    //result = "谢谢参与";
                    break;
                case 5355:
                    result = Award.真心话;
                    break;
                case 5400:
                    //result = "谢谢参与";
                    break;
                default:
                    break;
            }

            return result;
        }
    }

    public enum Award
    {
        大冒险,
        真心话,
        谢谢参与
    }
}
