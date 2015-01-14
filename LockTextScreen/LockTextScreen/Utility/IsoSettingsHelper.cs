using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace lockScreen.Utility
{
    public static class IsoSettingsHelper
    {
        public static object getValueByKey(string key)
        { 
            object result=null;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<object>(key, out result);
            return result;
        }

        public static void SavaSetting(string key,string value)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(key))
                IsolatedStorageSettings.ApplicationSettings.Add(key, value);
            else
                IsolatedStorageSettings.ApplicationSettings[key] = value;
        }

        public static bool DeleSetting(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            {
                IsolatedStorageSettings.ApplicationSettings.Remove(key);
                return true;
            }
            else
                return false;
        }

        public static bool checkKey(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                return true;
            else
                return false;
        }
    }
}
