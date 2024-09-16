using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HepsiburadaVeri
{
    public partial class MuratForm : Form
    {
        public MuratForm()
        {
            InitializeComponent();
        }

        private void MuratForm_Load(object sender, EventArgs e)
        {
            //js.ExecuteScript("window.scrollBy(0,150)", "");
            //Thread.Sleep(1000);
            string adres = "http://www.boyner.com";
            WebRequest istek = HttpWebRequest.Create(adres);
            WebResponse cevap;
            cevap =istek.GetResponse();
            StreamReader donenbilgiler = new StreamReader(cevap.GetResponseStream()); 
            string gelen= donenbilgiler.ReadToEnd();
            int baslikbaslangic = gelen.IndexOf("<title>") + 7;
            int baslikbitisi = gelen.Substring(baslikbaslangic).IndexOf("</title>");
            string baslik =gelen.Substring(baslikbaslangic, baslikbitisi);
            MessageBox.Show(baslik);
        }
    }
}
