using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace machineClassLibrary
{
    /// <summary>
    /// Class to store one CSV row
    /// </summary>
    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }

    /// <summary>
    /// Class to write data to a CSV file
    /// </summary>
    public class CsvFileWriter : StreamWriter
    {
        public CsvFileWriter(Stream stream)
            : base(stream)
        {
        }

        public CsvFileWriter(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }

    /// <summary>
    /// Class to read data from a CSV file
    /// </summary>
    public class CsvFileReader : StreamReader
    {
        public CsvFileReader(Stream stream)
            : base(stream)
        {
        }

        public CsvFileReader(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Reads a row of data from a CSV file
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool ReadRow(CsvRow row)
        {
            row.LineText = ReadLine();
            if (String.IsNullOrEmpty(row.LineText))
                return false;

            int pos = 0;
            int rows = 0;

            while (pos < row.LineText.Length)
            {
                string value;

                // Special handling for quoted field
                if (row.LineText[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    int start = pos;
                    while (pos < row.LineText.Length)
                    {
                        // Test for quote character
                        if (row.LineText[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (pos >= row.LineText.Length || row.LineText[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = row.LineText.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = pos;
                    while (pos < row.LineText.Length && row.LineText[pos] != ',')
                        pos++;
                    value = row.LineText.Substring(start, pos - start);
                }

                // Add field to list
                if (rows < row.Count)
                    row[rows] = value;
                else
                    row.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < row.LineText.Length && row.LineText[pos] != ',')
                    pos++;
                if (pos < row.LineText.Length)
                    pos++;
            }
            // Delete any unused items
            while (row.Count > rows)
                row.RemoveAt(rows);

            // Return true if any columns read
            return (row.Count > 0);
        }
    }

    static public class fio
    {
        //usage
        static public void writemachinevalues(string fn/*, machine m*/)
        {
            int i;
            CsvRow columns;
            using (var writer = new CsvFileWriter(fn))
            {
                //place parameter headers
                columns = new CsvRow();
                columns.Add("Ch A st.");
                columns.Add("Ch B st.");
                columns.Add("Ch C st.");
                columns.Add("Ch D st.");
                columns.Add("Gen. st.");
                columns.Add("Ch A rng.");
                columns.Add("Ch B rng.");
                columns.Add("Ch C rng.");
                columns.Add("Ch D rng.");
                columns.Add("Gen wfm.");
                columns.Add("LP Voltmeter Atten.");
                columns.Add("RP Voltmeter Atten.");
                columns.Add("Shunt Res.");
                columns.Add("Num. samp. in sweep");
                columns.Add("Num. sweeps");
                columns.Add("Sweeprate");
                columns.Add("Median window");
                columns.Add("Guard interval");
                columns.Add("Diff. interval");
                columns.Add("Trig. threshold");
                columns.Add("Probe length");
                columns.Add("Probe diameter");
                columns.Add("Probe cleaning potential");
                columns.Add("Probe cleaning time");
                columns.Add("PkToPk Vltg.");
                columns.Add("Offset Vltg.");
                writer.WriteRow(columns);

                //place parameters
                columns = new CsvRow();
                columns.Add(mach.channelson[0].ToString());
                columns.Add(mach.channelson[1].ToString());
                columns.Add(mach.channelson[2].ToString());
                columns.Add(mach.channelson[3].ToString());
                columns.Add(mach.channelson[4].ToString());
                columns.Add(mach.rangeinds[0].ToString());
                columns.Add(mach.rangeinds[1].ToString());
                columns.Add(mach.rangeinds[2].ToString());
                columns.Add(mach.rangeinds[3].ToString());
                columns.Add(mach.rangeinds[4].ToString());
                columns.Add(mach.LPVoltmeterAttenuation.ToString());
                columns.Add(mach.RPVoltmeterAttenuation.ToString());
                columns.Add(mach.CurrentMeterShuntResistance.ToString());
                columns.Add(mach.numberofsamplesinsweep.ToString());
                columns.Add(mach.numberofsweeps.ToString());
                columns.Add(mach.sweeprate.ToString());
                columns.Add(mach.medianwindow.ToString());
                columns.Add(mach.guardinterval.ToString());
                columns.Add(mach.diffinterval.ToString());
                columns.Add(mach.triggerthreshold.ToString());
                columns.Add(mach.probelength.ToString());
                columns.Add(mach.probediameter.ToString());
                columns.Add(mach.probecleaningpotential.ToString());
                columns.Add(mach.probecleaningtime.ToString());
                columns.Add(mach._pkToPk.ToString());
                columns.Add(mach._offsetVoltage.ToString());
                writer.WriteRow(columns);


                //dependent pars.

                //columns.Add(m.bufferlength.ToString());
                //columns.Add(m.samplingrate.ToString());
                //columns.Add(m.timebase.ToString());
                //writer.WriteRow(columns);

                //place data headers
                columns = new CsvRow();
                columns.Add("U LP [V]");
                columns.Add("I LP [A]");
                columns.Add("U RP [V]");
                columns.Add("U D [V]");
                writer.WriteRow(columns);
                // Write each row of data
                for (i = 0; i < mach.numberofsamplesinsweep; i++)
                {
                    //place data values
                    columns = new CsvRow();
                    if (mach.vlp != null && mach.vlp.Length > i)
                        columns.Add(mach.vlp[i].ToString());
                    else
                        columns.Add(" ");
                    if (mach.ilp != null && mach.ilp.Length > i)
                        columns.Add(mach.ilp[i].ToString());
                    else
                        columns.Add(" ");
                    if (mach.vrp != null && mach.vrp.Length > i)
                        columns.Add(mach.vrp[i].ToString());
                    else
                        columns.Add(" ");
                    if (mach.channelDvoltage != null && mach.channelDvoltage.Length > i)
                        columns.Add(mach.channelDvoltage[i].ToString());
                    else
                        columns.Add(" ");
                    writer.WriteRow(columns);
                }
            }
        }

        static public void readmachinevalues(string fn/*, machine *m*/)
        {
            CsvRow columns;
            //coefficients = new double[3];
            //prms = new uint[6];
            ////List<string> columns = new List<string>();
            using (var reader = new CsvFileReader(fn))
            {
                int n = 0;
                columns = new CsvRow();
                reader.ReadRow(columns); //read and discard headers
                columns = new CsvRow();
                reader.ReadRow(columns); //read and discard headers
                //unsafe
                {
                    mach.channelson[0] = Convert.ToBoolean(columns[n++]);
                    mach.channelson[1] = Convert.ToBoolean(columns[n++]);
                    mach.channelson[2] = Convert.ToBoolean(columns[n++]);
                    mach.channelson[3] = Convert.ToBoolean(columns[n++]);
                    mach.channelson[4] = Convert.ToBoolean(columns[n++]);
                    mach.rangeinds[0] = Convert.ToInt32(columns[n++]);
                    mach.rangeinds[1] = Convert.ToInt32(columns[n++]);
                    mach.rangeinds[2] = Convert.ToInt32(columns[n++]);
                    mach.rangeinds[3] = Convert.ToInt32(columns[n++]);
                    mach.rangeinds[4] = Convert.ToInt32(columns[n++]);
                    mach.LPVoltmeterAttenuation = Convert.ToDouble(columns[n++]);
                    mach.RPVoltmeterAttenuation = Convert.ToDouble(columns[n++]);
                    mach.CurrentMeterShuntResistance = Convert.ToDouble(columns[n++]);
                    mach.numberofsamplesinsweep = Convert.ToUInt32(columns[n++]);
                    mach.numberofsweeps = Convert.ToUInt32(columns[n++]);
                    mach.sweeprate = Convert.ToUInt32(columns[n++]);
                    mach.medianwindow = Convert.ToUInt32(columns[n++]);
                    mach.guardinterval = Convert.ToUInt32(columns[n++]);
                    mach.diffinterval = Convert.ToUInt32(columns[n++]);
                    mach.triggerthreshold = Convert.ToInt16(columns[n++]);
                    mach.probelength = Convert.ToDouble(columns[n++]);
                    mach.probediameter = Convert.ToDouble(columns[n++]);
                    mach.probecleaningpotential = Convert.ToDouble(columns[n++]);
                    mach.probecleaningtime = Convert.ToDouble(columns[n++]);
                    mach._pkToPk = Convert.ToUInt64(columns[n++]);
                    mach._offsetVoltage = Convert.ToInt64(columns[n++]);
                }
                while (reader.ReadRow(columns))
                {


                }
            }
        }

    }
}
