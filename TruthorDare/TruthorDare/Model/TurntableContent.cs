using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthorDare.Model
{
    public class TurntableContent:NotificationObject
    {
        private string oneContent;
        private string  twoContent;
        private string threeContent;
        private string fourContent;
        private string fiveContent;
        private string sixContent;
        private string sevenContent;
        private string eightContent;

        public string OneContent
        {
            get { return oneContent; }
            set
            {
                oneContent = value;
                base.RaisePropertyChanged("OneContent");
            }
        }

        public string  TwoContent
        {
            get { return twoContent; }
            set
            {
                twoContent = value;
                base.RaisePropertyChanged("TwoContent");
            }
        }

        public string ThreeContent
        {
            get { return threeContent; }
            set
            {
                threeContent = value;
                base.RaisePropertyChanged("ThreeContent");
            }
        }

        public string FourContent
        {
            get { return fourContent; }
            set
            {
                fourContent = value;
                base.RaisePropertyChanged("FourContent");
            }
        }

        public string FiveContent
        {
            get { return fiveContent; }
            set
            {
                fiveContent = value;
                this.RaisePropertyChanged("FiveContent");
            }
        }

        public string SixContent
        {
            get { return sixContent; }
            set
            {
                sixContent = value;
                base.RaisePropertyChanged("SixContent");
            }
        }
        public string SevenContent
        {
            get { return sevenContent; }
            set
            {
                sevenContent = value;
                base.RaisePropertyChanged("SevenContent");
            }
        }
        public string EightContent
        {
            get { return eightContent; }
            set
            {
                eightContent = value;
                base.RaisePropertyChanged("EightContent");
            }
        }
    }
}
