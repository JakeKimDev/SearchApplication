using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private float  _x;

        public float  X
        {
            get { return _x; }
            set { _x = value; }
        }

        private float _y;

        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        private float _lat;

        public float Lat
        {
            get { return _lat; }
            set { _lat = value; }
        }

        private float _lng;

        public float Lng
        {
            get { return _lng; }
            set { _lng = value; }
        }

        private int _seq;

        public int Seq
        {
            get { return _seq; }
            set { _seq = value; }
        }

    }
}
