using ATH_Giełda_Adrian_Smaza.Class;
using ATH_Giełda_Adrian_Smaza.Class.AppFunction;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Media;
using static ATH_Giełda_Adrian_Smaza.MainWindow;

using SeriesCollection = LiveCharts.SeriesCollection;

namespace ATH_Giełda_Adrian_Smaza
{
    class MovingAverageConvergenceDivergence : IIndicatorDataCreator
    {
        private readonly MainWindow _mainWindow;
        private AppFullDataAnalaizer _appFullDataAnalaizer;
        private AnalyzingData analyzingData;

        public MovingAverageConvergenceDivergence(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public MovingAverageConvergenceDivergence(AppFullDataAnalaizer appFullDataAnalaizer)
        {
            this._appFullDataAnalaizer = appFullDataAnalaizer;
        }

        public MovingAverageConvergenceDivergence(AnalyzingData analyzingData)
        {
            this.analyzingData = analyzingData;
        }

        public ReadOnlyCollection<IndicatorData> Calculate(ReadOnlyCollection<CompanyDataItem> companyDatas, int dayPeriodLength)
        {
            int dateCount = companyDatas.Count - dayPeriodLength;
            var indicatorDatas = new List<IndicatorData>();

            string[] data = new string[companyDatas.Count];
            float[] close = new float[companyDatas.Count];

            int iterator = 0;
            foreach (var item in companyDatas)
            {
                data[iterator] = item.Date;
                close[iterator++] = item.Close;
            }

            float[] EMA12 = CalculateEMA(close, 12);
            float[] EMA26 = CalculateEMA(close, 26);
            float[] MACD = CalculateMACD(EMA12, EMA26);
            float[] signal = CalculateSignal(MACD, 9);

            var signalDayPeriodLength = EMA12.Reverse().Take(dayPeriodLength);

            var a = companyDatas.Reverse().Take(90).Reverse();

            for (int i = companyDatas.Count - dayPeriodLength; i < companyDatas.Count; i++)
            {
                string endDate = data[i];
                float value = signal[i];
                indicatorDatas.Add(new IndicatorData(endDate, value));
            }
            GenarateChart(close, "Close", EMA12, "EMA12", EMA26, "EMA26", dayPeriodLength, data);

            indicatorDatas.Reverse();
            return new ReadOnlyCollection<IndicatorData>(indicatorDatas);
        }
        public float[] GetSignal(float[] values)
        {

            float[] signal = new float[values.Length];
            signal[0] = 0;

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i - 1] < 0 && values[i] >= 0)
                {
                    signal[i] = 1;

                }
                else if (values[i - 1] > 0 && values[i] <= 0)
                {
                    signal[i] = -1;
                }
                else
                {
                    signal[i] = 0;
                }
            }

            return signal;
        }
        public float[] CalculateEMA(float[] close, int day)
        {
            float[] EMA = new float[close.Length];
            for (int i = 26; i < close.Length; i++)
            {
                EMA[i] = OnesValieCalculator(i, close, day);
            }
            return EMA;
        }
        public float[] CalculateMACD(float[] EMAx, float[] EMAy)
        {
            float[] MACD = new float[EMAx.Length];
            for (int i = 26; i < EMAx.Length; i++)
            {
                MACD[i] = EMAx[i] - EMAy[i];
            }
            return MACD;
        }
        public float OnesValieCalculator(int i, float[] tab, int day)
        {
            float value = 0;
            for (int j = i - day; j < i; j++)
            {
                value += tab[j];
            }
            return value / day;
        }
        public float[] CalculateSignal(float[] MACD, int day)
        {
            float[] signal = new float[MACD.Length];

            for (int i = 26; i < MACD.Length; i++)
            {
                signal[i] = OnesValieCalculator(i, MACD, day);
            }
            return signal;
        }
        public ReadOnlyCollection<IndicatorData> GetSignal(ReadOnlyCollection<CompanyDataItem> companyDatas, int dayPeriodLength)
        {

            int dateCount = companyDatas.Count - dayPeriodLength;
            var indicatorDatas = new List<IndicatorData>();

            string[] data = new string[companyDatas.Count];
            float[] close = new float[companyDatas.Count];

            int iterator = 0;
            foreach (var item in companyDatas)
            {
                data[iterator] = item.Date;
                close[iterator++] = item.Close;
            }
            float[] EMA12 = CalculateEMA(close, 12);
            float[] EMA26 = CalculateEMA(close, 26);
            float[] MACD = CalculateMACD(EMA12, EMA26);
            float[] signal = CalculateSignal(MACD, 9);
            float[] signalTrueAndFalse = GetSignal(signal);

            var signalDayPeriodLength = signalTrueAndFalse.Reverse().Take(dayPeriodLength);

            for (int i = companyDatas.Count - dayPeriodLength; i < companyDatas.Count; i++)
            {
                string endDate = data[i];
                float value = signalTrueAndFalse[i];
                indicatorDatas.Add(new IndicatorData(endDate, value));
            }

            return new ReadOnlyCollection<IndicatorData>(indicatorDatas);
        }

        public void GenarateChart(float[] close, string closeName, float[] line1, string line1Name, float[] line2, string line2Name, int iter, string[] data)
        {
            _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                _mainWindow.ChartNameLabel.Content = "Wykres MACD";

                if (iter > 180)
                {
                    iter = 180;
                }

                _mainWindow.Chart.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = line1Name,
                        Values = new ChartValues<float> (line1.Skip(line1.Length-iter).Take(iter)),
                        PointGeometry = null
                    },
                    new LineSeries
                    {
                        Title = line2Name,
                        Values = new ChartValues<float> (line2.Skip(line1.Length-iter).Take(iter)),
                        PointGeometry = null
                    },
                    new LineSeries
                    {
                        Title = closeName,
                        Values = new ChartValues<float> (close.Skip(line1.Length-iter).Take(iter)),
                        PointGeometry = null,
                    },
                };
                var format = "yyyyMMdd";
                var castedData = data.Select(z => DateTime.ParseExact(z, format, CultureInfo.InvariantCulture)).Skip(line1.Length - iter);
                _mainWindow.Chart.AxisX[0].Labels = castedData.Select(z => z.ToShortDateString()).ToArray();
            }));

        }
    }
}



