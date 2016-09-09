using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PS5000AStreamingConsole;

using machineClassLibrary;
using OxyPlot;
using Microsoft.Win32;
using System.IO;
using System.Timers;
using System.Windows.Threading;

namespace machineUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///



    public partial class MainWindow : Window
    {
        //public machine mach { get; set; }
        DispatcherTimer timer { get; set; }

        public MainWindow()
        {
            int i;
            string[] args = Environment.GetCommandLineArgs();
            var arguments = new Dictionary<string, int>();
            for (i = 1; i < args.Length; i += 2)
            {
                int p;
                if (int.TryParse(args[i + 1], out p))
                {
                    arguments.Add(args[i], p);
                }
            }

            InitializeComponent();
            //mach = new machine { deviceisopen = false, devicelogs = ""};
            //App.mach = machine;
            mach.machineinit();
            //mach = App.mach;
            //{
                
                comboBoxRangeA.ItemsSource = mach.ranges;
                comboBoxRangeA.SelectedIndex = mach.rangeinds[0] = arguments.TryGetValue("ra", out i) ? i : comboBoxRangeA.Items.Count - 1;
                comboBoxRangeB.ItemsSource = mach.ranges;
                comboBoxRangeB.SelectedIndex = mach.rangeinds[1] = arguments.TryGetValue("rb", out i) ? i : comboBoxRangeB.Items.Count - 1;
                comboBoxRangeC.ItemsSource = mach.ranges;
                comboBoxRangeC.SelectedIndex = mach.rangeinds[2] = arguments.TryGetValue("rc", out i) ? i : comboBoxRangeC.Items.Count - 1;
                comboBoxRangeD.ItemsSource = mach.ranges;
                comboBoxRangeD.SelectedIndex = mach.rangeinds[3] = arguments.TryGetValue("rd", out i) ? i : comboBoxRangeD.Items.Count - 1;
                comboBoxRangeG.ItemsSource = mach.waveforms;
                comboBoxRangeG.SelectedIndex = mach.rangeinds[4] = arguments.TryGetValue("wf", out i) ? i : 3;
                sweepfrequencySlider.Value = mach.sweeprate = arguments.TryGetValue("sr", out i) ? (uint)i : 100;

                numberofsamplesinsweepSlider.Value = mach.numberofsamplesinsweep = arguments.TryGetValue("ns", out i) ? (uint)i : 1000;
                numberofsweepsSlider.Value = mach.numberofsweeps = arguments.TryGetValue("ss", out i) ? (uint)i : 10;

                LPVoltmeterAttenuationSlider.Value = mach.LPVoltmeterAttenuation = arguments.TryGetValue("lp", out i) ? (double)i/1000 : 19.9;
                RPVoltmeterAttenuationSlider.Value = mach.RPVoltmeterAttenuation = arguments.TryGetValue("rp", out i) ? (double)i / 1000 : 19.9;
                CurrentMeterShuntResistanceSlider.Value = mach.CurrentMeterShuntResistance = arguments.TryGetValue("sh", out i) ? (double)i / 1000.0 : 3.33;

                medianwindowSlider.Value = mach.medianwindow = arguments.TryGetValue("mw", out i) ? (uint)i : 100;
                guardintervalSlider.Value = mach.guardinterval = arguments.TryGetValue("gi", out i) ? (uint)i : 100;
                diffintervalSlider.Value = mach.diffinterval = arguments.TryGetValue("di", out i) ? (uint)i : 10;
                triggerthresholdSlider.Value = mach.triggerthreshold = arguments.TryGetValue("tt", out i) ? (short)i : (short)0;

                ProbeLengthSlider.Value = mach.probelength = arguments.TryGetValue("pl", out i) ? (double)i/1000.0 : 1.0;
                ProbeDiameterSlider.Value = mach.probediameter = arguments.TryGetValue("pd", out i) ? (uint)i/1000.0 : 0.3;
                ProbeCleaningPotentialSlider.Value = mach.probecleaningpotential = arguments.TryGetValue("cp", out i) ? (double)i/1000.0 : -0.5;
                ProbeCleaningTimerSlider.Value = mach.probecleaningtime = arguments.TryGetValue("ct", out i) ? (double)i/1000.0 : 0.1;

                buttonA.Background = (mach.channelson[0] = arguments.TryGetValue("ea", out i) ? (bool)(i != 0) : true) ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
                buttonB.Background = (mach.channelson[1] = arguments.TryGetValue("eb", out i) ? (bool)(i != 0) : true) ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
                buttonC.Background = (mach.channelson[2] = arguments.TryGetValue("ec", out i) ? (bool)(i != 0) : true) ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
                buttonD.Background = (mach.channelson[3] = arguments.TryGetValue("ed", out i) ? (bool)(i != 0) : false) ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
                buttonG.Background = (mach.channelson[4] = arguments.TryGetValue("eg", out i) ? (bool)(i != 0) : true) ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);

                sliderGAmplitude.Value = mach._pkToPk = arguments.TryGetValue("pp", out i) ? (ulong)i : (ulong)0;
                sliderGDCOffset.Value = mach._offsetVoltage = arguments.TryGetValue("do", out i) ? (long)i : (long)0;
                ////Console.WriteLine("pkTopk" + i.ToString() + "\n");
                canvasOpen.Background = buttonOpen.Background = new SolidColorBrush(Colors.Yellow);
                buttonAcquisitionStart.IsEnabled = false;
                canvasAsquisitionStart.Background = buttonAcquisitionStart.Background = new SolidColorBrush(Colors.LightSeaGreen);
            //}

            //for continuous acquisition
            timer = new DispatcherTimer();
            TimerSlider.Value = 1.0;
            timer.Interval = TimeSpan.FromSeconds(TimerSlider.Value);
            timer.Tick += timer_Tick;
            timer.Stop();
            checkBoxContinuous.IsChecked = false;
            checkBoxContinuous.IsEnabled = false;


        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            buttonStartAcquisition_Click(sender, (RoutedEventArgs)null);

            if (checkBoxContinuous.IsChecked == true)
            {
                timer.Start();
            }
            else
            {
                checkBoxContinuous.Background = new SolidColorBrush(Colors.Gray);
            }
        }



        private void buttonOpenDevice_Click(object sender, RoutedEventArgs e)
        {
            mach.opendevice();
            checkBoxContinuous.IsEnabled = mach.deviceisopen ? true : false;
            buttonAcquisitionStart.IsEnabled = mach.deviceisopen ? true : false;
            canvasOpen.Background = mach.deviceisopen ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.Yellow);
            buttonOpen.Content = mach.deviceisopen ? "Close Device" : "Open Device";
            buttonOpen.Background = canvasOpen.Background;
            textBlockLog.Text += mach.devicelogs;   ///mach.deviceisopen ? "opened\n":"closed\n";
            textBlockLogScroll.ScrollToBottom();

            //timer.Start();
        }

        private void buttonStartAcquisition_Click(object sender, RoutedEventArgs e)
        {
            if (mach.deviceisopen)
            {
                buttonAcquisitionStart.IsEnabled = false;
                if (mach.channelson[4])
                {
                    canvasGAmplitude.Background = new SolidColorBrush(Colors.LightSeaGreen);
                    canvasGDCOffset.Background = new SolidColorBrush(Colors.LightSeaGreen);
                }
                else
                {
                    canvasGAmplitude.Background = new SolidColorBrush(Colors.Yellow);
                    canvasGDCOffset.Background = new SolidColorBrush(Colors.Yellow);
                }
                mach._pkToPk = (ulong)sliderGAmplitude.Value;
                mach._offsetVoltage = (long)sliderGDCOffset.Value;

                mach.startacquisition();

                sliderGAmplitude.Value = mach._pkToPk;

                canvasAsquisitionStart.Background = new SolidColorBrush(Colors.LightSeaGreen);
                
                textBlockLog.Text += mach.devicelogs;   ///mach.deviceisopen ? "opened\n":"closed\n";
                textBlockLogScroll.ScrollToBottom();

                if (mach.channelAvoltage != null && mach.channelAvoltage.Length > 0)
                {
                    int i;
                    for (i = 0; i < 4; i++)
                        combinedview.Series[i].ItemsSource = new List<DataPoint>();

                    for (i = 0; i < 3; i++)
                        averagesweepsplot.Series[i].ItemsSource = new List<DataPoint>();

                    for (i = 0; i < 3; i++)
                        logplot.Series[i].ItemsSource = new List<DataPoint>();

                    eepdlogplot.Series[0].ItemsSource = new List<DataPoint>();

                    for (i = 0; i < 3; i++)
                        linplot.Series[i].ItemsSource = new List<DataPoint>();


                    ////didvplot.Series[0].ItemsSource = new List<DataPoint>();

                    ////linearview1.Series[0].ItemsSource = new List<DataPoint>();

                    ////linearview2.Series[0].ItemsSource = new List<DataPoint>();

                    for (i = 0; i < mach.bufferlength; i++)
                    {
                        (combinedview.Series[0].ItemsSource as List<DataPoint>).Add(new DataPoint((double)i, mach.BufferA[i]));
                        (combinedview.Series[1].ItemsSource as List<DataPoint>).Add(new DataPoint((double)i, mach.BufferB[i]));
                        (combinedview.Series[2].ItemsSource as List<DataPoint>).Add(new DataPoint((double)i, mach.BufferC[i]));
                        (combinedview.Series[3].ItemsSource as List<DataPoint>).Add(new DataPoint((double)i, mach.BufferD[i]));
                    }

                    for (i = 0; i < mach.vlp.Length; i++)
                        (averagesweepsplot.Series[0].ItemsSource as List<DataPoint>).Add(new DataPoint((double)i, mach.vlp[i]/mach.vlpmax));
                    for (i = 0; i < mach.ilp.Length; i++)
                        (averagesweepsplot.Series[1].ItemsSource as List<DataPoint>).Add(new DataPoint((double)i, mach.ilp[i]/mach.ilpmax));
                    for (i = 0; i < mach.vrp.Length; i++)
                        (averagesweepsplot.Series[2].ItemsSource as List<DataPoint>).Add(new DataPoint((double)i, mach.vrp[i]/mach.vrpmax));


                    for (i = 0; i < mach.vp.Length && i<mach.ilp.Length; i++)
                        (logplot.Series[0].ItemsSource as List<DataPoint>).Add(new DataPoint(mach.vp[i], Math.Abs(mach.ilp[i])));
                    for (i = 0; i < mach.vp.Length && i<mach.dilp.Length; i++)
                        (logplot.Series[1].ItemsSource as List<DataPoint>).Add(new DataPoint(mach.vp[i], mach.ilpmax/mach.dilpmax*Math.Abs(mach.dilp[i])));
                    for (i = 0; i < mach.vp.Length && i < mach.d2ilp.Length; i++)
                        (logplot.Series[2].ItemsSource as List<DataPoint>).Add(new DataPoint(mach.vp[i], mach.ilpmax / mach.d2ilpmax * Math.Abs(mach.d2ilp[i])));

                    if (plasmacalculator.electronenergy != null)
                    {
                        if (plasmacalculator.electronenergy.Length > 0)
                        {
                            for (i = 0; i < plasmacalculator.electronenergy.Length; i++)
                                (eepdlogplot.Series[0].ItemsSource as List<DataPoint>).Add(new DataPoint(plasmacalculator.electronenergy[i], plasmacalculator.electronprobability[i]));
                            eepdlogplot.InvalidatePlot(true);
                        }
                    }


                    for (i = 0; i < mach.vp.Length && i < mach.ilp.Length; i++)
                        (linplot.Series[0].ItemsSource as List<DataPoint>).Add(new DataPoint(mach.vp[i], mach.ilp[i]));
                    for (i = 0; i < mach.vp.Length && i < mach.dilp.Length; i++)
                        (linplot.Series[1].ItemsSource as List<DataPoint>).Add(new DataPoint(mach.vp[i], mach.ilpmax / mach.dilpmax * mach.dilp[i]));
                    for (i = 0; i < mach.vp.Length && i < mach.d2ilp.Length; i++)
                        (linplot.Series[2].ItemsSource as List<DataPoint>).Add(new DataPoint(mach.vp[i], mach.ilpmax / mach.d2ilpmax * mach.d2ilp[i]));

                    plasmacalculator.get_plasmatemp_plasmapotential(mach.vp,mach.ilp,mach.dilp,mach.d2ilp);
                    textBlockResult.Text = 
                        "Acquired\t\t" + mach.numberofsweeps.ToString() + " sweeps\n" +
                        "Ion saturation current\t" + plasmacalculator.ion_saturation_current_a + "A.\n" +
                        "Floating potential\t\t" + plasmacalculator.floating_potential_v.ToString() + " V.\n" +
                        "Plasma potential\t\t" + plasmacalculator.plasma_potential_v.ToString() + " V.\n" +
                        "Plasma density\t\t" + plasmacalculator.plasma_density.ToString() + "\n" +
                        "Plasma Temperature\t" + plasmacalculator.plasma_temperature_ev.ToString() + " eV.\n";
                    textBlockResultScroll.ScrollToEnd();

                    combinedview.InvalidatePlot(true);
                    averagesweepsplot.InvalidatePlot(true);
                    logplot.InvalidatePlot(true);
                    linplot.InvalidatePlot(true);
                    ////didvplot.InvalidatePlot(true);
                    ////linearview1.InvalidatePlot(true);
                    ////linearview2.InvalidatePlot(true);
                    buttonAcquisitionStart.IsEnabled = !(bool)(checkBoxContinuous.IsChecked);
                }

            }
            else
            {
                canvasAsquisitionStart.Background = new SolidColorBrush(Colors.Red);
                textBlockLog.Text += "Device not ready\n";   ///mach.deviceisopen ? "opened\n":"closed\n";
                textBlockLogScroll.ScrollToBottom();
            }
        }


        private void comboBoxRangeA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mach.rangeinds[0] = comboBoxRangeA.SelectedIndex;
            //if (mach.deviceisopen) buttonOpenDevice_Click(sender, e);
        }
        private void comboBoxRangeB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mach.rangeinds[1] = comboBoxRangeB.SelectedIndex;
            //if (mach.deviceisopen) buttonOpenDevice_Click(sender, e);
        }
        private void comboBoxRangeC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mach.rangeinds[2] = comboBoxRangeC.SelectedIndex;
            //if (mach.deviceisopen) buttonOpenDevice_Click(sender, e);
        }
        private void comboBoxRangeD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mach.rangeinds[3] = comboBoxRangeD.SelectedIndex;
            //if (mach.deviceisopen) buttonOpenDevice_Click(sender, e);
        }
        private void comboBoxRangeG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mach.rangeinds[4] = comboBoxRangeG.SelectedIndex;
            //if (mach.deviceisopen) buttonOpenDevice_Click(sender, e);
        }

        private void buttonA_Click(object sender, RoutedEventArgs e)
        {
            buttonA.Background = (mach.channelson[0] = !mach.channelson[0]) ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            //if (mach.deviceisopen) buttonOpenDevice_Click(sender, e);
        }

        private void buttonB_Click(object sender, RoutedEventArgs e)
        {
            buttonB.Background = (mach.channelson[1] = !mach.channelson[1]) ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            //if (mach.deviceisopen) buttonOpenDevice_Click(sender, e);
        }

        private void buttonC_Click(object sender, RoutedEventArgs e)
        {
            buttonC.Background = (mach.channelson[2] = !mach.channelson[2]) ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            //if (mach.deviceisopen) buttonOpenDevice_Click(sender, e);
        }

        private void buttonD_Click(object sender, RoutedEventArgs e)
        {
            buttonD.Background = (mach.channelson[3] = !mach.channelson[3]) ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            //if (mach.deviceisopen) buttonOpenDevice_Click(sender, e);
        }

        private void buttonG_Click(object sender, RoutedEventArgs e)
        {
            buttonG.Background = (mach.channelson[4] = !mach.channelson[4]) ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            //if (mach.deviceisopen) buttonOpenDevice_Click(sender, e);
        }

        private void buttonStartGenerator_Click(object sender, RoutedEventArgs e)
        {
            if (mach.deviceisopen)
            {
                ////buttonStartGenerator.IsEnabled = false;
                mach.startgenerator();
                canvasGAmplitude.Background = new SolidColorBrush(Colors.LightSeaGreen);
                //buttonStartGenerator.IsEnabled = true;
                textBlockLog.Text += mach.devicelogs;   ///mach.deviceisopen ? "opened\n":"closed\n";
                textBlockLogScroll.ScrollToBottom();
            }
        }

        private void parametersChanged(object sender, RoutedEventArgs e)
        {
            if (//mach!=null && 
                numberofsamplesinsweepSlider != null && 
                numberofsweepsSlider != null && 
                sweepfrequencySlider != null && 
                samplingrateLable != null && 
                bufferlengthLable != null &&
                LPVoltmeterAttenuationSlider != null &&
                RPVoltmeterAttenuationSlider != null &&
                CurrentMeterShuntResistanceSlider != null &&
                sliderGAmplitude != null &&
                sliderGDCOffset != null &&
                medianwindowSlider != null &&
                guardintervalSlider != null &&
                diffintervalSlider != null &&
                triggerthresholdSlider != null
                )
            {
                uint numberofsamplesinsweep = (uint)numberofsamplesinsweepSlider.Value;
                uint numberofsweeps = (uint)numberofsweepsSlider.Value;
                uint sweepfrequency = (uint)sweepfrequencySlider.Value;
                uint guardinterval = (uint)guardintervalSlider.Value;
                uint diffinterval = (uint)diffintervalSlider.Value;
                short triggerthreshold = (short)triggerthresholdSlider.Value;


                uint buflen, samprate, timebase;

                buflen = (numberofsamplesinsweep + guardinterval * 2) * numberofsweeps;
                samprate = (numberofsamplesinsweep + guardinterval * 2) * sweepfrequency;
                timebase = 2 + 125000000 / samprate;

                bufferlengthLable.Content = buflen;
                samplingrateLable.Content = samprate;
                timebaseLable.Content = timebase;

                mach.sweeprate = sweepfrequency;
                mach.numberofsweeps = numberofsweeps;
                mach.numberofsamplesinsweep = numberofsamplesinsweep;
                mach.samplingrate = samprate;
                mach.bufferlength = buflen;
                mach.timebase = timebase;


                mach.LPVoltmeterAttenuation = LPVoltmeterAttenuationSlider.Value;
                mach.RPVoltmeterAttenuation = RPVoltmeterAttenuationSlider.Value;
                mach.CurrentMeterShuntResistance = CurrentMeterShuntResistanceSlider.Value;

                mach.medianwindow = (uint)medianwindowSlider.Value;
                mach.guardinterval = (uint)guardintervalSlider.Value;
                mach.diffinterval = (uint)diffintervalSlider.Value;
                mach.triggerthreshold = (short)triggerthresholdSlider.Value;

                mach._pkToPk = (ulong)sliderGAmplitude.Value;
                mach._offsetVoltage = (long)sliderGDCOffset.Value ;
            }
        }

        private void parametersUpdated()
        {
            //machine m = mach;
            buttonA.Background = mach.channelson[0] ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            buttonA.InvalidateArrange();
            buttonB.Background = mach.channelson[1] ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            buttonB.InvalidateArrange();
            buttonC.Background = mach.channelson[2] ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            buttonC.InvalidateArrange();
            buttonD.Background = mach.channelson[3] ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            buttonD.InvalidateArrange();
            buttonG.Background = mach.channelson[4] ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            buttonG.InvalidateArrange();
            comboBoxRangeA.SelectedIndex = mach.rangeinds[0];
            comboBoxRangeA.InvalidateArrange();
            comboBoxRangeB.SelectedIndex = mach.rangeinds[1];
            comboBoxRangeB.InvalidateArrange();
            comboBoxRangeC.SelectedIndex = mach.rangeinds[2];
            comboBoxRangeC.InvalidateArrange();
            comboBoxRangeD.SelectedIndex = mach.rangeinds[3];
            comboBoxRangeD.InvalidateArrange();
            comboBoxRangeG.SelectedIndex = mach.rangeinds[4];
            LPVoltmeterAttenuationSlider.Value = mach.LPVoltmeterAttenuation;
            LPVoltmeterAttenuationSlider.InvalidateArrange();
            RPVoltmeterAttenuationSlider.Value = mach.RPVoltmeterAttenuation;
            RPVoltmeterAttenuationSlider.InvalidateArrange();
            CurrentMeterShuntResistanceSlider.Value = mach.CurrentMeterShuntResistance;
            CurrentMeterShuntResistanceSlider.InvalidateArrange();
            numberofsamplesinsweepSlider.Value = mach.numberofsamplesinsweep;
            numberofsamplesinsweepSlider.InvalidateArrange();
            numberofsweepsSlider.Value = mach.numberofsweeps;
            numberofsweepsSlider.InvalidateArrange();
            sweepfrequencySlider.Value = mach.sweeprate;
            sweepfrequencySlider.InvalidateArrange();
            medianwindowSlider.Value = mach.medianwindow;
            medianwindowSlider.InvalidateArrange();
            guardintervalSlider.Value = mach.guardinterval;
            guardintervalSlider.InvalidateArrange();
            diffintervalSlider.Value = mach.diffinterval;
            diffintervalSlider.InvalidateArrange();
            triggerthresholdSlider.Value = mach.triggerthreshold;
            triggerthresholdSlider.InvalidateArrange();
            ProbeLengthSlider.Value = mach.probelength;
            ProbeLengthSlider.InvalidateArrange();
            ProbeDiameterSlider.Value = mach.probediameter;
            ProbeDiameterSlider.InvalidateArrange();
            ProbeCleaningPotentialSlider.Value = mach.probecleaningpotential;
            ProbeCleaningPotentialSlider.InvalidateArrange();
            ProbeCleaningTimerSlider.Value = mach.probecleaningtime;
            ProbeCleaningTimerSlider.InvalidateArrange();
            sliderGAmplitude.Value = mach._pkToPk;
            sliderGAmplitude.InvalidateArrange();
            sliderGDCOffset.Value = mach._offsetVoltage;
            sliderGDCOffset.InvalidateArrange();
        }


        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtEditor.Text = "File: " + openFileDialog.FileName + "\n\n" + File.ReadAllText(openFileDialog.FileName);

                //unsafe
                {
                    fio.readmachinevalues(openFileDialog.FileName/*, mach*/);
                }
                parametersUpdated();
                tabControl.InvalidateVisual();
            }
        }

        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                fio.writemachinevalues(saveFileDialog.FileName/*, mach*/);
                txtEditor.Text = "File: " + saveFileDialog.FileName + "\n\n" + File.ReadAllText(saveFileDialog.FileName);
            }
        }

        private void checkBoxContinuous_Unchecked(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            checkBoxContinuous.Background = new SolidColorBrush(Colors.Red);
            buttonAcquisitionStart.IsEnabled = mach.deviceisopen ? true : false;
            btnSaveFile.IsEnabled = true;
        }

        private void checkBoxContinuous_Checked(object sender, RoutedEventArgs e)
        {
            timer.Start();
            checkBoxContinuous.Background = new SolidColorBrush(Colors.LightGreen);
            buttonAcquisitionStart.IsEnabled = false;
            btnSaveFile.IsEnabled = false;
        }

        private void TimerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer != null)
            {
                timer.Interval = TimeSpan.FromSeconds(TimerSlider.Value);
            }
        }
    }
}
