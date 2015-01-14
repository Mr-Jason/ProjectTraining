using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnBlogs.Model
{
    public class Blogger
    {
        private string id;
        private string title;
        private string updated;
        private string link;
        private string blogapp;
        private string avatar;
        private string postcount;

        public string Postcount
        {
            get { return postcount; }
            set { postcount ="随笔："+ value; }
        }
        public string Avatar
        {
            get { return avatar; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    avatar = "Images/NoUserPic.png";
                }
                else
                {
                    avatar = value;
                }
            }
        }
        public string Blogapp
        {
            get { return blogapp; }
            set { blogapp = value; }
        }
        public string Link
        {
            get { return link; }
            set { link = value; }
        }
        public string Updated
        {
            get { return updated; }
            set { updated = "最后更新日期：" + string.Format("{0:G}", DateTime.Parse(value)); }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
    }

    public class BloggerCollection:ObservableCollection<Blogger>
    {
        public BloggerCollection()
            : base()
        { 
        
        }
    }
}
