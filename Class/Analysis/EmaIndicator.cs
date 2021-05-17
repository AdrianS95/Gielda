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

namespace ATH_Giełda_Adrian_Smaza.Class.Analysis
{
    class EmaIndicaror : IIndicatorDataCreator
    {
        private readonly MainWindow _mainWindow;
        private AppFullDataAnalaizer _OshIndicaror;
        private AnalyzingData analyzingData;

        public EmaIndicaror(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }
        public EmaIndicaror(AppFullDataAnalaizer appFullDataAnalaizer)
        {
            this._OshIndicaror = appFullDataAnalaizer;
        }



        private const float Smooth = 2.0f;
        public ReadOnlyCollection<IndicatorData> Calculate(ReadOnlyCollection<CompanyDataItem> companyDatas, int dayPeriodLength)
        {
            var outDataSize = companyDatas.Count - dayPeriodLength;
            if (outDataSize < 0)
                return new ReadOnlyCollection<IndicatorData>(new List<IndicatorData>(0));

            var outData = new List<IndicatorData>(outDataSize);

            var spanData = companyDatas.ToArray();
            var firstEma = (float)(Avg(spanData.Take(dayPeriodLength)));

            outData.Add(new IndicatorData(spanData[dayPeriodLength - 1].Date, firstEma));

            var lastEma = firstEma;
            var mulValue = (Smooth / (1 + dayPeriodLength));

            for (int i = (dayPeriodLength + 1); i < companyDatas.Count; i++)
            {
                var currentValue = companyDatas[i].Close;
                var currentEma = (currentValue * mulValue) + (lastEma * (1 - mulValue));
                outData.Add(new IndicatorData(
                        spanData[i].Date,
                        currentEma
                    ));
                lastEma = currentEma;
            }

            outData.Reverse();
            var result = new ReadOnlyCollection<IndicatorData>(outData.Take(dayPeriodLength).ToArray());
            var oshValue = result.Select(x => x.Value).ToArray();
            var oshDate = result.Select(x => x.Date).ToArray();

            GenarateChart(oshValue, oshDate, "EMA", dayPeriodLength);

            return new ReadOnlyCollection<IndicatorData>(outData.Take(dayPeriodLength).ToArray());
        }


        public ReadOnlyCollection<IndicatorData> GetSignal(ReadOnlyCollection<CompanyDataItem> companyDatas, int dayPeriodLength)
        {
            var outDataSize = companyDatas.Count - dayPeriodLength;

            if (outDataSize < 0)
                return new ReadOnlyCollection<IndicatorData>(new List<IndicatorData>(0));

            var outData = new List<IndicatorData>(outDataSize);

            var spanData = companyDatas.ToArray();
            var firstEma = (float)(Avg(spanData.Take(dayPeriodLength)));

            outData.Add(new IndicatorData(spanData[dayPeriodLength - 1].Date, firstEma));

            var lastEma = firstEma;
            var mulValue = (Smooth / (1 + dayPeriodLength));

            for (int i = (dayPeriodLength + 1); i < companyDatas.Count; i++)
            {
                var currentValue = companyDatas[i].Close;
                var currentEma = (currentValue * mulValue) + (lastEma * (1 - mulValue));
                outData.Add(new IndicatorData(
                        spanData[i].Date,
                        currentEma
                    ));
                lastEma = currentEma;
            }
            var outSignal = new List<IndicatorData>(dayPeriodLength);

            for (int i = outData.Count- dayPeriodLength; i < outData.Count; i++)
            {
                float EMA5Day = (outData[i].Value - outData[i-5].Value)/outData[i - 5].Value;
                outSignal.Add(new IndicatorData(outData[i].Date, firstEma));
            }
            return new ReadOnlyCollection<IndicatorData>(outSignal);
        }

        private object Avg(IEnumerable<CompanyDataItem> enumerable)
        {
            var sum = 0.0f;
            foreach (var item in enumerable)
                sum += item.Close;
            return sum / enumerable.Count();
        }

        public void GenarateChart(float[] rsi, string[] data, string Name, int iter)
        {
            _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                _mainWindow.ChartNameLabel.Content = "Wykres " + Name;

                if (iter > 180)
                {
                    iter = 180;
                }

                _mainWindow.Chart.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = Name,
                        Values = new ChartValues<float> (rsi.Take(iter).Reverse()),
                        PointGeometry = null,

                    },
                };
                var format = "yyyyMMdd";
                var castedData = data.Select(z => DateTime.ParseExact(z, format, CultureInfo.InvariantCulture)).Take(iter).Reverse();

                _mainWindow.Chart.AxisX[0].Labels = castedData.Select(z => z.ToShortDateString()).ToArray();
            }));

        }
    }
}
