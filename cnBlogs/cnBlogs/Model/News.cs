using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnBlogs.Model
{
   public class News
    {
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private string summary;

        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }
        private string published;

        public string Published
        {
            get { return published; }
            set { published = string.Format("{0:G}", DateTime.Parse(value)); }
        }
        private string link;

        public string Link
        {
            get { return link; }
            set { link = value; }
        }
        private string diggs;

        public string Diggs
        {
            get { return diggs; }
            set { diggs = value; }
        }
        private string views;

        public string Views
        {
            get { return views; }
            set { views = value; }
        }
        private string comments;

        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }
        private string topic;

        public string Topic
        {
            get { return topic; }
            set { topic = value; }
        }
        private string topicIcon;

        public string TopicIcon
        {
            get { return topicIcon; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    topicIcon = "Images/NoUserPic.png";
                }
                else
                {
                    topicIcon = value;
                }
            }
        }
        private string sourceName;

        public string SourceName
        {
            get { return sourceName; }
            set { sourceName = value; }
        }
    }

   class NewsCollection : ObservableCollection<News>
   {
       public NewsCollection()
           : base()
       {

       }
   }
}
