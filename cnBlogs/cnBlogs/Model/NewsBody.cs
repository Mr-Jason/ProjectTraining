using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnBlogs.Model
{
    public class NewsBody
    {
        private string title;
        private string sourcenName;
        private string submitDate;
        private string content;
        private string imageUrl;
        private string prevNews;
        private string nextNews;
        private string commentCount;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string SourcenName
        {
            get { return sourcenName; }
            set { sourcenName = value; }
        }
        public string SubmitDate
        {
            get { return submitDate; }
            set { submitDate = value; }
        }
        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }
        public string PrevNews
        {
            get { return prevNews; }
            set { prevNews = value; }
        }
        public string NextNews
        {
            get { return nextNews; }
            set { nextNews = value; }
        }
        public string CommentCount
        {
            get { return commentCount; }
            set { commentCount = value; }
        }
    }
}
