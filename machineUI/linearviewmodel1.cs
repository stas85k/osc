using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace machineUI
{
    public class linearviewmodel1
    {
        public linearviewmodel1()
        {
            this.p1 = new List<DataPoint>();
            this.p2 = new List<DataPoint>();
            this.p3 = new List<DataPoint>();
            for (int x = 0; x <= 1000; x++)
            {
                /*if (App.mach != null && App.mach.channelAvoltage.Length > x)
                {
                    this.p1.Add(new DataPoint((double)x, 10+ Math.Abs(App.mach.channelAvoltage[x])));
                    this.p2.Add(new DataPoint((double)x, App.mach.channelAvoltage[x]));
                    this.p3.Add(new DataPoint((double)x, 10 - App.mach.channelAvoltage[x]));
                }
                else */
                {
                    this.p1.Add(new DataPoint(x, 10));
                    this.p2.Add(new DataPoint(x, 0));
                    this.p3.Add(new DataPoint(x, 5));
                }
            }
        }

        public IList<DataPoint> p1 { get; private set; }
        public IList<DataPoint> p2 { get; private set; }
        public IList<DataPoint> p3 { get; private set; }
    }
}
