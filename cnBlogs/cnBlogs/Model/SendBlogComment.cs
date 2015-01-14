using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnBlogs.Model
{
   public class SendBlogComment
    {
        private string data;

        public string Data
        {
            get { return data; }
            set { data = value; }
        }
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string isSuccess;

        public string IsSuccess
        {
            get { return isSuccess; }
            set { isSuccess = value; }
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        private string commentId;

        public string CommentId
        {
            get { return commentId; }
            set { commentId = value; }
        }
    }
}
