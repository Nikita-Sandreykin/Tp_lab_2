using System;
using System.Collections.Generic;
using System.Text;

namespace PP_server2
{
    class Data
    {
        internal List<string> clients = new List<string>();
        internal List<string> poem = new List<string>();
        internal int iterations = 0;
        public List<string> Clients { get; set; }
        public int Iterations { get; set; }
        public List<string> Poem { get; set; }
    }
}
