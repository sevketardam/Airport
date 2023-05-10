using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.MessageExtension.VM
{
    public class Mesaj
    {
        public string msg { get; set; }
        public string dest { get; set; }
        public Mesaj() { }
        public Mesaj(string msg, string dest)
        {
            this.msg = msg;
            this.dest = dest;
        }
    }

    public class SmsIstegi
    {
        public string username { get; set; }
        public string password { get; set; }
        public string source_addr { get; set; }
        public Mesaj[] messages { get; set; }
    }
}
