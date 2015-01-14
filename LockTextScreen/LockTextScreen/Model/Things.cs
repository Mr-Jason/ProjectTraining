using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockTextScreen.Model
{
    [Table]
    public class Things : INotifyPropertyChanged, INotifyPropertyChanging 
    {
        private int id;

        [Column(CanBeNull = false, IsPrimaryKey = true,IsDbGenerated=true)]  
        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    OnPropertyChanging(new PropertyChangingEventArgs("Id"));
                    this.id = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Id"));
                }
            }
        }

        private string  content;

         [Column]
        public string  Content
        {
            get { return content; }
            set
            {
                if (content != value)
                {
                    OnPropertyChanging(new PropertyChangingEventArgs("Content"));
                    this.content = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Content"));
                }
            }
        }

        private string intervalDate;

         [Column]
        public string IntervalDate
        {
            get { return intervalDate; }
            set
            {
                if (intervalDate != value)
                {
                    OnPropertyChanging(new PropertyChangingEventArgs("IntervalDate"));
                    this.intervalDate = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IntervalDate"));
                }
            }
        }

         private string setDate;

        [Column]
         public string SetDate
         {
             get { return setDate; }
             set
             {
                 if (intervalDate != value)
                 {
                     OnPropertyChanging(new PropertyChangingEventArgs("SetDate"));
                     this.setDate = value;
                     OnPropertyChanged(new PropertyChangedEventArgs("SetDate"));
                 }
             }
         }

        
        //private bool isSelected;

        //public bool IsSelected
        //{
        //    get;
        //    set;
        //}
        
         public event PropertyChangingEventHandler PropertyChanging;

         public event PropertyChangedEventHandler PropertyChanged;

         protected void OnPropertyChanging(PropertyChangingEventArgs e)
         {
             if (PropertyChanging != null)
             {
                 PropertyChanging(this, e);
             }
         }

         protected void OnPropertyChanged(PropertyChangedEventArgs e)
         {
             if (PropertyChanged != null)
             {
                 PropertyChanged(this, e);
             }
         }
    }
}
