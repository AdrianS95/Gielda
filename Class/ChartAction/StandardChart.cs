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

namespace ATH_Giełda_Adrian_Smaza.Class.ChartAction
{
    class StandardChart
    {
        private readonly MainWindow _mainWindow;

        public StandardChart(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void Chart(ReadOnlyCollection<CompanyDataItem> companyDatas, int dayPeriodLength)
        {
            var close = companyDatas.Select(x=>x.Close).Reverse().Take(dayPeriodLength).ToArray();
            var date = companyDatas.Select(x => x.Date).Reverse().Take(dayPeriodLength).ToArray();


            _mainWindow.Dispatcher.BeginInvoke(new Action(async() =>
            {
                await Task.Delay(100);
                _mainWindow.Chart.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Close",
                        Values = new ChartValues <float> (close.Reverse()),
                        PointGeometry = null
                    },
                };
                var format = "yyyyMMdd";
                var castedData = date.Select(z => DateTime.ParseExact(z, format, CultureInfo.InvariantCulture));
                _mainWindow.Chart.AxisX[0].Labels = castedData.Select(z => z.ToShortDateString()).Reverse().ToArray();
            }));

        }

        public void Note(ReadOnlyCollection<CompanyDataItem> companyDatas)
        {
            _mainWindow.GameOutputDataTextBlock.Text = "";
            var close = companyDatas.Select(x => x.Close).Reverse().Take(1).ToArray();
            var open = companyDatas.Select(x => x.Open).Reverse().Take(1).ToArray();
            var high = companyDatas.Select(x => x.High).Reverse().Take(1).ToArray();
            var low = companyDatas.Select(x => x.Low).Reverse().Take(1).ToArray();
            var date = companyDatas.Select(x => x.Date).Reverse().Take(1).ToArray();
            var vol = companyDatas.Select(x => x.Vol).Reverse().Take(1).ToArray();
            _mainWindow.GameOutputDataTextBlock.Text += "Data : " + date[0] + "\n";
            _mainWindow.GameOutputDataTextBlock.Text += "Vol  : " + vol[0] + "\n";
            _mainWindow.GameOutputDataTextBlock.Text += "Low  : " + low[0] + "\n";
            _mainWindow.GameOutputDataTextBlock.Text += "High : " + high[0] + "\n";
            _mainWindow.GameOutputDataTextBlock.Text += "Open : " + open[0] + "\n";
            _mainWindow.GameOutputDataTextBlock.Text += "Close: " + close[0] + "\n";
        }
    }
 }
