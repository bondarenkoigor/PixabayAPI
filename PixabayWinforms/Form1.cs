using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace PixabayWinforms
{
    public partial class Form1 : Form
    {
        public class PixabaySearchResults
        {
            public int total { get; set; }
            public int totalHits { get; set; }
            public PixabaySearchHits[] hits { get; set; }
        }
        public class PixabaySearchHits
        {
            public int id { get; set; }
            public string largeImageURL { get; set; }
            public string type { get; set; }
            public string tags { get; set; }
        }

        WebClient webClient = new WebClient();

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Task getSearchResultsTask;

        public Form1()
        {
            InitializeComponent();
            getSearchResultsTask = new Task(GetSearchResults, tokenSource.Token);
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            tokenSource.Cancel();
            this.PicsFlowLayoutPanel.Controls.Clear();

            if (getSearchResultsTask.Status == TaskStatus.Running)
                getSearchResultsTask.Wait();
            tokenSource = new CancellationTokenSource();
            getSearchResultsTask = new Task(GetSearchResults, tokenSource.Token);
            getSearchResultsTask.Start();
        }
        private void GetSearchResults()
        {
            if (!Directory.Exists("tmpImages")) Directory.CreateDirectory("tmpImages");

            var result = JsonSerializer.Deserialize<PixabaySearchResults>(webClient.DownloadString($"https://pixabay.com/api/?key=28501108-d97b0f7079e828f6078c1e754&q={this.textBox1.Text.Replace(' ', '+')}&image_type=photo&pretty=true"));

            int counter = 0;
            foreach (var hit in result.hits)
            {
                webClient.DownloadFile(hit.largeImageURL, $"tmpImages\\image{counter}.png");
                PictureBox pictureBox = new PictureBox();
                Image tmp = new Bitmap($"tmpImages\\image{counter}.png");
                pictureBox.Image = new Bitmap(tmp);
                tmp.Dispose();
                pictureBox.Size = new Size(300, 200);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                this.BeginInvoke(new Action(() =>
                {
                    this.PicsFlowLayoutPanel.Controls.Add(pictureBox);
                }));
                counter++;
                if (tokenSource.IsCancellationRequested)
                {
                    tokenSource = new CancellationTokenSource();
                    getSearchResultsTask = new Task(GetSearchResults, tokenSource.Token);
                    break;
                }
            }
        }
    }
}
