using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookTaxiV2
{
    class login
    {
        private string user_name;
        private string user_pass;

        public void insert(string uname , string pass)
        {
            user_name = uname;
            user_pass = pass;
        }
    }
}
