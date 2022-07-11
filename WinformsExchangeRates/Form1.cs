using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsExchangeRates
{
    public partial class Form1 : Form
    {
        public class ExchangeRatesInfo
        {
            public string date { get; set; }
            public string bank { get; set; }
            public double baseCurrency { get; set; }
            public string baseCurrencyLit { get; set; }
            public ExchangeRate[] exchangeRate { get; set; }
        }

        public class ExchangeRate
        {
            public string baseCurrency { get; set; }
            public string currency { get; set; }
            public double saleRateNB { get; set; }
            public double purchaseRateNB { get; set; }
        }

        WebClient client = new WebClient();

        public Form1()
        {
            InitializeComponent();
            this.monthCalendar1.DateSelected += MonthCalendar1_DateSelected;
        }

        private void MonthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            if(e.Start.CompareTo(DateTime.Now) > 0) this.monthCalendar1.SelectionStart = DateTime.Now;
            Task getExchangeRates = Task.Factory.StartNew(() =>
            {
                StringBuilder sb = new StringBuilder();
                var exchangeRatesInfo = JsonSerializer.Deserialize<ExchangeRatesInfo>(client.DownloadString($"https://api.privatbank.ua/p24api/exchange_rates?json&date={this.monthCalendar1.SelectionStart.ToShortDateString()}"));
                foreach (var exchangeRate in exchangeRatesInfo.exchangeRate)
                {
                    sb.Append($"Base Currency: {exchangeRate.baseCurrency}\n");
                    sb.Append($"Currency: {exchangeRate.currency}\n");
                    sb.Append($"Sale Rate: {exchangeRate.saleRateNB}\n");
                    sb.Append($"Purchase Rate: {exchangeRate.purchaseRateNB}\n");
                    sb.Append("-------------------------\n");
                }
                this.BeginInvoke(new Action(() =>
                {
                    this.label2.Text = $"Date:{exchangeRatesInfo.date}\nBank: {exchangeRatesInfo.bank}\nBase Currency: {exchangeRatesInfo.baseCurrency}\nBaseCurrencyLit:{exchangeRatesInfo.baseCurrencyLit}";
                    this.label1.Text = sb.ToString();

                }));
            });
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
