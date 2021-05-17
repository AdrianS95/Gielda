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

namespace ATH_Giełda_Adrian_Smaza.Class.Analysis
{
    //Klasa opracowania przez: Kamil Kubica. Została dostosowana do potrzeb tej aplikacji.
    class OshIndicaror : IIndicatorDataCreator
    {
        private readonly MainWindow _mainWindow;
        private AppFullDataAnalaizer _OshIndicaror;
        private AnalyzingData analyzingData;

        public OshIndicaror(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }
        public OshIndicaror(AppFullDataAnalaizer appFullDataAnalaizer)
        {
            this._OshIndicaror = appFullDataAnalaizer;
        }

        public OshIndicaror(AnalyzingData analyzingData)
        {
            this.analyzingData = analyzingData;
        }

        public ReadOnlyCollection<IndicatorData> GetSignal(ReadOnlyCollection<CompanyDataItem> companyDatas, int dayPeriodLength)
        {
            var tmpData = new List<double>();
            var indicatorDatas = new List<IndicatorData>();
            for (int k = dayPeriodLength; k < companyDatas.Count; k++)
            {
                for (int i = k - dayPeriodLength; i < k; ++i)
                {
                    tmpData.Add(companyDatas[i].Close);
                }
                var K = (tmpData.Last() - tmpData.Min()) / (tmpData.Max() - tmpData.Min());
                var signal = 0;
                if (K >= 0.80)
                {
                    signal = 1;
                }
                else if (K <= 0.20)
                {
                    signal = -1;
                }
                else
                {
                    signal = 0;
                }
                indicatorDatas.Add(new IndicatorData(companyDatas[k].Date, signal));
                tmpData.Clear();
            }
            indicatorDatas.Reverse();
            var result = new ReadOnlyCollection<IndicatorData>(indicatorDatas.Take(dayPeriodLength).ToArray());

            return result;
        }

        public string GetName()
        {
            return "StochasticOscilator";
        }

        public ReadOnlyCollection<IndicatorData> Calculate(ReadOnlyCollection<CompanyDataItem> companyDatas, int dayPeriodLength)
        {
            var tmpData = new List<double>();
            var indicatorDatas = new List<IndicatorData>();
            for (int k = dayPeriodLength; k < companyDatas.Count; k++)
            {
                for (int i = k - dayPeriodLength; i < k; ++i)
                {
                    tmpData.Add(companyDatas[i].Close);
                }
                var K = (tmpData.Last() - tmpData.Min()) / (tmpData.Max() - tmpData.Min());

                indicatorDatas.Add(new IndicatorData(companyDatas[k].Date, (float)(K)));
                tmpData.Clear();
            }

            indicatorDatas.Reverse();
            var result = new ReadOnlyCollection<IndicatorData>(indicatorDatas.Take(dayPeriodLength).ToArray());
            var oshValue = result.Select(x => x.Value).ToArray();
            var oshDate = result.Select(x => x.Date).ToArray();

            GenarateChart(oshValue, oshDate, "OSH", dayPeriodLength);

            return result;

        }

        class Signal
        {
            public Signal(string date, SignalReaction reaction)
            {
                Date = date;
                Reaction = reaction;
            }

            public string Date { get; }
            public SignalReaction Reaction { get; }
        }


        public enum SignalReaction { Sell = -1, NoReaction = 0, Buy = 1 }

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