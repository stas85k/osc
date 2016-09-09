using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// probeareamm2 = 0.09424778 mm ^ 2 for Godyak's probe
namespace machineClassLibrary
{
    static public class plasmacalculator
    {
        static public double probearea;
        static public double plasma_temperature_ev;
        static public double plasma_density;
        static public double plasma_potential_v;
        static public double floating_potential_v;
        static public double ion_saturation_current_a;
        static public double[] electronenergy;
        static public double[] electronprobability;



        static public int get_plasmatemp_plasmapotential(double [] volt, double [] current, double [] didv, double [] d2idv2/*, double probeareamm2*/)
        {

            //const double echarge = -1.602e-19; //C
            //const double emass = 9.109e-31; //kg
            double t;
            int i,j,maxind=0;
            int retval = 0;
            double epsilon = 1e-3;
            //double[] currentdensityamperepermm2 = new double[current.Length];
            //double[] vdistr = new double[current.Length];
            double maxval=0.0;
            
            int ti;
            int plasma_potential_index;

            //find floating potential (volt)
            maxval = 0.0;
            ti = 0;
            for (i = 0; i < current.Length; i++)
            {
                t = 1.0/(epsilon+Math.Abs(current[i]));
                if (t > maxval)
                {
                    maxval = t;
                    ti = i;
                }
            }
            floating_potential_v = volt[ti];
            
            //ion_saturation_current (ampere)
            maxval = 0.0;
            for (i = 0; i < ti; i++)
            {
                if (maxval > current[i])
                {
                    maxval = current[i];
                }
            }
            ion_saturation_current_a = maxval;

            //plasma potential & temperature
            maxval = 0.0;
            for (i = ti; i < current.Length; i++)
            { //must be sorted in ascending order
                t = (Math.Abs(didv[i]) + epsilon) / (Math.Abs(d2idv2[i]) + epsilon);
                if (maxval < t)
                {
                    maxind = i;
                    maxval = t;
                }
            }
            plasma_potential_v = volt[maxind];
            plasma_potential_index = maxind;
            plasma_temperature_ev = Math.Abs(current[maxind]) / Math.Abs(didv[maxind]);

            //electron energy (eV)
            probearea = (Math.PI * mach.probediameter / 1000000.0) * (mach.probediameter / 4.0 + mach.probelength);
            double coefficient = (1.6404e+51) / probearea; //sqrt(8*emass/pow(ecahrge,7))/probearea
            electronenergy = new double[plasma_potential_index];
            electronprobability = new double[plasma_potential_index];
            j = 0;
            for(i= plasma_potential_index-1; i>0;i--)
            {
                electronenergy[j] = (plasma_potential_v - volt[i]);
                t = Math.Sqrt(electronenergy[j]);
                electronprobability[j++] = coefficient*d2idv2[i]/t;
            }

            //plasma density
            double density_coef = (4.21e+13) /probearea; // 2sqrt(2m)/e/sqrt(e)/Sp
            plasma_density = 0.0;
            j = 0;
            for (i = plasma_potential_index - 1; i > 0; i--)
            {
                t = Math.Sqrt(electronenergy[j++]);
                plasma_density += t * d2idv2[i];
            }
            plasma_density *= density_coef;

            //plasma temperature
            plasma_temperature_ev = 0.0;
            j = 0;
            double temperature_coef = (2.8067e+13)/ plasma_density/probearea; //4*sqrt(2*m)/3/(e^(3/2))/Sp/N
            for (i = plasma_potential_index - 1; i > 0; i--)
            {
                t = Math.Sqrt(electronenergy[j]);
                t *= electronenergy[j++];
                plasma_temperature_ev += t * d2idv2[i];
            }
            plasma_temperature_ev *= temperature_coef;

            return retval;
        }

    }
}
