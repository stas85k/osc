using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace machineUI
{
    public class mashine2
    {
        public mashine2()
        {
            n = 3;
            a = new double[n];
        }

        public int minit(int u)
        {
            n = n + u;
            a = new double[n];

            return n;
        }
        public int n;
        public double[] a;

    }
}
