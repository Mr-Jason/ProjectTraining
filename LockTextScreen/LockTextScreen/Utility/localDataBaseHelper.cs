using LockTextScreen.Model;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockTextScreen.Utility
{
    public class localDataBaseHelper : DataContext
    {
        public const string ConnectionString = "Data Source='isostore:/lockscreen.sdf';Password='administrator'";

        public localDataBaseHelper() : base(ConnectionString) { }

        public Table<Things> Things;

        public static void Add(string content,string intervalDate,string showTip)
        {
            using (localDataBaseHelper db = new localDataBaseHelper())
            {
                if (string.IsNullOrEmpty(intervalDate))
                {
                    intervalDate = showTip;
                }
                else
                {
                    if (showTip.IndexOf("set") > 0)
                    {
                        intervalDate = "Date：" + intervalDate;
                    }
                    else
                        intervalDate = "日期：" + intervalDate;
                }

                Things thing = new Things
                {
                    Content = content,
                    IntervalDate = intervalDate,
                    SetDate = DateTime.Now.Month + "/" + DateTime.Now.Day + Environment.NewLine+System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek)
                };
                db.Things.InsertOnSubmit(thing);
                db.SubmitChanges();
            }
        }

        public static void Delete(int id)
        {
            using (localDataBaseHelper db = new localDataBaseHelper())
            {
                Things thing = db.Things.FirstOrDefault(t => t.Id == id);
                if (thing != null)
                {
                    db.Things.DeleteOnSubmit(thing);
                    db.SubmitChanges();
                }
            }
        }

        public static IEnumerable<Things> Query()
        {
            List<Things> lThing = new List<Things>();
            using (localDataBaseHelper db = new localDataBaseHelper())
            {
                var things = from thing in db.Things
                             select thing;
                lThing = things.ToList<Things>();
            }
            return lThing;
        }
    }
}
