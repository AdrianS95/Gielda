using Ionic.Zip;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;

namespace ATH_Giełda_Adrian_Smaza.Class.AppFunction
{
    class AppLogic
    {
        private readonly MainWindow _mainWindow;

        private AppFullDataAnalaizer _appFullDataAnalaizer;

        public AppLogic(
                MainWindow mainWindow,
                AppFullDataAnalaizer appFullDataAnalaizer
            )
        {
            _mainWindow = mainWindow;
            _appFullDataAnalaizer = appFullDataAnalaizer;
        }

        public void AllButtonLock()
        {
            _mainWindow.GameLoadDataButton.IsEnabled = false;
            _mainWindow.GameScoreButton.IsEnabled = false;
            _mainWindow.GameChart.IsEnabled = false;
            _mainWindow.GameAnalaizerButton.IsEnabled = false;
        }
        public void AllButtonUnLock()
        {

            _mainWindow.GameLoadDataButton.IsEnabled = true;
            _mainWindow.GameScoreButton.IsEnabled = true;
            _mainWindow.GameChart.IsEnabled = true;
            _mainWindow.GameAnalaizerButton.IsEnabled = true;

        }

       


    }
}
