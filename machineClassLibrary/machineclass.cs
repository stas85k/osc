using PS5000A;
using PS5000AStreamingConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using machineClassLibrary;

namespace machineClassLibrary
{
    static public class mach //ine
    {
        static public bool deviceisopen { get; set; }
        static public string devicelogs { get; set; }


        static public double[] vlp { get; set; }
        static public double[] vrp { get; set; }
        static public double [] vp { get; set; }
        static public double[] ilp { get; set; }
        static public double[] dilp { get; set; }
        static public double[] d2ilp { get; set; }

        static public double vlpmax { get; set; }
        static public double vrpmax { get; set; }
        static public double vpmax { get; set; }

        static public double ilpmax { get; set; }
        static public double dilpmax { get; set; }
        static public double d2ilpmax { get; set; }

        static public double vlpmin { get; set; }
        static public double vrpmin { get; set; }
        static public double vpmin { get; set; }

        static public double ilpmin { get; set; }
        static public double dilpmin { get; set; }
        static public double d2ilpmin { get; set; }

        static public double[] channelAvoltage { get; set; }
        static public double[] channelBvoltage { get; set; }
        static public double[] channelCvoltage { get; set; }
        static public double[] channelDvoltage { get; set; }
        static public short[] BufferA { get; set; }
        static public short[] BufferB { get; set; }
        static public short[] BufferC { get; set; }
        static public short[] BufferD { get; set; }


        static public uint medianwindow { set; get; } = 100;
        static public uint guardinterval { set; get; } = 100;
        static public uint diffinterval { set; get; } = 10;
        static public uint sweeprate { set; get; } = 100;
        static public uint numberofsweeps { set; get; } = 1;
        static public uint numberofsamplesinsweep { set; get; } = 1000;
        static public uint samplingrate { set; get; } = 100000;
        static public uint bufferlength { set; get; }
        static public uint timebase { set; get; } = 3;
        static public short triggerthreshold { set; get; } = 0;//[-32767:+32767]

        static public double LPVoltmeterAttenuation { set; get; } = 19.9;
        static public double RPVoltmeterAttenuation { set; get; } = 19.9;
        static public double CurrentMeterShuntResistance { set; get; } = 3.33;

        static public double probelength { set; get; } = 1.0;//mm
        static public double probediameter { set; get; } = 0.3;//mm
        static public double probecleaningpotential { set; get; } = -0.5;//Vpp on generator input
        static public double probecleaningtime { set; get; } = 0.1; //seconds

        static public long _offsetVoltage { set; get; } = 0; //uV
        static public ulong _pkToPk { set; get; } = 0; //uV

        static private short _handle;
        public const int BUFFER_SIZE = 1024;
        public const int MAX_CHANNELS = 4;
        public const int QUAD_SCOPE = 4;
        public const int DUAL_SCOPE = 2;





        //short _oversample = 1;
        //bool _scaleVoltages = true;
        //ushort[] inputRanges = { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000 };
        static bool _ready = false;
        //short _trig = 0;
        //uint _trigAt = 0;

        static uint _startIndex = 0;
        static bool _autoStop;
        //private ChannelSettings[] _channelSettings;
        static private int _channelCount;
        static private Imports.Range _firstRange;
        static private Imports.Range _lastRange;
        static private int _digitalPorts;
        static private Imports.ps5000aBlockReady _callbackDelegate;
        static private string StreamFile = "stream.txt";
        static private string BlockFile = "block.txt";

        static public Array ranges;
        static public Array waveforms;
        static public int[] rangeinds;
        static public bool[] channelson;





        static public void machineinit()
        {
            deviceisopen = false;
            devicelogs="";

            ranges = System.Enum.GetValues(typeof(Imports.Range));
            waveforms = System.Enum.GetValues(typeof(Imports.enPS5000AWaveType));
            rangeinds = new int[5];
            channelson = new bool[5];
           
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        static private void BlockCallback(short handle, short status, IntPtr pVoid)
        {
            // flag to say done reading data
            if (status != (short)Imports.PICO_CANCELLED)
                _ready = true;
        }

        static private short SetTrigger(Imports.TriggerChannelProperties[] channelProperties,
            short nChannelProperties,
            Imports.TriggerConditions[] triggerConditions,
            short nTriggerConditions,
            Imports.ThresholdDirection[] directions,
            uint delay,
            short auxOutputEnabled,
            int autoTriggerMs)
        {
            short status;

            if (
              (status = Imports.SetTriggerChannelProperties(_handle, channelProperties, nChannelProperties, auxOutputEnabled,
                                                   autoTriggerMs)) != 0)
            {
                return status;
            }

            if ((status = Imports.SetTriggerChannelConditions(_handle, triggerConditions, nTriggerConditions)) != 0)
            {
                return status;
            }

            if (directions == null)
                directions = new Imports.ThresholdDirection[] { Imports.ThresholdDirection.None,
                                Imports.ThresholdDirection.None, Imports.ThresholdDirection.None, Imports.ThresholdDirection.None,
                                Imports.ThresholdDirection.None, Imports.ThresholdDirection.None};

            if ((status = Imports.SetTriggerChannelDirections(_handle,
                                                              directions[(int)Imports.Channel.ChannelA],
                                                              directions[(int)Imports.Channel.ChannelB],
                                                              directions[(int)Imports.Channel.ChannelC],
                                                              directions[(int)Imports.Channel.ChannelD],
                                                              directions[(int)Imports.Channel.External],
                                                              directions[(int)Imports.Channel.Aux])) != 0)
            {
                return status;
            }

            if ((status = Imports.SetTriggerDelay(_handle, delay)) != 0)
            {
                return status;
            }



            return status;
        }


        static public void opendevice()
        {
            StringBuilder UnitInfo = new StringBuilder(80);

            short handle;

            string[] description = {
                           "Driver Version    ",
                           "USB Version       ",
                           "Hardware Version  ",
                           "Variant Info      ",
                           "Serial            ",
                           "Cal Date          ",
                           "Kernel Ver        ",
                           "Digital Hardware  ",
                           "Analogue Hardware "
                         };

            Imports.DeviceResolution resolution = Imports.DeviceResolution.PS5000A_DR_14BIT;
            //Imports.DeviceResolution resolution = Imports.DeviceResolution.PS5000A_DR_8BIT;


            if (_handle > 0)
            {
                Imports.CloseUnit(_handle);

                devicelogs = "";
                _handle = 0;
                devicelogs += "Closed\n";
                deviceisopen = false;
            }
            else
            {
                if (channelson[0] || channelson[1] || channelson[2] || channelson[3] || channelson[4])
                {
                    short status = Imports.OpenUnit(out handle, null, resolution);
                    if (handle > 0)
                    {
                        _handle = handle;

                        devicelogs = "Handle " + _handle.ToString() + "\r\n";

                        for (int i = 0; i < 9; i++)
                        {
                            short requiredSize;
                            Imports.GetUnitInfo(_handle, UnitInfo, 80, out requiredSize, i);
                            devicelogs += description[i] + UnitInfo + "\r\n";
                        }
                        deviceisopen = true;
                        devicelogs += "Open\n";
                    }
                }
                else
                {
                    devicelogs += "Activate channels or generator\n";
                }
            }
        }

        static public void startacquisition()
        {
            short status;
            uint i,j;

            if (channelson[4])
            {
                startgenerator();
            }

            status = Imports.SetChannel(_handle, Imports.Channel.ChannelA, channelson[0] ? (short)1 : (short)0, 1, (Imports.Range)ranges.GetValue(rangeinds[0]), 0);
            status = Imports.SetChannel(_handle, Imports.Channel.ChannelB, channelson[1] ? (short)1 : (short)0, 1, (Imports.Range)ranges.GetValue(rangeinds[1]), 0);
            status = Imports.SetChannel(_handle, Imports.Channel.ChannelC, channelson[2] ? (short)1 : (short)0, 1, (Imports.Range)ranges.GetValue(rangeinds[2]), 0);
            status = Imports.SetChannel(_handle, Imports.Channel.ChannelD, channelson[3] ? (short)1 : (short)0, 1, (Imports.Range)ranges.GetValue(rangeinds[3]), 0);

            short enable = 1;//0;
            uint delay = 0;
            //triggerthreshold = 0;//25000;
            short auto = 0;

            Imports.ThresholdDirection direction;
            switch (rangeinds[4])
            {
                case 3: //ramp up
                    direction = Imports.ThresholdDirection.Falling;
                    break;
                case 4: // ramp down
                    direction = Imports.ThresholdDirection.Rising;
                    break;
                default:
                    direction = Imports.ThresholdDirection.Rising;
                    break;
            }
            status = Imports.SetSimpleTrigger(_handle, enable, Imports.Channel.ChannelA, triggerthreshold, direction, delay, auto);

            _ready = false;
            _callbackDelegate = BlockCallback;
            _channelCount = 4;

            devicelogs = "";

            bool retry;
           ///PinnedArray<short>[] minPinned = new PinnedArray<short>[_channelCount];
            ///PinnedArray<short>[] maxPinned = new PinnedArray<short>[_channelCount];

            int timeIndisposed;
            ////short[] minBuffers = new short[sampleCount];
            BufferA = new short[bufferlength];
            BufferB = new short[bufferlength];
            BufferC = new short[bufferlength];
            BufferD = new short[bufferlength];
            ////minPinned[0] = new PinnedArray<short>(minBuffers);
            ////maxPinned[0] = new PinnedArray<short>(BufferA);
            //status = Imports.SetDataBuffers(_handle, Imports.Channel.ChannelA, BufferA, minBuffers, (int)sampleCount, 0, Imports.RatioMode.None);

            status = Imports.SetDataBuffer(_handle, Imports.Channel.ChannelA, BufferA, (int)bufferlength, 0, Imports.RatioMode.None);
            devicelogs += status == (short)Imports.PICO_OK ? "Ch A OK\n" : "Ch A Error\n";
            status = Imports.SetDataBuffer(_handle, Imports.Channel.ChannelB, BufferB, (int)bufferlength, 0, Imports.RatioMode.None);
            devicelogs += status == (short)Imports.PICO_OK ? "Ch B OK\n" : "Ch B Error\n";
            status = Imports.SetDataBuffer(_handle, Imports.Channel.ChannelC, BufferC, (int)bufferlength, 0, Imports.RatioMode.None);
            devicelogs += status == (short)Imports.PICO_OK ? "Ch C OK\n" : "Ch C Error\n";
            status = Imports.SetDataBuffer(_handle, Imports.Channel.ChannelD, BufferD, (int)bufferlength, 0, Imports.RatioMode.None);
            devicelogs += status == (short)Imports.PICO_OK ? "Ch D OK\n" : "Ch D Error\n";
            devicelogs += "BlockData\n";

            /*  find the maximum number of samples, the time interval (in timeUnits),
             *		 the most suitable time units, and the maximum _oversample at the current _timebase*/
            int timeInterval;
            int maxSamples;
            while (Imports.GetTimebase(_handle, timebase, (int)bufferlength, out timeInterval, out maxSamples, 0) != 0)
            {
                devicelogs += "Timebase selection\n";
                timebase++;

            }
            devicelogs += "Timebase Set " + timebase.ToString() +"\n";
            devicelogs += "Sample Count Set " + bufferlength.ToString() + "\n";
            devicelogs += "Time Interval Set " + timeInterval.ToString() + "\n";
            devicelogs += "Max Samples Set " + maxSamples.ToString() + "\n";

            /* Start it collecting, then wait for completion*/
            _ready = false;
            _callbackDelegate = BlockCallback;

            do
            {
                retry = false;
                status = Imports.RunBlock(_handle, 0, (int)bufferlength, timebase, out timeIndisposed, 0, _callbackDelegate, IntPtr.Zero);
                if (status == (short)Imports.PICO_POWER_SUPPLY_CONNECTED || status == (short)Imports.PICO_POWER_SUPPLY_NOT_CONNECTED || status == (short)Imports.PICO_POWER_SUPPLY_UNDERVOLTAGE)
                {
                    retry = true;
                }
                else
                {
                    devicelogs +="Run Block Called\n";
                }
            }
            while (retry);

            devicelogs += "Waiting for Data\n";

            { 
                uint sleepcntr = 0;
                while (sleepcntr<10 && !_ready)
                {
                    Thread.Sleep(100);
                    sleepcntr++;
                }
                devicelogs += String.Format("{0} ms\n", sleepcntr*100);
            }

            Imports.Stop(_handle);
            if (channelson[4])
            {
                stopgenerator();
            }

            if (_ready)
            {
                uint[] rangemv = { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 20000 };
                short overflow;
                uint t = bufferlength;
                status = Imports.GetValues(_handle, 0, ref t, 1, Imports.DownSamplingMode.None, 0, out overflow);
                if (status == (short)Imports.PICO_OK && (numberofsamplesinsweep+guardinterval*2)*numberofsweeps==bufferlength)
                {
                    devicelogs += "Have Data\n";

                    channelAvoltage = new double[numberofsamplesinsweep];
                    channelBvoltage = new double[numberofsamplesinsweep];
                    channelCvoltage = new double[numberofsamplesinsweep];
                    channelDvoltage = new double[numberofsamplesinsweep];
                    vlp = new double[numberofsamplesinsweep];
                    vrp = new double[numberofsamplesinsweep];
                    vp = new double[numberofsamplesinsweep];
                    ilp = new double[numberofsamplesinsweep];
                    dilp = new double[numberofsamplesinsweep];
                    d2ilp = new double[numberofsamplesinsweep];
                    vlpmin = 1e12;
                    vrpmin = 1e12;
                    vpmin = 1e12;
                    ilpmin = 1e12;
                    dilpmin = 1e12;
                    d2ilpmin = 1e12;
                    vlpmax = -1e12;
                    vrpmax = -1e12;
                    vpmax = -1e12;
                    ilpmax = -1e12;
                    dilpmax = -1e12;
                    d2ilpmax = -1e12;

                    devicelogs += "Reallocated " + channelAvoltage.Length.ToString() + " doubles\n";


                    //median filtering starting.
                    if (medianwindow > 0)
                    {
                        short[] tempArr;
                        uint medianguard = Math.Min(medianwindow / 2, numberofsamplesinsweep / 4);
                        if (channelson[0])
                        {
                            tempArr = BufferA.SubArray(0, bufferlength);
                            for (i = medianguard; i < bufferlength - medianguard; i++)
                                BufferA[i] = statisticalprocessing.Median(tempArr.SubArray(i - medianguard, 2 * medianguard));
                            for (i = medianguard; i > 0; i--)
                            {
                                BufferA[i - 1] = (short)(2 * (long)BufferA[i] - (long)BufferA[i + 1]);
                                j = bufferlength - i;
                                BufferA[j] = (short)(2 * (long)BufferA[j - 1] - (long)BufferA[j - 2]);
                            }
                        }

                        if (channelson[1])
                        {
                            tempArr = BufferB.SubArray(0, bufferlength);
                            for (i = medianguard; i < bufferlength - medianguard; i++)
                                BufferB[i] = statisticalprocessing.Median(tempArr.SubArray(i - medianguard, 2 * medianguard));
                            for (i = medianguard; i > 0; i--)
                            {

                                BufferB[i - 1] = (short)(2 * (long)BufferB[i] - (long)BufferB[i + 1]);
                                j = bufferlength - i;
                                BufferB[j] = (short)(2 * (long)BufferB[j - 1] - (long)BufferB[j - 2]);
                            }
                        }

                        if (channelson[2])
                        {
                            tempArr = BufferC.SubArray(0, bufferlength);
                            for (i = medianguard; i < bufferlength - medianguard; i++)
                                BufferC[i] = statisticalprocessing.Median(tempArr.SubArray(i - medianguard, 2 * medianguard));
                            for (i = medianguard; i > 0; i--)
                            {
                                BufferC[i - 1] = (short)(2 * (long)BufferC[i] - (long)BufferC[i + 1]);
                                j = bufferlength - i;
                                BufferC[j] = (short)(2 * (long)BufferC[j - 1] - (long)BufferC[j - 2]);
                            }
                        }

                        if (channelson[3])
                        {
                            tempArr = BufferD.SubArray(0, bufferlength);
                            for (i = medianguard; i < bufferlength - medianguard; i++)
                                BufferD[i] = statisticalprocessing.Median(tempArr.SubArray(i - medianguard, 2 * medianguard));
                            for (i = medianguard; i > 0; i--)
                            {
                                BufferD[i - 1] = (short)(2 * (long)BufferD[i] - (long)BufferD[i + 1]);
                                j = bufferlength - i - 1;
                                BufferD[j] = (short)(2 * (long)BufferD[j - 1] - (long)BufferD[j - 2]);
                            }
                        }
                    }
                    //median filtering done.

                    uint x;
                    for (x = 0; x < numberofsamplesinsweep; x++)
                    {
                        uint x1;
                        if (channelson[0])
                        {
                            channelAvoltage[x] = (double)BufferA[x + guardinterval];
                            for(x1 = numberofsamplesinsweep + 3*guardinterval + x; x1 < bufferlength; x1 += (numberofsamplesinsweep + 2*guardinterval))
                                channelAvoltage[x] += (double)BufferA[x1];
                            channelAvoltage[x] *= (double)rangemv[rangeinds[0]] / ((double)numberofsweeps * (double)Imports.MaxLogicLevel);
                            vlp[x] = channelAvoltage[x] * LPVoltmeterAttenuation / 1000;
                            vlpmax = vlp[x] > vlpmax ? vlp[x] : vlpmax;
                            vlpmin = vlp[x] < vlpmin ? vlp[x] : vlpmin;
                        }
                        else
                        {
                            vlp[x] = vlpmin = vlpmax = channelAvoltage[x] = 0.0;
                        }

                        if (channelson[1])
                        {
                            channelBvoltage[x] = (double)BufferB[x];
                            for (x1 = numberofsamplesinsweep + 3 * guardinterval + x; x1 < bufferlength; x1 += (numberofsamplesinsweep + 2 * guardinterval))
                                channelBvoltage[x] += (double)BufferB[x1];
                            channelBvoltage[x] *= (double)rangemv[rangeinds[1]] / ((double)numberofsweeps * (double)Imports.MaxLogicLevel);
                            ilp[x] = channelBvoltage[x] / CurrentMeterShuntResistance / 1000;
                            ilpmax = ilp[x] > ilpmax ? ilp[x] : ilpmax;
                            ilpmin = ilp[x] < ilpmin ? ilp[x] : ilpmin;
                        }
                        else
                        {
                            ilp[x] = ilpmin = ilpmax = channelBvoltage[x] = 0.0;
                        }

                        if (channelson[2])
                        {
                            channelCvoltage[x] = (double)BufferC[x];
                            for (x1 = numberofsamplesinsweep + 3 * guardinterval + x; x1 < bufferlength; x1 += (numberofsamplesinsweep + 2 * guardinterval))
                                channelCvoltage[x] += (double)BufferC[x1];
                            channelCvoltage[x] *= (double)rangemv[rangeinds[2]] / ((double)numberofsweeps * (double)Imports.MaxLogicLevel);
                            vrp[x] = channelCvoltage[x] * RPVoltmeterAttenuation / 1000;
                            vrpmax = vrp[x] > vrpmax ? vrp[x] : vrpmax;
                            vrpmin = vrp[x] < vrpmin ? vrp[x] : vrpmin;
                        }
                        else
                        {
                            vrp[x] = vrpmin = vrpmax =channelCvoltage[x] = 0.0;
                        }


                        if (channelson[3])
                        {
                            channelDvoltage[x] = (double)BufferD[x];
                            for (x1 = numberofsamplesinsweep + 3 * guardinterval + x; x1 < bufferlength; x1 += (numberofsamplesinsweep + 2 * guardinterval))
                                channelDvoltage[x] += (double)BufferD[x1];
                            channelDvoltage[x] *= (double)rangemv[rangeinds[3]] / ((double)numberofsweeps * (double)Imports.MaxLogicLevel);
                        }
                        else
                        {
                            channelDvoltage[x] = 0.0;
                        }


                        vp[x] = vlp[x] - vrp[x];
                        vpmax = vp[x] > vpmax ? vp[x] : vpmax;
                        vpmin = vp[x] < vpmin ? vp[x] : vpmin;
                    }



                    for (x = diffinterval; x < numberofsamplesinsweep - diffinterval; x++)
                    {
                        dilp[x] = (ilp[x + diffinterval] - ilp[x - diffinterval]) / (vp[x + diffinterval] - vp[x - diffinterval]);
                        dilpmax = dilp[x] > dilpmax ? dilp[x] : dilpmax;
                        dilpmin = dilp[x] < dilpmin ? dilp[x] : dilpmin;
                    }
                    for (x = 0; x < diffinterval; x++)
                    {
                        dilp[x] = dilp[diffinterval];
                        dilp[numberofsamplesinsweep - x - 1] = dilp[numberofsamplesinsweep - diffinterval -1];
                    }

                    for (x = diffinterval; x < numberofsamplesinsweep - diffinterval; x++)
                    {
                        d2ilp[x] = (dilp[x + diffinterval] - dilp[x - diffinterval]) / (vp[x + diffinterval] - vp[x - diffinterval]);
                        d2ilpmax = d2ilp[x] > d2ilpmax ? d2ilp[x] : d2ilpmax;
                        d2ilpmin = d2ilp[x] < d2ilpmin ? d2ilp[x] : d2ilpmin;
                    }
                    for (x = 0; x < diffinterval; x++)
                    {
                        d2ilp[x] = d2ilp[diffinterval];
                        d2ilp[numberofsamplesinsweep - x -1] = d2ilp[numberofsamplesinsweep - diffinterval - 1];
                    }
                }
                else
                {
                    devicelogs += "No Data\n";
                }
            }
            else
            {
                devicelogs += "data collection aborted\n";
            }

            Imports.Stop(_handle);

            ///foreach (PinnedArray<short> p in minPinned)
            ///{
            ///    if (p != null)
            ///        p.Dispose();
            ///}
            ///foreach (PinnedArray<short> p in maxPinned)
            ///{
            ///    if (p != null)
            ///        p.Dispose();
            ///}
        }

        //pd. 3)
        static public void startgenerator()
        {
            if (_handle >= 0)
            {
                ulong retval = Imports.PICO_OK;
                long offsetVoltage = _offsetVoltage; //uV
                ulong pkToPk = _pkToPk; //uV
                //Imports.enPS5000AWaveType waveType = Imports.enPS5000AWaveType.PS5000A_SINE;
                Imports.enPS5000AWaveType waveType = (Imports.enPS5000AWaveType)waveforms.GetValue(rangeinds[4]);
                float startFrequency = sweeprate; // 80000; // Hz
                float stopFrequency = startFrequency;
                float increment = 1.0F; //Hz
                float dwellTime = 1.0F; //seconds on each frequency increment
                Imports.enPS5000ASweepType sweepType = Imports.enPS5000ASweepType.PS5000A_UPDOWN;
                Imports.enPS5000AExtraOperations operation = Imports.enPS5000AExtraOperations.PS5000A_ES_OFF;
                ulong shots = Imports.PS5000A_SHOT_SWEEP_TRIGGER_CONTINUOUS_RUN;
                ulong sweeps = Imports.PS5000A_SHOT_SWEEP_TRIGGER_CONTINUOUS_RUN;
                Imports.enPS5000ASigGenTrigType triggerType = Imports.enPS5000ASigGenTrigType.PS5000A_SIGGEN_RISING;
                Imports.enPS5000ASigGenTrigSource triggerSource = Imports.enPS5000ASigGenTrigSource.PS5000A_SIGGEN_NONE;
                short extInThreshold = 0;

                if ((pkToPk + (ulong)Math.Abs(offsetVoltage * 2)) > 4000000)
                {
                    pkToPk = 2000000 - (ulong)Math.Abs(offsetVoltage);
                    _pkToPk = pkToPk;
                }
                retval = Imports.ps5000aSetSigGenBuiltIn(_handle, offsetVoltage, pkToPk, waveType, startFrequency, stopFrequency, increment,
                                            dwellTime, sweepType, operation, shots, sweeps,
                                            triggerType, triggerSource, extInThreshold);
                if (retval == Imports.PICO_OK)
                {
                    devicelogs += "Generator Started\n";
                }
                else
                {
                    devicelogs += "ERROR: Generator not Started\n";
                }
            }
        }

        static public void stopgenerator()
        {
            if (_handle >= 0)
            {
                ulong retval = Imports.PICO_OK;
                long offsetVoltage = 0; //uV
                ulong pkToPk =0; //uV
                Imports.enPS5000AWaveType waveType = Imports.enPS5000AWaveType.PS5000A_SINE;
                float startFrequency = 80000; // Hz
                float stopFrequency = startFrequency;
                float increment = 1.0F; //Hz
                float dwellTime = 1.0F; //seconds on each frequency increment
                Imports.enPS5000ASweepType sweepType = Imports.enPS5000ASweepType.PS5000A_UPDOWN;
                Imports.enPS5000AExtraOperations operation = Imports.enPS5000AExtraOperations.PS5000A_ES_OFF;
                ulong shots = Imports.PS5000A_SHOT_SWEEP_TRIGGER_CONTINUOUS_RUN;
                ulong sweeps = Imports.PS5000A_SHOT_SWEEP_TRIGGER_CONTINUOUS_RUN;
                Imports.enPS5000ASigGenTrigType triggerType = Imports.enPS5000ASigGenTrigType.PS5000A_SIGGEN_RISING;
                Imports.enPS5000ASigGenTrigSource triggerSource = Imports.enPS5000ASigGenTrigSource.PS5000A_SIGGEN_NONE;
                short extInThreshold = 0;
                retval = Imports.ps5000aSetSigGenBuiltIn(_handle, offsetVoltage, pkToPk, waveType, startFrequency, stopFrequency, increment,
                                            dwellTime, sweepType, operation, shots, sweeps,
                                            triggerType, triggerSource, extInThreshold);
                if (retval == Imports.PICO_OK)
                {
                    devicelogs += "Generator Stopped\n";
                }
                else
                {
                    devicelogs += "ERROR: Generator not Stopped\n";
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        //file io
        static void WriteTest()
        {
            // Write sample data to CSV file
            using (CsvFileWriter writer = new CsvFileWriter("WriteTest.csv"))
            {
                for (int i = 0; i < 100; i++)
                {
                    CsvRow row = new CsvRow();
                    for (int j = 0; j < 5; j++)
                        row.Add(String.Format("Column{0}", j));
                    writer.WriteRow(row);
                }
            }
        }

        static void ReadTest()
        {
            // Read sample data from CSV file
            using (CsvFileReader reader = new CsvFileReader("ReadTest.csv"))
            {
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    foreach (string s in row)
                    {
                        Console.Write(s);
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                }
            }
        }

    }
}
