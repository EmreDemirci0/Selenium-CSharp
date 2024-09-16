using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HepsiburadaVeri
{
    public partial class YeniBoynerYorumlar : Form
    {

        public YeniBoynerYorumlar()
        {
            InitializeComponent();
            //textBox1.Text = "https://www.boyner.com.tr/kiz-cocuk-11-14-yas-c-14374544";
            //textBox1.Text = "  https://www.boyner.com.tr/1-2-yas-bebek-abiye-elbise-c-12969870";
            textBox1.Text = "   https://www.boyner.com.tr/mammaramma-pembe-kiz-bebek-bisiklet-yaka-kisa-kollu-baskili-elbise-22sg-62-p-1130889";
        }

        void VeriGetir(string adres)
        {
            listBox1.Items.Clear();
            // ChromeDriver'ı başlat
            IWebDriver driver = new ChromeDriver();
            // Web sitesine git

            // Ürünün detay sayfasına git
            driver.Navigate().GoToUrl(adres);
            driver.Manage().Window.Maximize();

            string emre = "";
            System.Threading.Thread.Sleep(1000);

            try
            {
                //var button = driver.FindElement(By.XPath("//button[contains(@class, 'tabs_title__gO9Hr')]"));
                var button = driver.FindElement(By.XPath("//div[contains(@class, 'tabs_tab__gmnw2')]//div[contains(@class, 'tabs_item__CbCRV')][1]//button"));
                button.Click();
                MessageBox.Show("Clicked 'Tüm Değerlendirmeler' button successfully.");
            }
            catch (NoSuchElementException)
            {
                MessageBox.Show("'Tüm Değerlendirmeler' button not found.");
            }
            catch (ElementClickInterceptedException)
            {
                MessageBox.Show("Click on 'Tüm Değerlendirmeler' button intercepted.");
            }


            // Yorum tarihlerini çek
            try
            {
                // Yorum tarihlerini bul
                var reviewDates = driver.FindElements(By.XPath("//div[contains(@class, 'review-item_date__HSf_X')]"));
                foreach (var reviewDate in reviewDates)
                {
                    emre += reviewDate.Text + " ,";
                    // Yorum tarihini göster
                }
                MessageBox.Show("Yorum Tarihi: " + emre);

            }
            catch (NoSuchElementException)
            {
                MessageBox.Show("Yorum tarihleri bulunamadı.");
            }

            // Sayfayı maksimum boyuta getir

            

           
            driver.Quit();
        }

        private void YeniBoynerYorumlar_Load(object sender, EventArgs e)
        {

        }

        private void btn_Ara_Click(object sender, EventArgs e)
        {
            // Clear the listbox before populating new data
            listBox1.Items.Clear();

            string adres = textBox1.Text;
            VeriGetir(adres);

        }
    }
}
