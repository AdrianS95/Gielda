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
    internal enum TradeMode
    {
        Buy,
        Sell,
        Hold
    }

    internal enum DateOrder
    {
        Ascending,
        Descending,
        NotParsed
    }
    //Klasa opracowania przez: Damian Tlałka. Została dostosowana do potrzeb tej aplikacji.
    class RsiIndicator : IIndicatorDataCreator
    {
        private readonly MainWindow _mainWindow;
        private AppFullDataAnalaizer _appFullDataAnalaizer;
        private AnalyzingData analyzingData;

        public RsiIndicator(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public RsiIndicator(AppFullDataAnalaizer appFullDataAnalaizer)
        {
            this._appFullDataAnalaizer = appFullDataAnalaizer;
        }

        public RsiIndicator(AnalyzingData analyzingData)
        {
            this.analyzingData = analyzingData;
        }
 
        public ReadOnlyCollection<IndicatorData> Calculate(ReadOnlyCollection<CompanyDataItem> companyDatas,
           int dayPeriodLength)
        {
            if (companyDatas.Count < dayPeriodLength)
                return null;

            var processedDatasCount = 0;
            var nextData = 0;

            var datesOrder = CheckDatesOrder(companyDatas);
            if (datesOrder == DateOrder.Ascending)
                companyDatas = Array.AsReadOnly(companyDatas.OrderByDescending(x => x.Date).ToArray());

            var indicators = new List<IndicatorData>();
            var rs = CalculateRs(companyDatas);

            while (processedDatasCount <= companyDatas.Count - dayPeriodLength)
            {
                var rsi = 100 - 100 / (1 + rs[nextData]);
                var firstDate = companyDatas[nextData].Date;
                indicators.Add(new IndicatorData(firstDate, rsi));

                processedDatasCount++;
                nextData++;
            }
            var result = new ReadOnlyCollection<IndicatorData>(indicators.Take(dayPeriodLength).ToArray());
                var rsiValue = result.Select(x=>x.Value).ToArray();
                var rsiDate = result.Select(x => x.Date).ToArray();

            GenarateChart(rsiValue, rsiDate, "RSI", dayPeriodLength);
            return result;
        }

        private List<float> CalculateRs(ReadOnlyCollection<CompanyDataItem> stocks)
        {
            float avgGain = 0;
            float avgLoss = 0;

            var differences = CalculateDifferences(stocks);
            var rs = new List<float>();

            int offset = 0;
            for (int i = 0; i < differences.Length; i++)
            {
                if (differences[i] > 0)
                    avgGain += differences[i];
                else
                    avgLoss -= differences[i];

                if (i != 0 && i % (12 + offset) == 0)
                {
                    rs.Add(avgGain / avgLoss);
                    i = ++offset;
                }

                else
                    continue;

                avgGain = 0;
                avgLoss = 0;
            }

            return rs;
        }

        private float[] CalculateDifferences(ReadOnlyCollection<CompanyDataItem> stocks)
        {
            var result = new float[stocks.Count];
            for (int i = 0; i < stocks.Count - 1; i++)
            {
                result[i] = stocks[i].Close - stocks[i + 1].Close;
            }

            return result;
        }

        public Dictionary<string, TradeMode> GetSignals(ReadOnlyCollection<IndicatorData> indicators)
        {
            var signals = new Dictionary<string, TradeMode>();
            foreach (var indicator in indicators)
                if (indicator.Value <= 30)
                    signals.Add(indicator.Date, TradeMode.Buy);
                else if (indicator.Value >= 70)
                    signals.Add(indicator.Date, TradeMode.Sell);
                else
                    signals.Add(indicator.Date, TradeMode.Hold);

            return signals;
        }

        /// <summary>
        ///     Checks whether dates are sorted ascending or descending
        /// </summary>
        /// <param name="companyDatas"></param>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        private DateOrder CheckDatesOrder(ReadOnlyCollection<CompanyDataItem> companyDatas,
            string dataFormat = "yyyyMMdd")
        {
            if (!DateTime.TryParseExact(companyDatas[0].Date, dataFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var firstDate)) return DateOrder.NotParsed;

            var lastDate = DateTime.ParseExact(companyDatas.Last().Date, dataFormat, CultureInfo.InvariantCulture);

            return firstDate < lastDate ? DateOrder.Ascending : DateOrder.Descending;
        }

        public ReadOnlyCollection<IndicatorData> GetSignal(ReadOnlyCollection<CompanyDataItem> companyDatas, int dayPeriodLength)
        {
            if (companyDatas.Count < dayPeriodLength)
                return null;

            var processedDatasCount = 0;
            var nextData = 0;

            var datesOrder = CheckDatesOrder(companyDatas);
            if (datesOrder == DateOrder.Ascending)
                companyDatas = Array.AsReadOnly(companyDatas.OrderByDescending(x => x.Date).ToArray());

            var indicators = new List<IndicatorData>();
            var rs = CalculateRs(companyDatas);

            while (processedDatasCount <= companyDatas.Count - dayPeriodLength)
            {
                var rsi = 100 - 100 / (1 + rs[nextData]);
                var firstDate = companyDatas[nextData].Date;
                indicators.Add(new IndicatorData(firstDate, rsi));

                processedDatasCount++;
                nextData++;
            }

            var result = new ReadOnlyCollection<IndicatorData>(indicators.Take(dayPeriodLength).ToArray());
            return result;
        }

        public void GenarateChart(float[] rsi, string[] data, string rsiName, int iter)
        {
            _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                _mainWindow.ChartNameLabel.Content = "Wykres RSI";

                if (iter > 180)
                {
                    iter = 180;
                }

                _mainWindow.Chart.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = rsiName,
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

