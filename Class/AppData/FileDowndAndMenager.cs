using Ionic.Zip;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ATH_Giełda_Adrian_Smaza
{
    class FileDowndAndMenager
    {
        private readonly MainWindow _mainWindow;
        private readonly LoadAppData _loadAppData;
        public FileDowndAndMenager(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _loadAppData = new LoadAppData(this);
        }

        public async void RunAsyncDataDownd()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(1);
                DowndFile();
            });
        }

        public void DowndFile() // funkcja pobierająca plik
        {
            WebClient webUserAdrian = new WebClient();
            try
            {
                _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _mainWindow.GameOutputControlTextBlock.Text += "Pobieram Dane z: \nhttp://bossa.pl/pub/metastock/mstock/mstall.zip\nTrochę to potrwa\n";
                }));
                webUserAdrian.DownloadFile("http://bossa.pl/pub/metastock/mstock/mstall.zip", "mstall.zip");
                _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _mainWindow.GameOutputControlTextBlock.Text += "Pobieram zakończone sukcesem\n";
                }));

                RunAsyncOpenZip();

            }
            catch (WebException)
            {
                _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _mainWindow.GameOutputControlTextBlock.Text += "Błąd pobierania pliku z: \nhttp://bossa.pl/pub/metastock/mstock/'mstall.zip'\n";
                }));
            }
        }

        public async void RunAsyncOpenZip()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(1);
                ZipOpen();
            });
        }


        public void ZipOpen() // wypakowanie popranego pliku zip
        {
            _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                _mainWindow.GameOutputControlTextBlock.Text += "Rozpoczynam wypakowywanie pliku 'mstall.zip'\n";
            }));
            try
            {
                ZipFile zip = ZipFile.Read("mstall.zip");
                Directory.CreateDirectory("bossa");
                zip.ExtractAll("bossa", ExtractExistingFileAction.OverwriteSilently);
                _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _mainWindow.GameOutputControlTextBlock.Text += "Plik 'mstall.zip' został wypakowany\n";
                    _mainWindow.GameOutputControlTextBlock.Text += "Kasuje małe i nieaktualne pliki.\n";
                }));
                DeleteSmalFile();// wywołanie funkcji kasującej małe pliki
                DeleteOldDataFile();// wywołanie funkcji kasującej nie aktualne pliki
            }
            catch (Exception)
            {
                _mainWindow.GameOutputControlTextBlock.Text += "Plik 'mstall.zip' nie został wypakowany!\n";
            }
        }

        public void DeleteSmalFile() //Kasowanie małych plików 
        {
            string[] mstFiles = Directory.GetFiles("bossa", "*.mst").Select(Path.GetFileName).ToArray();

            Parallel.For(0, mstFiles.Length, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, isx =>
            {
                string[] myFile = File.ReadAllLines(@"bossa/" + mstFiles[isx]);
                var DataGielda = _loadAppData.LoadStructureGameReadOnlyColection2(myFile);

                if (DataGielda.Count < 200) // odrzucamy pliki poniżej 200 lini;
                {
                    try
                    {
                        System.IO.File.Delete(@"bossa/" + mstFiles[isx]);
                    }
                    catch (System.IO.IOException)
                    {
                    }
                }
            });
        }

        public void DeleteOldDataFile()//kasuje stare pliki (ostatnia notowanie może byń max 10 mniejsze od dateTime>now)
        {
            string[] mstFiles = Directory.GetFiles("bossa", "*.mst").Select(Path.GetFileName).ToArray();
            DateTime dataNow = DateTime.Now.Date;

            Parallel.For(0, mstFiles.Length, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, isx =>
            {
                string[] myFile = File.ReadAllLines(@"bossa/" + mstFiles[isx]);
                var DataGielda = _loadAppData.LoadStructureGameReadOnlyColection2(myFile);
                string castedDataString = DataGielda.Last().Date;

                bool tmp = false;

                for (int i = 0; i < 10; i++)
                {
                    string date = dataNow.AddDays(i*(-1)).ToString("yyyyMMdd");
                    if (date == castedDataString)
                    {
                        tmp = true;
                    }
                }

                if (tmp == false)
                {
                    try
                    {
                        System.IO.File.Delete(@"bossa/" + mstFiles[isx]);
                    }
                    catch (System.IO.IOException)
                    {
                    }
                }
            });
        }
    }
}

