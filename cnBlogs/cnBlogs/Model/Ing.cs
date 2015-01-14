using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnBlogs.Model
{
    public class Ing
    {
        private string author;
        private string author_id;
        private string avatar;
        private string body;
        private com_feeds[] com_feeds;

        private string com_time;
        private string has_more_com;
        private string id;
        private string lucky;
        private string newbie;

        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        public string Author_id
        {
            get { return author_id; }
            set { author_id = value; }
        }

        public string Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }

        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        public com_feeds[] Com_feeds
        {
            get { return com_feeds; }
            set { com_feeds = value; }
        }

        public string Com_time
        {
            get { return com_time; }
            set { com_time = value; }
        }

        public string Has_more_com
        {
            get { return has_more_com; }
            set { has_more_com = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Lucky
        {
            get { return lucky; }
            set { lucky = value; }
        }

        public string Newbie
        {
            get { return newbie; }
            set { newbie = value; }
        }
    }

    public class com_feeds
    {
        private string author;
        private string author_avatar;
        private string author_id;
        private string com_time;
        private string conent;
        private string id;
        private string parent_id;

        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        public string Author_avatar
        {
            get { return author_avatar; }
            set { author_avatar = value; }
        }

        public string Author_id
        {
            get { return author_id; }
            set { author_id = value; }
        }

        public string Com_time
        {
            get { return com_time; }
            set { com_time = value; }
        }

        public string Conent
        {
            get { return conent; }
            set { conent = value; }
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Parent_id
        {
            get { return parent_id; }
            set { parent_id = value; }
        }
    }

    public class IngCollection : ObservableCollection<Ing>
    {
        public IngCollection()
            : base()
        {

        }
    }
}
