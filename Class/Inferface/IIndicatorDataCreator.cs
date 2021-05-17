using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ATH_Giełda_Adrian_Smaza.Class
{
    //Wsp€lny interfejs klass wykorzystany przez kilka osób
    public interface IIndicatorDataCreator
    {
        ReadOnlyCollection<IndicatorData> Calculate(
            ReadOnlyCollection<CompanyDataItem> companyDatas,
            int dayPeriodLength
        );
        ReadOnlyCollection<IndicatorData> GetSignal(
            ReadOnlyCollection<CompanyDataItem> companyDatas,
            int dayPeriodLength
        );


    }

    public class IndicatorData
    {

        public IndicatorData(string endDate, float value)
        {
            Date = endDate;
            Value = value;
        }

        public string Date { get; }
        public float Value { get; }
    }

    public class CompanyData
    {
        public CompanyData(
                string ticker,
                CompanyDataItem[] items
            )
        {
            Ticker = ticker;
            Items = items;
        }
        public string Ticker { get; }
        public CompanyDataItem[] Items { get; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CompanyDataItem
    {
        public CompanyDataItem(
            string date,
                float open,
                float high,
                float low,
                float close,
                float vol
            )
        {
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Vol = vol;
        }
        public string Date { get; }
        public float Open { get; }
        public float High { get; }
        public float Low { get; }
        public float Close { get; }
        public float Vol { get; }
    }
}