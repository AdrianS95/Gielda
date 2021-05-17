using ATH_Giełda_Adrian_Smaza.Class;
using ATH_Giełda_Adrian_Smaza.Class.Analysis;
using ATH_Giełda_Adrian_Smaza.Class.AppFunction;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Media;
using static ATH_Giełda_Adrian_Smaza.MainWindow;

namespace ATH_Giełda_Adrian_Smaza.Class.AppFunction
{
    class AppFullDataAnalaizer
    {
        private readonly MainWindow _mainWindow;
        private readonly MovingAverageConvergenceDivergence _MACD;
        private readonly LoadAppData _loadAppData;
        private readonly RsiIndicator _RSI;
        private readonly OshIndicaror _OSH;
        private readonly EmaIndicaror _EMA;
        private readonly AppLogic _AppLogic;

        public object StopWatch { get; private set; }

        public AppFullDataAnalaizer(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _loadAppData = new LoadAppData(this);
            _MACD = new MovingAverageConvergenceDivergence(this);
            _RSI = new RsiIndicator(this);
            _OSH = new OshIndicaror(this);
            _EMA = new EmaIndicaror(this);
            _AppLogic = new AppLogic(_mainWindow,this);
        }

        public void DisplayData(string tekst)
        {
            _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                _mainWindow.GameOutputDataTextBlock.Text += tekst + "\n";
            }));
        }


        public void AnalizSingData(string file, int iter)// Funkcja po obciążania aplikacji
        {
            string[] myFile = File.ReadAllLines($@"bossa/{file}");
            var DataGielda = _loadAppData.LoadStructureGameReadOnlyColection2(myFile);

            var macd = _MACD.GetSignal(DataGielda, iter);
            var rsi = _RSI.GetSignal(DataGielda, iter);
            var osh = _OSH.GetSignal(DataGielda, iter);
            var ema = _EMA.GetSignal(DataGielda, iter);


            var LastRSI = macd.Take(9).FirstOrDefault(z => z.Value == 1);
            var LastOSH = osh.Take(9).FirstOrDefault(z => z.Value == 1);

            if (LastOSH?.Value == 1 && LastRSI?.Value == 1 && rsi.First().Value > 70 && ema.First().Value > 6)
            {
                DisplayData(file);
            }

        }
        public async Task AllFileTestAllCore() // Obciążenie aplikacji wszystkie wątki
        {
            _mainWindow.GameOutputDataTextBlock.Text = "Rozważ zakup:\n";
            var iter = Convert.ToInt32(_mainWindow.GamePrecisionComboBox.Text);
            var st = Stopwatch.StartNew();

            _AppLogic.AllButtonLock();

            await Task.Run(() =>
            {
                string[] mstFiles = Directory.GetFiles("bossa", "*.mst").Select(Path.GetFileName).ToArray();

                Parallel.For(0, mstFiles.Length, isx =>
                {
                    AnalizSingData(mstFiles[isx], iter);
                });
            });

            _AppLogic.AllButtonUnLock();
            _mainWindow.GameOutputControlTextBlock.Text += "Czas przetwarzania wszystkich plików " + st.ElapsedMilliseconds + "ms\n";
        }

        public async void AllFileTestOneCore() // Obciążenie aplikacji 1 wątek
        {
            _AppLogic.AllButtonLock();
            string[] mstFiles = Directory.GetFiles("bossa", "*.mst").Select(Path.GetFileName).ToArray(); ;
            int start, stop;
            start = Environment.TickCount & Int32.MaxValue;
            _mainWindow.GameOutputDataTextBlock.Text = "Rozważ zakup:\n";
            int iter = Convert.ToInt32(_mainWindow.GamePrecisionComboBox.Text);
            for (int i = 0; i < mstFiles.Length; i++)
            {
                await Task.Run(() =>
                {
                    AnalizSingData(mstFiles[i], iter);
                });
            }

            stop = Environment.TickCount & Int32.MaxValue;
            int time = stop - start;

            _mainWindow.GameOutputControlTextBlock.Text += "Czas przetwarzania wszystkich plików " + time + "ms\n";
            _AppLogic.AllButtonUnLock();
        }
    }
}
