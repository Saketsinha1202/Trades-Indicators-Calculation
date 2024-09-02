using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TTNETAPI_Sample_Console_AlgoOrderRouting
{
    public class HLC_Values
    {
        public string TimeFrame { get; set; }
        public double Parameter_Open { get; set; }
        public double Parameter_High { get; set; }
        public double Parameter_Low { get; set; }
        public double Parameter_Close { get; set; }
    }
    public class Boolinger_Values
    {
        public double UpperBand { get; set; }
        public double SMA { get; set; }
        public double LowerBand { get; set; }
    }
    public class Keltner_Values
    {
        public double UPPER { get; set; }
        public double MIDDLE { get; set; }
        public double LOWER { get; set; }
    }
    public class MACD
    {
        public double MACDLine { get; set; }
        public double SignalLine { get; set; }
    }
    class Program
    {
        static void Main()
        {
            Console.WriteLine("lslsls");
            String path = @"/Users/saketsinha/Desktop/All work/Visual Studio/Data/ZS Jul24_60min.CSV";
            String[] lines = ReadCSV(path);

            List<HLC_Values> myList = ParseCSV(lines);
            ExecuteIndicatorAlgo(lines);


        }

        static string[] ReadCSV(string filePath)
        {
            return File.ReadAllLines(filePath);
        }
        static List<HLC_Values> ParseCSV(string[] lines)
        {
            List<HLC_Values> myList = new List<HLC_Values>();
            for (int i = 1; i < lines.Length; i++) // Start from the second line (index 1)
            {
                string line = lines[i];
                string[] values = line.Split(',');
                string value1 = values[0];
                double value2 = double.Parse(values[1]);
                double value3 = double.Parse(values[2]);
                double value4 = double.Parse(values[3]);
                double value5 = double.Parse(values[4]);
                myList.Add(new HLC_Values { TimeFrame = value1, Parameter_Open = value2, Parameter_High = value3, Parameter_Low = value4, Parameter_Close = value5 });
            }

            return myList;
        }

        static void WriteToCSV(List<double> indicator, List<HLC_Values> data, double timePeriod, String Indicator_name)
        {
            string filePath = @"C:\Users\sinha\OneDrive\Desktop\VS_Works\Result.CSV";
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    int i = 0, j = 0;
                    writer.WriteLine("TimeFrame,Parameter_Open,Parameter_High,Parameter_Low,Parameter_Close," + Indicator_name);
                    foreach (var item in data)
                    {
                        if (j < data.Count - indicator.Count && Indicator_name != ".KC" && Indicator_name != ".BB" && Indicator_name != ".MACD")
                        {

                            writer.WriteLine($"{item.TimeFrame},{item.Parameter_Open},{item.Parameter_High},{item.Parameter_Low},{item.Parameter_Close}");
                        }
                        else if (Indicator_name == ".KC" || (Indicator_name == ".BB"))
                        {
                            if (j < data.Count - (indicator.Count / 3))
                                writer.WriteLine($"{item.TimeFrame},{item.Parameter_Open},{item.Parameter_High},{item.Parameter_Low},{item.Parameter_Close}");
                            else
                            {
                                writer.WriteLine($"{item.TimeFrame},{item.Parameter_Open},{item.Parameter_High},{item.Parameter_Low},{item.Parameter_Close},{indicator[i]},{indicator[i + 1]},{indicator[i + 2]}");
                                i += 3;
                            }
                        }
                        else if (Indicator_name == ".MACD")
                        {
                            if (j < data.Count - indicator.Count / 2)
                                writer.WriteLine($"{item.TimeFrame},{item.Parameter_Open},{item.Parameter_High},{item.Parameter_Low},{item.Parameter_Close}");
                            else
                            {
                                writer.WriteLine($"{item.TimeFrame},{item.Parameter_Open},{item.Parameter_High},{item.Parameter_Low},{item.Parameter_Close},{indicator[i]},{indicator[i + 1]}");
                                i += 2;
                            }
                        }
                        else
                        {
                            writer.WriteLine($"{item.TimeFrame},{item.Parameter_Open},{item.Parameter_High},{item.Parameter_Low},{item.Parameter_Close},{indicator[i]}");
                            i++;
                        }
                        j++;

                    }
                }
                Console.WriteLine("Data written to CSV successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static (List<double>, List<HLC_Values>) MULTILEGS()
        {
            Console.WriteLine("Enter Number of Legs");
            string LEGS = Console.ReadLine();
            int l = int.Parse(LEGS);
            List<double> multi = new List<double>();

            List<HLC_Values> list = new List<HLC_Values>();

            List<string> path = new List<string>();
            path.Add(@"C:\Users\sinha\OneDrive\Desktop\VS_Works\ZS Jul24_60min.CSV");
            path.Add(@"C:\Users\sinha\OneDrive\Desktop\VS_Works\ZS Aug24_60min.CSV");
            path.Add(@"C:\Users\saket.sinha\Downloads\ZS Sep24_60min (7).CSV");
            path.Add(@"C:\Users\saket.sinha\Downloads\ZS Nov24_60min (1).CSV");
            path.Add(@"C:\Users\saket.sinha\Downloads\ZS Jan25_60min.CSV");

            // Initialize close list with zeros
            List<double> close = new List<double>();
            List<double> high = new List<double>();
            List<double> low = new List<double>();
            List<double> open = new List<double>();
            List<string> TimeFrame = new List<String>();
            for (int i = 0; i < l; i++)
            {
                Console.WriteLine($"Enter Multiple for Leg {i + 1}");
                string Multi = Console.ReadLine();
                multi.Add(double.Parse(Multi));
                String[] lines = ReadCSV(path[i]);

                // Initialize close, open, high, low for current leg with zeros
                close.Add(0); open.Add(0); high.Add(0); low.Add(0);

                if (i == 0)
                {
                    close.AddRange(new double[lines.Length-2]);
                    high.AddRange(new double[lines.Length-2]);
                    low.AddRange(new double[lines.Length-2]);
                    open.AddRange(new double[lines.Length - 2]);
                }

                int k = 0;
                // Process each line in the CSV file
                for (int j = 1; j < lines.Length; j++)
                {
                    string line = lines[j];
                    string[] values = line.Split(',');
                    string value1 = values[0];
                    double value2 = double.Parse(values[1]);
                    double value3 = double.Parse(values[2]);
                    double value4 = double.Parse(values[3]);
                    double value5 = double.Parse(values[4]);

                    // Add values to corresponding lists
                    TimeFrame.Add(value1);
                    close[k] += value5 * multi[i];
                    open[k] += value2 * multi[i];
                    high[k] += value3 * multi[i];
                    low[k] += value4 * multi[i];
                    k++;
                    
                }
            }

            for (int i = 0; i < close.Count; i++)
            {
                list.Add(new HLC_Values { TimeFrame = TimeFrame[i], Parameter_Open = open[i], Parameter_High = high[i], Parameter_Low = low[i], Parameter_Close = close[i] });
                
            }
            return (close, list);

        }
        static void ExecuteIndicatorAlgo(string[] lines)
        {
            try
            {
                List<HLC_Values> myList = ParseCSV(lines);
                Console.WriteLine("Enter Indicator Name");
                string indicator_name = Console.ReadLine();

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////    SIMPLE MOVING AVERAGE    ///////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (indicator_name == ".SMA")
                {
                    var (close, list) = MULTILEGS();
                    Console.WriteLine("Enter Period");
                    string time_period = Console.ReadLine();
                    double tp = double.Parse(time_period);
                    List<double> SMA = CalculateSMA(close, tp, indicator_name);
                    WriteToCSV(SMA, list, tp, indicator_name);
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////    STANDARD DEVIATION    //////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                else if (indicator_name == ".SD")
                {
                    var (close, list) = MULTILEGS();
                    Console.WriteLine("Enter Period");
                    string time_period = Console.ReadLine();
                    double tp = double.Parse(time_period);
                    List<double> SD = CalculateSD(close, tp, indicator_name);
                    WriteToCSV(SD, myList, tp, indicator_name);
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////    BOOLINGER BANDS    /////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                else if (indicator_name == ".BB")
                {
                    var (close, list) = MULTILEGS();
                    Console.WriteLine("Enter Period");
                    string time_period = Console.ReadLine();
                    double tp = double.Parse(time_period);
                    Console.WriteLine("Enter Deviation Factor");
                    string deviationFactor = Console.ReadLine();
                    double df = double.Parse(deviationFactor);
                    List<Boolinger_Values> Bool = CalculateBoolinger(close, tp, indicator_name, df);
                    List<double> BB = new List<double>();
                    foreach (var item in Bool)
                    {
                        BB.Add(item.UpperBand);
                        BB.Add(item.SMA);
                        BB.Add(item.LowerBand);
                    }
                    WriteToCSV(BB, myList, tp, indicator_name);
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////    EXPONENTIAL MOVING AVERAGE    //////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                else if (indicator_name == ".EMA")
                {
                    var (close, list) = MULTILEGS();
                    Console.WriteLine("Enter Period");
                    string time_period = Console.ReadLine();
                    double tp = double.Parse(time_period);
                    List<double> EMA = CalculateEMA(close, tp, indicator_name);
                    WriteToCSV(EMA, list, tp, indicator_name);
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////////////    AVERAGE TRUE RANGE    //////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                else if (indicator_name == ".ATR")
                {
                    Console.WriteLine("Enter Period");
                    string time_period = Console.ReadLine();
                    double tp = double.Parse(time_period);
                    List<double> ATR = CalculateATR(lines, tp, indicator_name);
                    WriteToCSV(ATR, myList, tp, indicator_name);
                }


                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////     KELTNER CHANNEL       ///////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                else if (indicator_name == ".KC")
                {
                    Console.WriteLine("Enter Period");
                    string time_period = Console.ReadLine();
                    double tp = double.Parse(time_period);
                    Console.WriteLine("Enter Moving Average Type: EMA OR SMA");
                    string mat = Console.ReadLine();
                    Console.WriteLine("Enter Shift Value");
                    string shift = Console.ReadLine();
                    int s = int.Parse(shift);
                    List<Keltner_Values> KC = CalculateKC(lines, tp, indicator_name, mat, s);
                    List<double> KTC = new List<double>();
                    foreach (var item in KC)
                    {
                        KTC.Add(item.UPPER);
                        KTC.Add(item.MIDDLE);
                        KTC.Add(item.LOWER);
                    }
                    WriteToCSV(KTC, myList, tp, indicator_name);
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////     MACD     ////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                else if (indicator_name == ".MACD")
                {
                    var (close, list) = MULTILEGS();
                    Console.WriteLine("Enter Fast MA Period");
                    string fmp = Console.ReadLine();
                    double f = double.Parse(fmp);
                    Console.WriteLine("Enter Slow MA Period");
                    string smp = Console.ReadLine();
                    double s = double.Parse(smp);
                    Console.WriteLine("Enter Signal Period");
                    string sp = Console.ReadLine();
                    double p = double.Parse(sp);
                    List<MACD> mcd = CalculateMACD(close, f, s, p, indicator_name);
                    List<double> macd = new List<double>();
                    foreach (var item in mcd)
                    {
                        macd.Add(item.MACDLine);
                        macd.Add(item.SignalLine);
                    }
                    WriteToCSV(macd, list, f, indicator_name);
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////     DCW      ///////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                else if (indicator_name == ".DCW")
                {
                    Console.WriteLine("Enter High Period");
                    string HighPeriod = Console.ReadLine();
                    double hp = double.Parse(HighPeriod);
                    Console.WriteLine("Enter Low Period");
                    string LowPeriod = Console.ReadLine();
                    double lp = double.Parse(LowPeriod);
                    List<double> DCW = CalculateDCW(lines, indicator_name, hp, lp);
                    WriteToCSV(DCW, myList, hp, indicator_name);
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////     SUPER TREND       ///////////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                else if (indicator_name == ".ST")
                {
                    Console.WriteLine("Enter Period");
                    string time_period = Console.ReadLine();
                    double tp = double.Parse(time_period);
                    Console.WriteLine("Enter Multiplier Value");
                    string Multiplier = Console.ReadLine();
                    double mv = double.Parse(Multiplier);
                    List<double> ST=CalculateST(lines, tp, indicator_name, mv);
                    WriteToCSV(ST, myList, tp, indicator_name);
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////     RSI       ///////////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                else if (indicator_name == ".RSI")
                {
                    Console.WriteLine("Enter Period");
                    string time_period = Console.ReadLine();
                    double tp = double.Parse(time_period);
                    CalculateRSI(lines, tp, indicator_name);
                }


                else
                {
                    Console.WriteLine("Wrong Indicator Name");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Wrong Input, " + e.Message);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------SIMPLE MOVING AVERAGE--------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------
        static List<double> CalculateSMA(List<double> close, double timePeriod, string indicator_name)
        {
            
            List<double> SMA = new List<double>();
            double sum = 0;
            for (int i = 0; i < timePeriod; i++)
            {
                sum += close[i];
            }
            SMA.Add((sum / timePeriod));
            for (int i = 0; i < (close.Count - timePeriod); i++)
            {
                sum -= close[i];
                sum += close[i + (int)timePeriod];
                SMA.Add((sum / timePeriod));
            }
            if (indicator_name == ".SMA")
            {
                foreach (var item in SMA)
                {
                    Console.WriteLine(item);
                }
            }
            return SMA;
        }
        //----------------------------------------------------------------------------------------
        static List<double> CalculateSD(List<double> close, double timePeriod, string indicator_name)
        {
            List<double> SMA = CalculateSMA(close, timePeriod, indicator_name);
            List<double> SDs = new List<double>();
            for (int i = 0; i < (close.Count - timePeriod + 1); i++)
            {
                double squaresum = 0;
                for (int j = i; j < i + timePeriod; j++)
                {
                    double thirdColumn = close[j];
                    squaresum += Math.Pow(thirdColumn - SMA[i], 2);
                }
                squaresum /= timePeriod;
                SDs.Add((Math.Sqrt(squaresum)));

            }
            if (indicator_name == ".SD")
            {
                foreach (var item in SDs)
                {
                    Console.WriteLine(item);
                }
            }
            return SDs;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------BOOLINGER BANDS--------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------
        static List<Boolinger_Values> CalculateBoolinger(List<double> close, double timePeriod, string indicator_name, double df)
        {

            List<double> SMA = CalculateSMA(close, timePeriod, indicator_name);
            List<double> SDs = CalculateSD(close, timePeriod, indicator_name);
            List<Boolinger_Values> BB = new List<Boolinger_Values>();
            for (int i = 0; i < SMA.Count; i++)
            {
                double value1 = SMA[i] + df * SDs[i];
                double value2 = SMA[i];
                double value3 = SMA[i] - df * SDs[i];
                BB.Add(new Boolinger_Values { UpperBand = value1, SMA = value2, LowerBand = value3 });
            }
            foreach (var item in BB)
            {

                Console.WriteLine($"{item.UpperBand},{item.SMA},{item.LowerBand}");
            }
            return BB;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------EXPONENTIAL MOVING AVERAGE--------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------
        static List<double> CalculateEMA(List<double> close, double timePeriod, string indicator_name)
        {
            List<double> SMA = CalculateSMA(close, timePeriod, indicator_name);
            List<double> EMA = new List<double>();
            double k = 2.0 / (timePeriod + 1);
            double ema = SMA[0]; // Initialize EMA with the first data point
            EMA.Add(ema);

            // Calculate EMA for subsequent data points
            for (int i = 1; i < SMA.Count; i++)
            {
                double thirdColumn = close[i + (int)timePeriod-1];
                ema = (thirdColumn - ema) * k + ema;
                EMA.Add(ema);
            }
            Console.WriteLine("HIII");
            if (indicator_name == ".EMA")
            {
                foreach (var item in EMA)
                {
                    Console.WriteLine(item);
                }
            }
            return EMA;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------Average True Range---------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------
        static List<double> CalculateATR(string[] lines, double timePeriod, string indicator_name)
        {

            List<HLC_Values> myList = ParseCSV(lines);
            List<double> trueRanges = new List<double>();
            List<double> ATR = new List<double>();

            for (int i = 1; i < myList.Count; i++)
            {
                string line = lines[i + 1];
                string[] columns = line.Split(',');
                double Value1 = double.Parse(columns[2]);
                double Value2 = double.Parse(columns[3]);
                line = lines[i];
                columns = line.Split(',');
                double Value3 = double.Parse(columns[4]);
                double trueRange = Math.Max(Value1 - Value2, Math.Max(Math.Abs(Value1 - Value3), Math.Abs(Value2 - Value3)));

                // Console.WriteLine(Value1 + "-" + Value2 + "-" + Value3);

                trueRanges.Add(trueRange);
            }
            double sum = 0;
            for (int i = 0; i < timePeriod; i++)
            {
                sum += trueRanges[i];
            }
            double atr = sum / timePeriod;

            ATR.Add(atr);
            for (int i = 0; i < (trueRanges.Count - timePeriod); i++)
            {
                //sum -= trueRanges[i];
                //sum += trueRanges[i + (int)timePeriod];
                //atr = (sum / timePeriod);
                //ATR.Add(atr);
                atr = (atr * (timePeriod - 1) + trueRanges[i + (int)timePeriod]) / timePeriod;
                ATR.Add(atr);
            }
            if (indicator_name == ".ATR")
            {
                foreach (var item in ATR)
                {
                    Console.WriteLine(item);
                }
            }
            return ATR;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------KELTNER CHANNEL---------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------
        static List<Keltner_Values> CalculateKC(string[] lines, double timePeriod, string indicator_name, string mat, int s)
        {
            List<double> close = new List<double>();
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] columns = line.Split(',');
                double cl = double.Parse(columns[4]);
                close.Add(cl);
            }
            List<double> EMA = CalculateEMA(close, timePeriod, indicator_name);
            List<double> SMA = CalculateSMA(close, timePeriod, indicator_name);
            List<double> ATR = CalculateATR(lines, timePeriod, indicator_name);
            List<Keltner_Values> KC = new List<Keltner_Values>();
            KC.Add(new Keltner_Values { MIDDLE = EMA[0] });
            if (mat == "EMA")
            {
                for (int i = 0; i < ATR.Count; i++)
                {
                    double upperBandEMA = EMA[i + 1] + ATR[i] * s;
                    double lowerBandEMA = EMA[i + 1] - ATR[i] * s;
                    KC.Add(new Keltner_Values { UPPER = upperBandEMA, MIDDLE = EMA[i + 1], LOWER = lowerBandEMA });
                }
            }
            else if (mat == "SMA")
            {
                for (int i = 0; i < ATR.Count; i++)
                {
                    double upperBandSMA = SMA[i + 1] + ATR[i] * s;
                    double lowerBandSMA = SMA[i + 1] - ATR[i] * s;
                    KC.Add(new Keltner_Values { UPPER = upperBandSMA, MIDDLE = SMA[i + 1], LOWER = lowerBandSMA });
                }
            }
            foreach (var item in KC)
            {
                Console.WriteLine($"U:{item.UPPER}, M:{item.MIDDLE}, L:{item.LOWER}");
            }
            return KC;
        }
        //---------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------SUPER TREND---------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------------------------
        static List<double> CalculateST(string[] lines, double timePeriod, string indicator_name, double Multiplier)
        {
            List<double> supertrendValues = new List<double>();
            List<double> atrValues = CalculateATR(lines, timePeriod, indicator_name);
            List<double> BasicUpperBand = new List<double>();
            List<double> BasicLowerBand = new List<double>();
            for (int i = 0; i < atrValues.Count; i++)
            {
                string line = lines[i + (int)timePeriod];
                string[] columns = line.Split(',');
                double Value1 = double.Parse(columns[2]);
                double Value2 = double.Parse(columns[3]);
                double basicUpperBand = (Value1 + Value2) / 2 + atrValues[i] * Multiplier;
                double basicLowerBand = (Value1 + Value2) / 2 - atrValues[i] * Multiplier;
                BasicUpperBand.Add(basicUpperBand);
                BasicLowerBand.Add(basicLowerBand);
            }
            List<double> UpperBand = new List<double>();
            List<double> LowerBand = new List<double>();
            UpperBand.Add(BasicUpperBand[0]);
            LowerBand.Add(BasicLowerBand[0]);
            double upperband = BasicUpperBand[0];
            double lowerband = BasicLowerBand[0];

            double supertrend = lowerband;
            supertrendValues.Add(supertrend);
            double trendDirection = -1;


            for (int i = 1; i < BasicUpperBand.Count; i++)
            {
                string line = lines[i + (int)timePeriod - 1];
                string[] columns = line.Split(',');
                double Value3 = double.Parse(columns[4]);

                upperband = ((BasicUpperBand[i] < UpperBand[i - 1]) || (Value3 > UpperBand[i - 1])) ? BasicUpperBand[i] : UpperBand[i - 1];
                UpperBand.Add(upperband);
                lowerband = ((BasicLowerBand[i] > LowerBand[i - 1]) || (Value3 < LowerBand[i - 1])) ? BasicLowerBand[i] : LowerBand[i - 1];
                LowerBand.Add(lowerband);
                line = lines[i + (int)timePeriod ];
                columns = line.Split(',');
                Value3 = double.Parse(columns[4]);
                if (supertrendValues[i - 1] == UpperBand[i - 1])
                {
                    trendDirection = (Value3 > upperband) ? 1 : -1;
                }
                else
                {
                    trendDirection = (Value3 < lowerband) ? -1 : 1;
                }
                supertrend = (trendDirection == 1) ? lowerband : upperband;
                supertrendValues.Add(supertrend);
            }
            foreach(var item in supertrendValues)
            {
                Console.WriteLine(item);
            }
            return supertrendValues;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------DOCHAIN WEIDTH---------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------
        static List<double> CalculateDCW(string[] lines, string indicator_name, double HighPeriod, double LowPeriod)
        {
            List<HLC_Values> myList = ParseCSV(lines);
            List<double> upperlist = new List<double>();
            List<double> lowerlist = new List<double>();
            List<double> dcw = new List<double>();
            string line = lines[1];
            string[] columns = line.Split(',');
            double high = double.Parse(columns[2]);

            int sum = 1;
            int k = 1;
            while (sum < HighPeriod)
            {
                line = lines[k + 1];
                columns = line.Split(',');
                double high1 = double.Parse(columns[2]);
                upperlist.Add(high);
                high = Math.Max(high, high1);
                k++;
                sum++;
            }

            for (int i = sum; i <= myList.Count; i++)
            {
                line = lines[i];
                columns = line.Split(',');
                high = double.Parse(columns[2]);
                for (int j = i - 1; i - j <= HighPeriod; j--)
                {
                    line = lines[j + 1];
                    columns = line.Split(',');
                    double high1 = double.Parse(columns[2]);
                    high = Math.Max(high, high1);
                }
                upperlist.Add(high);

            }
            line = lines[1];
            columns = line.Split(',');
            double low = double.Parse(columns[3]);
            double sum1 = 1;
            int k1 = 1;
            while (sum1 < LowPeriod)
            {
                line = lines[k1 + 1];
                columns = line.Split(',');
                double low1 = double.Parse(columns[3]);
                lowerlist.Add(low);
                low = Math.Min(low, low1);
                k1++;
                sum1++;
            }

            for (int i = sum; i <= myList.Count; i++)
            {
                line = lines[i];
                columns = line.Split(',');
                low = double.Parse(columns[3]);
                for (int j = i - 1; i - j <= LowPeriod; j--)
                {
                    line = lines[j + 1];
                    columns = line.Split(',');
                    double low1 = double.Parse(columns[3]);
                    low = Math.Min(low, low1);
                }
                lowerlist.Add(low);

            }
            for (int i = 0; i < lowerlist.Count; i++)
            {
                dcw.Add(upperlist[i] - lowerlist[i]);
            }
            foreach (var item in dcw)
            {
                Console.WriteLine(item);
            }
            return dcw;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------MACD---------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------        
        static List<MACD> CalculateMACD(List<double> close, double fast_ma_period, double slow_ma_period, double signal_ma_period, string indicator_name)
        {
         
            List<double> Fast_EMA = CalculateEMA(close, fast_ma_period, indicator_name);
            List<double> Slow_EMA = CalculateEMA(close, slow_ma_period, indicator_name);
            List<double> MACDLine = new List<double>();
            for (int i = 0; i < Slow_EMA.Count; i++)
            {
                MACDLine.Add(Fast_EMA[i + Fast_EMA.Count - Slow_EMA.Count] - Slow_EMA[i]);
            }
            List<double> sma = new List<double>();
            double sum = 0;
            for (int i = 0; i < signal_ma_period; i++)
            {
                sum += MACDLine[i];

            }
            sma.Add((sum / signal_ma_period));
            for (int i = 0; i < (MACDLine.Count - signal_ma_period); i++)
            {
                sum -= MACDLine[i];
                sum += MACDLine[i + (int)signal_ma_period];
                sma.Add((sum / signal_ma_period));
            }
            List<double> eMA = new List<double>();
            double k = 2.0 / (signal_ma_period + 1);
            double ema = sma[0];
            eMA.Add(ema);
            for (int i = 1; i < sma.Count; i++)
            {
                ema = (MACDLine[i + (int)signal_ma_period - 1] - ema) * k + ema;
                eMA.Add(ema);
            }
            List<MACD> macd = new List<MACD>(); int j = 0;
            for (int i = 0; i < MACDLine.Count; i++)
            {
                if (i < signal_ma_period - 1)
                {
                    macd.Add(new MACD { MACDLine = MACDLine[i] });
                }
                else
                {
                    macd.Add(new MACD { MACDLine = MACDLine[i], SignalLine = eMA[j] });
                    j++;
                }
            }
            foreach (var item in macd)
            {

                Console.WriteLine($"{item.MACDLine},{item.SignalLine}");
            }
            return macd;
        }
        //----------------------------------------------------------------------------------------
        static List<double> CalculateRSI(string[] lines, double timePeriod, string indicator_name)
        {
            List<HLC_Values> myList = ParseCSV(lines);
            List<double> RSI = new List<double>();

            List<double> changes = new List<double>();
            List<double> gains = new List<double>();
            List<double> losses = new List<double>();
            for (int i = 1; i < myList.Count; i++)
            {
                string line = lines[i + 1];
                string[] columns = line.Split(',');
                double thirdcolumn1 = double.Parse(columns[4]);
                line = lines[i];
                columns = line.Split(',');
                double thirdcolumn2 = double.Parse(columns[4]);
                changes.Add(thirdcolumn1 - thirdcolumn2);

            }

            // calculate average gains and losses


            for (int i = 0; i < changes.Count; i++)
            {
                if (changes[i] > 0)
                {
                    gains.Add(changes[i]);
                    losses.Add(0);
                }
                else
                {
                    gains.Add(0);
                    losses.Add(Math.Abs(changes[i]));
                }
            }
            double sumofgains = 0;
            double sumoflosses = 0;
            for (int i = 0; i < timePeriod; i++)
            {
                sumofgains += gains[i];
                sumoflosses += losses[i];
            }
            double rs;
            rs = (sumofgains / sumoflosses);
            double rsi;
            rsi = 100 - (100 / (1 + rs));
            RSI.Add(rsi);


            for (int i = 0; i < (gains.Count - timePeriod); i++)
            {
                sumofgains -= gains[i];
                sumofgains += gains[i + (int)timePeriod];

                sumoflosses -= losses[i];
                sumoflosses += losses[i + (int)timePeriod];

                rs = (sumofgains / sumoflosses);
                rsi = 100 - (100 / (1 + rs));
                RSI.Add(rsi);
            }
            if (indicator_name == ".RSI")
            {
                foreach (var item in RSI)
                {
                    Console.WriteLine(item);
                }
            }
            return RSI;
        }
    }
}
