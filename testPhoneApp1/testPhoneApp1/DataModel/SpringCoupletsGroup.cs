using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testPhoneApp1.DataModel
{
   public class SpringCoupletsGroup
    {
       public SpringCoupletsGroup(string guangpi, string firstline, string secondline)
        {
            this.Guangpi = guangpi;
            this.FirstLine = firstline;
            this.SecondLine = secondline;
            this.Items = new ObservableCollection<SpringCouplets>();
        }

        private string guangpi;

        public string Guangpi
        {
            get { return guangpi; }
            set { guangpi = value; }
        }

        private string firstLine;

        public string FirstLine
        {
            get { return firstLine; }
            set { firstLine = value; }
        }

        private string secondLine;

        public string SecondLine
        {
            get { return secondLine; }
            set { secondLine = value; }
        }

        public ObservableCollection<SpringCouplets> Items { get; set; }
    }
}
