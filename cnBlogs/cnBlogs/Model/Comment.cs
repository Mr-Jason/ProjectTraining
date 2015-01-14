using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace cnBlogs.Model
{
    public class Comment
    {
        private string id;

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private string content;

        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                NotifyPropertyChanged("Content");
            }
        }
        private string published;

        public string Published
        {
            get { return published; }
            set
            {
                published = string.Format("{0:G}", DateTime.Parse(value));

                NotifyPropertyChanged("Published");
            }
        }

        private CommentAuthor author;
        public CommentAuthor Author
        {
            get { return author; }
            set 
            { 
                author = value;
                NotifyPropertyChanged("Author");
            }
        }

        #region 属性变化事件
        public PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion  
    }

    public class CommentAuthor
    {
        private string name;
        public string Name { get; set; }

        private string uri;
        public string Uri { get; set; }
    }

    public class CommentCollection : ObservableCollection<Comment>
    {
        public CommentCollection()
            : base()
        {

        }
    }
}
