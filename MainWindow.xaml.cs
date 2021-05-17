using ATH_Giełda_Adrian_Smaza.Class;
using ATH_Giełda_Adrian_Smaza.Class.Analysis;
using ATH_Giełda_Adrian_Smaza.Class.AppFunction;
using ATH_Giełda_Adrian_Smaza.Class.ChartAction;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ATH_Giełda_Adrian_Smaza.MainWindow;

namespace ATH_Giełda_Adrian_Smaza
{

    public partial class MainWindow : Window
    {
        private readonly LoadAppData _loadAppData;
        private readonly DiagnosticService _DiagnosticService;
        private readonly FileDowndAndMenager _FileDowndAndMenager;
        private readonly AppFullDataAnalaizer _AppFullDataAnalaizer;

        private readonly MovingAverageConvergenceDivergence _MACD;
        private readonly RsiIndicator _RSI;
        private readonly OshIndicaror _OSH;
        private readonly EmaIndicaror _EMA;

        private readonly StandardChart _SChart;
        private AppLogic _AppLogic;



        //string[] myFile = System.IO.File.ReadAllLines(@"WIG-POLAND.mst");

        public string[] Labels { get; internal set; }

        public MainWindow()
        {
            InitializeComponent();
            InitChartView();
            _DiagnosticService = new DiagnosticService(this);
            RunAsyncDiagnosticData();
            _loadAppData = new LoadAppData(this);
            _FileDowndAndMenager = new FileDowndAndMenager(this);
            _MACD = new MovingAverageConvergenceDivergence(this);
            _AppFullDataAnalaizer = new AppFullDataAnalaizer(this);
            _SChart = new StandardChart(this);
            _RSI = new RsiIndicator(this);
            _OSH = new OshIndicaror(this);
            _EMA = new EmaIndicaror(this);
            InitAppComboBoxDataAsync();
        }


        public void InitChartView()
        {
            Chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Witaj",
                    Values = new ChartValues<double> { 1, 2, 3, 4 ,5 },
                    PointGeometry = null
                },
                new LineSeries
                {
                    Title = "Giełdo",
                    Values = new ChartValues<double> { 5, 4, 3, 2 ,1 },
                    PointGeometry = null
                },

            };
        }



        public void InitAppComboBoxDataAsync() // inicjalizacja danych. Pierwsze uruchom,ienie troche zajmuje. pobierane są wtedy dane.
        {

            try
            {
                string[] mstFiles = Directory.GetFiles("bossa", "*.mst").Select(Path.GetFileName).ToArray();
                if (mstFiles.Length == 0)
                {

                    _FileDowndAndMenager.RunAsyncDataDownd();

                }

                for (int i = 0; i < mstFiles.Length; i++)
                {
                    GameComboBox.Items.Add(mstFiles[i]);
                }

                GameComboBox.SelectedIndex = 10;
            }
            catch (Exception)
            {

                _FileDowndAndMenager.RunAsyncDataDownd();

            }





            //Opcja X
            OcjaX.Items.Add("Polska");
            OcjaX.Items.Add("USA");
            OcjaX.Items.Add("Niemcy");
            OcjaX.SelectedIndex = 0;

            //rodzaje analizy

            TypeAnaliz.Items.Add("Tylko index");
            TypeAnaliz.Items.Add("MACD");
            TypeAnaliz.Items.Add("RSI");
            TypeAnaliz.Items.Add("OSH");
            TypeAnaliz.Items.Add("EMA");

            TypeAnaliz.SelectedIndex = 0;
            //Zakres analizy
            GamePrecisionComboBox.Items.Add("15");
            GamePrecisionComboBox.Items.Add("25");
            GamePrecisionComboBox.Items.Add("35");
            GamePrecisionComboBox.Items.Add("90");
            GamePrecisionComboBox.Items.Add("180");

            GamePrecisionComboBox.SelectedIndex = 3;
            //Co zrobić
            GameOptionComboBox.Items.Add("Ostatnie notowanie");
            GameOptionComboBox.Items.Add("Generuj wykres");
            GameOptionComboBox.Items.Add("Kupować czy sprzedawać");
            GameOptionComboBox.Items.Add("Obciążenie aplikacji (100%)");
            GameOptionComboBox.Items.Add("Obciążenie aplikacji (1 wątek)");
            //Date
            DateGame.DisplayDateStart = DateTime.Now.AddDays(-180);
            DateGame.DisplayDateEnd = DateTime.Now;
        }

        internal struct Game
        //public class Game
        {
            public string Ticker { get; set; }
            public DateTime Data { get; set; }
            public double Open { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public double Close { get; set; }
            public double Vol { get; set; }
        }
        public async void RunAsyncDiagnosticData()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    _DiagnosticService.ShowDiagnosticData();
                    Thread.Sleep(1000);
                }
            });
        }


        private async void GameAnalaizerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] myFile = File.ReadAllLines(@"bossa/" + GameComboBox.Text);
                var DataGielda = _loadAppData.LoadStructureGameReadOnlyColection2(myFile);

                if (GameOptionComboBox.Text == "Obciążenie aplikacji (100%)")
                {
                    await _AppFullDataAnalaizer.AllFileTestAllCore();
                }
                if (GameOptionComboBox.Text == "Obciążenie aplikacji (1 wątek)")
                {
                    _AppFullDataAnalaizer.AllFileTestOneCore();
                }
                if (GameOptionComboBox.Text == "Ostatnie notowanie")
                {
                    _SChart.Note(DataGielda);
                }




            }
            catch (Exception)
            {


            }



        }
        private void PCUUseProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void GameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GameOptionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GamePrecisionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GameLoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            _FileDowndAndMenager.RunAsyncDataDownd();
            GameLoadDataButton.IsEnabled = false;
        }

        private void GameScoreButton_Click(object sender, RoutedEventArgs e)
        {

        }






        private void RAMUseProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void GameTypeButton_Click(object sender, RoutedEventArgs e)
        {
            string[] myFile = File.ReadAllLines(@"bossa/" + GameComboBox.Text);
            var DataGielda = _loadAppData.LoadStructureGameReadOnlyColection2(myFile);

            int start, stop;
            start = Environment.TickCount & Int32.MaxValue;

            if (TypeAnaliz.Text == "MACD")
            {
                _MACD.Calculate(DataGielda, Convert.ToInt32(GamePrecisionComboBox.Text));

            }
            if (TypeAnaliz.Text == "RSI")
            {
                _RSI.Calculate(DataGielda, Convert.ToInt32(GamePrecisionComboBox.Text));

            }
            if (TypeAnaliz.Text == "OSH")
            {
                _OSH.Calculate(DataGielda, Convert.ToInt32(GamePrecisionComboBox.Text));

            }
            if (TypeAnaliz.Text == "EMA")
            {
                _EMA.Calculate(DataGielda, Convert.ToInt32(GamePrecisionComboBox.Text));

            }
            if (TypeAnaliz.Text == "Tylko index")
            {
                _SChart.Chart(DataGielda, Convert.ToInt32(GamePrecisionComboBox.Text));
                _SChart.Note(DataGielda);
            }
            stop = Environment.TickCount & Int32.MaxValue;
            int time = stop - start;
            GameOutputControlTextBlock.Text += TypeAnaliz.Text + " czas wykonania:" + time + "ms\n";
        }
    }
}
