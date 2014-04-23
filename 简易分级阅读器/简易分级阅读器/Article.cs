using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 简易分级阅读器
{
    public class Article
    {
        string title;
        string question;
        string content;
        string newWord;
        string translation;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Question
        {
            get { return question; }
            set { question = value; }
        }

        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public string NewWord
        {
            get { return newWord; }
            set { newWord = value; }
        }

        public string Translation
        {
            get { return translation; }
            set { translation = value; }
        }
    }
}
