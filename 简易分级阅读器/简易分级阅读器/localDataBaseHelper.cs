using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Resources;

namespace 简易分级阅读器
{
    class localDataBaseHelper :DataContext
    {
        public const string ConnectionString = "Data Source='isostore:/EasyReaders.sdf';Password='qwe123!@#'";

        public localDataBaseHelper() : base(ConnectionString) { }

        public Table<WordLevel> WordLevel;

        public void InitializeData()
        {
            int i = 0;

            using (localDataBaseHelper db = new localDataBaseHelper())
            {
                if (db.WordLevel.Select(c => c.Word).Count() > 0)
                    return;
                StreamResourceInfo reader = Application.GetResourceStream(new Uri("简易分级阅读器;component/Resources/nce4_words", UriKind.Relative));
                using (StreamReader readWord = new StreamReader(reader.Stream))
                {

                    string sLine = "";

                    while (sLine != null)
                    {
                        sLine = readWord.ReadLine();
                        if (sLine != null)
                        {
                            string[] str = sLine.Split('\t');
                            i++;
                            if (str[1].Trim() != "level")
                            {
                                WordLevel wordlevel = new WordLevel
                                {
                                    Word = str[0].Trim(),
                                    Level = int.Parse(str[1].Trim())
                                };
                                db.WordLevel.InsertOnSubmit(wordlevel);
                                db.SubmitChanges();
                            }
                        }
                    }
                }
            }
        }
    }
}
