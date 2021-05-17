using ATH_Giełda_Adrian_Smaza.Class;
using ATH_Giełda_Adrian_Smaza.Class.AppFunction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ATH_Giełda_Adrian_Smaza.MainWindow;
using System.Linq;

namespace ATH_Giełda_Adrian_Smaza
{

    class LoadAppData
    {
        private readonly MainWindow _mainWindow;
        private AppFullDataAnalaizer appFullDataAnalaizer;
        private AnalyzingData analyzingData;
        private FileDowndAndMenager fileDowndAndMenager;
        private readonly System.Globalization.CultureInfo _customCulture;
        public LoadAppData(MainWindow mainWindow) : this()
        {
            _mainWindow = mainWindow;
        }

        public LoadAppData(AppFullDataAnalaizer appFullDataAnalaizer) : this()
        {
            this.appFullDataAnalaizer = appFullDataAnalaizer;
        }

        public LoadAppData(AnalyzingData analyzingData) : this()
        {
            this.analyzingData = analyzingData;
        }

        public LoadAppData(FileDowndAndMenager fileDowndAndMenager) : this()
        {
            this.fileDowndAndMenager = fileDowndAndMenager;
        }

        public LoadAppData()
        {
            _customCulture = CultureInfo.GetCultureInfo("en-US");
        }


        private const string RegexDl = @"([^@]{1,20}),([0-9]+),([0-9.]+),([0-9.]+),([0-9.]+),([0-9.]+),([0-9]+)";
        private readonly Regex _regex = new Regex(RegexDl);

        public ReadOnlyCollection<CompanyDataItem> LoadStructureGameReadOnlyColection2(string[] lines) //Funkcja tworząca kolekcje plików
        {
            var structDataGame = new List<CompanyDataItem>(lines.Length);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var splittedLine = line.Split(',');

                var companyDataItem = new CompanyDataItem(
                    splittedLine[1],
                    Convert.ToSingle(splittedLine[2], _customCulture),
                    Convert.ToSingle(splittedLine[3], _customCulture),
                    Convert.ToSingle(splittedLine[4], _customCulture),
                    Convert.ToSingle(splittedLine[5], _customCulture),
                    Convert.ToSingle(splittedLine[6], _customCulture));
                structDataGame.Add(companyDataItem);
            }

            return new ReadOnlyCollection<CompanyDataItem>(structDataGame);
        }
    }
}
