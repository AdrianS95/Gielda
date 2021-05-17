using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ATH_Giełda_Adrian_Smaza
{
    class DiagnosticService
    {
        private readonly MainWindow _mainWindow;
        public DiagnosticService(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }
        PerformanceCounter cpu_use = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        PerformanceCounter ram_use = new PerformanceCounter("Memory", "% Committed Bytes In Use");

        public void ShowDiagnosticData() //Funkcja pokazująca użycie cpu tekst + (progres bar)
        {
            var cpuUse = (int)(cpu_use.NextValue());
            var ramUse = (int)(ram_use.NextValue());

            _mainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                _mainWindow.CPUUseProgressBar.Value = cpuUse;
                _mainWindow.RAMUseProgressBar.Value = ramUse;

                _mainWindow.CPUpr.Content = cpuUse + "%";
                _mainWindow.RAMpr.Content = ramUse + "%";

                if (cpuUse > 80)
                    _mainWindow.CPUUseProgressBar.Foreground = Brushes.Red;
                else
                    _mainWindow.CPUUseProgressBar.Foreground = Brushes.LightGreen;

                if (ramUse > 80)
                    _mainWindow.RAMUseProgressBar.Foreground = Brushes.Red;
                else
                    _mainWindow.RAMUseProgressBar.Foreground = Brushes.LightGreen;
            }));
        }

        public async void RunAsyncDiagnosticData() //Wypołanie funkcji pokazującej aktualne użycie CPU i RAM w interwale 500ms
        {
            await Task.Run(async() =>
            {
                await Task.Delay(1);
                while (true)
                {
                    ShowDiagnosticData();
                    Thread.Sleep(500);
                }
            });
        }

    }
}

