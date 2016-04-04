using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.ObjectModel;

namespace SearchApplication.Models
{
    public class AreaDataModel
    {

        private String _step1;

        public String Step1
        {
            get { return _step1; }
            set { _step1 = value; }
        }

        private String _step2;

        public String Step2
        {
            get { return _step2; }
            set { _step2 = value; }
        }

        private String _step3;

        public String Step3
        {
            get { return _step3; }
            set { _step3 = value; }
        }

        private String _x;

        public String X
        {
            get { return _x; }
            set { _x = value; }
        }

        private String _y;

        public String Y
        {
            get { return _y; }
            set { _y = value; }
        }

        private String _lat;

        public String Lat
        {
            get { return _lat; }
            set { _lat = value; }
        }

        private String _lng;

        public String Lng
        {
            get { return _lng; }
            set { _lng = value; }
        }
 
    }

    public class ResponsData
    {
        private String _baseDate;

        public String BaseDate
        {
            get { return _baseDate; }
            set { _baseDate = value; }
        }

        private String _baseTime;

        public String BaseTime
        {
            get { return _baseTime; }
            set { _baseTime = value; }
        }

        private String _category;

        public String Category
        {
            get { return _category; }
            set { _category = value; }
        }

        private String _nx;

        public String NX
        {
            get { return _nx; }
            set { _nx = value; }
        }

        private String _ny;

        public String NY
        {
            get { return _ny; }
            set { _ny = value; }
        }

        private String _obsrValue;

        public String ObsrValue
        {
            get { return _obsrValue; }
            set { _obsrValue = value; }
        }


    }




    public static class XmlParser
    {
        public static async Task<List<AreaDataModel>> LoadFile()
        {
            XmlDocument dom = new XmlDocument();
            try
            {

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/fileAccess.xml"));
                Stream st = await file.OpenStreamForReadAsync();
                XDocument data = XDocument.Load(st);
                List<AreaDataModel> itemList = new List<AreaDataModel>();
                
                foreach (var item in data.Descendants("AreaItems"))
                {

                    AreaDataModel model = new AreaDataModel();
                    model.Step1 = item.Element("Step1").Value;
                    model.Step2 = item.Element("Step2").Value;
                    model.Step3 = item.Element("Step3").Value;
                    model.X = item.Element("X").Value;
                    model.Y = item.Element("Y").Value;
                    model.Lat = item.Element("Lat").Value;
                    model.Lng = item.Element("Lng").Value;

                    itemList.Add(model);


                }

                 
                return itemList;
            }
            catch
            {


            }
            return null;
        }



    }
}
