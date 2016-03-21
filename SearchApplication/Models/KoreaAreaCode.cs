using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchApplication.Models
{
    public class KoreaAreaCode
    {
        private String _code;

        public String Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private String _rNum;

        public String RNum
        {
            get { return _rNum; }
            set { _rNum = value; }
        }



    }
}
