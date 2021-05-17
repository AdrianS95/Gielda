using ATH_Giełda_Adrian_Smaza.Class.Analysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATH_Giełda_Adrian_Smaza.Class.AppFunction
{
    class AnalyzingData  //Zanjdą sięty funkcje kalkulejące między innymi opłacalność zakupu.
    {
        private readonly MainWindow _mainWindow;
        private readonly MovingAverageConvergenceDivergence _MACD;
        private readonly LoadAppData _loadAppData;
        private readonly RsiIndicator _RSI;
        private readonly OshIndicaror _OSH;

        

        public AnalyzingData(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _loadAppData = new LoadAppData(this);
            _MACD = new MovingAverageConvergenceDivergence(this);
            _RSI = new RsiIndicator(this);
            _OSH = new OshIndicaror(this);
        }

        public void BuyNowOneCompany(ReadOnlyCollection<CompanyDataItem> companyDatas, int dayPeriodLength)
        {
            var macd = _MACD.GetSignal(companyDatas, Convert.ToInt32(_mainWindow.GamePrecisionComboBox.Text)).Last();
            var rsi = _RSI.Calculate(companyDatas, Convert.ToInt32(_mainWindow.GamePrecisionComboBox.Text)).Last();
            var osh = _OSH.Calculate(companyDatas, Convert.ToInt32(_mainWindow.GamePrecisionComboBox.Text)).Last();

            int i = 900;
            if (macd.Value == 0)
            {
                i -= 100;
            }
            else if (macd.Value == -1)
            {
                i -= 300;
            }
            if (rsi.Value > 50)
            {
                int s = (int)(rsi.Value * 3);
                i += s;
            }
            else if (rsi.Value < 50)
            {
                int s = (int)(rsi.Value * 3);
                i -= s;
            }
            if (rsi.Value > 70)
            {
                int s = (int)(rsi.Value * 2);
                i += s;
            }
            else if (rsi.Value < 30)
            {
                int s = (int)(rsi.Value * 3);
                i -= s;
            }
            string tekst;
            if (i>800)
            {
                tekst = "Kup";
            }
            else
            {
                tekst = "sprzedaj";
            }
            _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                _mainWindow.GameOutputDataTextBlock.Text = "Dane Akcji\n";
                _mainWindow.GameOutputDataTextBlock.Text += "MACD = " + macd.Value +"\n";
                _mainWindow.GameOutputDataTextBlock.Text += "RSI  = " + rsi.Value + "\n";
                _mainWindow.GameOutputDataTextBlock.Text += "OSH  = " + osh.Value + "\n";
                _mainWindow.GameOutputDataTextBlock.Text += "Spułka uzyskała:  " + i+ " punktów: "+tekst+ "\n";

            }));
        }

        public void SellNowOnecompany()
        {

        }

        public void BuyNowAllCompany()
        {

        }

        public void SellNowAllcompany()
        {

        }




    }


    
}
