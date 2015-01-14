using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Resources;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;

namespace testPhoneApp1.DataModel
{
    class SprigCoupletsDataSource
    {
        private static SprigCoupletsDataSource _sampleDataSource = new SprigCoupletsDataSource();

        private ObservableCollection<HappyElement> _groups = new ObservableCollection<HappyElement>();
        public ObservableCollection<HappyElement> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<HappyElement>> GetGroupsAsync()
        {
            await _sampleDataSource.GetElements();

            return _sampleDataSource.Groups;
        }

        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/SpringCouplets.xml");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);

            IInputStream x =await file.OpenSequentialReadAsync();
            
        }

        public async Task GetElements()
        {
              Uri dataUri = new Uri("ms-appx:///DataModel/HappyElements.xml");
              StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
              string str = await FileIO.ReadTextAsync(file);
              XDocument doc = XDocument.Parse(str);
              var elementlist = from query in doc.Descendants("Element")
                             select new HappyElement
                             {
                                 ImagePath = (string)query.Element("imagePath")
                             };
               List<HappyElement> lelements= elementlist.ToList<HappyElement>();
               for (int i = 0; i < lelements.Count; i++)
               {
                   this.Groups.Add(lelements[i]);
               }
            //StreamResourceInfo sr =//App.GetContentStream(new Uri("SpringCouplets.xml", UriKind.RelativeOrAbsolute));
        }
    }
}
