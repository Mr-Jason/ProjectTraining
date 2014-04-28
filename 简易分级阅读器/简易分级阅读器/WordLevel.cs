using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace 简易分级阅读器
{
    [Table]
    public class WordLevel:INotifyPropertyChanged,INotifyPropertyChanging
    {
        string _word;
        int _level;
        
        /// <summary>
        /// 单词
        /// </summary>
        [Column(CanBeNull = false, IsPrimaryKey = true)]
        public string Word
        {
            get { return _word; }
            set
            {
                if (_word != value)
                {
                    OnPropertyChanging(new PropertyChangingEventArgs("Word"));
                    this._word = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Word"));
                }

            }
        }
        
        /// <summary>
        /// 单词等级
        /// </summary>
        [Column]
        public int Level
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    OnPropertyChanging(new PropertyChangingEventArgs("Level"));
                    this._level = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Level"));
                }
            }
        }

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
