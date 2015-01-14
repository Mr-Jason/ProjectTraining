using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnBlogs.Model
{
   public class Blogs
    {
        private string id;
        private string title;
        private string summary;
        private string published;
        private Author author;  //博文作者
        private string link;    //博文地址
        private string blogapp;
        private string diggs;  //多少人赞
        private string views;//多少人阅读
        private string comments; //多少评论

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
       
        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }
        
        public string Published
        {
            get { return published; }
            set
            {
                Debug.WriteLine(value);
                published = string.Format("{0:G}", DateTime.Parse(value));
            }
        }
       
        public Author Author
        {
            get { return author; }
            set { author = value; }
        }
      
        public string Link
        {
            get { return link; }
            set { link = value; }
        }

        public string Blogapp
        {
            get { return blogapp; }
            set { blogapp = value; }
        }

        public string Diggs
        {
            get { return diggs; }
            set { diggs = value; }
        }
     
        public string Views
        {
            get { return views; }
            set { views = value; }
        }

        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }
    }
   public class Author
   {
       //名字
       private string authorName;
      
       //博客地址
       private string addressBlog;

       //头像
       private string avatar;

       public string AddressBlog
       {
           get { return addressBlog; }
           set { addressBlog = value; }
       }

       public string AuthorName
       {
           get { return authorName; }
           set { authorName = value; }
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
   }

   public class ArticleCollection : ObservableCollection<Blogs>
   {
       public ArticleCollection()
           : base()
       {

       }
   }
}
