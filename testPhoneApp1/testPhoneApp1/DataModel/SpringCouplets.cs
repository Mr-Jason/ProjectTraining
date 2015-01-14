using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testPhoneApp1.DataModel
{
    public class SpringCouplets
    {
        public SpringCouplets(string guangpi,string firstline,string secondline)
        {
            this.Guangpi = guangpi;
            this.FirstLine = firstline;
            this.SecondLine = secondline;
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
        
    }
}
