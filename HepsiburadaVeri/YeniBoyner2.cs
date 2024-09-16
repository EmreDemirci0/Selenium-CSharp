using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HepsiburadaVeri
{
    public partial class YeniBoyner2 : Form
    {
        public YeniBoyner2()
        {
            InitializeComponent();
            textBox1.Text = "https://www.boyner.com.tr/kiz-cocuk-11-14-yas-c-14374544";
            //textBox1.Text = "  https://www.boyner.com.tr/1-2-yas-bebek-abiye-elbise-c-12969870";
        }

        void VeriGetir(string adres)
        {
            listBox1.Items.Clear();
            // ChromeDriver'ı başlat
            IWebDriver driver = new ChromeDriver();
            // Web sitesine git
            driver.Navigate().GoToUrl(adres);



            // Sayfayı maksimum boyuta getir
            driver.Manage().Window.Maximize();

            // Ürünleri listelemek için bir liste oluştur
            List<BoynerProducts> productsList = new List<BoynerProducts>();

            // Ürün bloklarını bul
            var productElements = driver.FindElements(By.XPath("//div[contains(@class, 'product-item_root')]"));

            foreach (var productElement in productElements)
            {
                //MessageBox.Show("ProductElement.findelement" + productElement.Text);

                BoynerProducts product = new BoynerProducts();

                // Marka adını bul
                try
                {
                    IWebElement markaElement = productElement.FindElement(By.XPath(".//div[contains(@class, 'product-item_brand')]"));
                    product.MarkaAdi = markaElement.Text;


                }
                catch (NoSuchElementException) { product.MarkaAdi = "Marka adı bulunamadı"; }

                // Ürün adını bul
                try
                {
                    IWebElement urunAdiElement = productElement.FindElement(By.XPath(".//h3[contains(@class, 'product-item_name')]"));
                    product.UrunAdi = urunAdiElement.Text;

                }
                catch (NoSuchElementException) { product.UrunAdi = "Ürün adı bulunamadı"; }

                // Ürün linkini bul
                try
                {
                    IWebElement UrunLinkelement = productElement.FindElement(By.XPath(".//a"));
                    product.UrunLink = UrunLinkelement.GetAttribute("href");
                }
                catch (NoSuchElementException) { product.UrunLink = "Link bulunamadı"; }

                // İndirimsiz fiyatı bul
                try
                {
                    string oldPriceText = productElement.FindElement(By.XPath(".//div[contains(@class, 'product-price_oldPrice')]")).Text;

                    oldPriceText = oldPriceText.Replace("TL", "").Replace(".", ",");
                    product.IndirimsizFiyat = Convert.ToDouble(oldPriceText.Trim());
                }
                catch (NoSuchElementException) { product.IndirimsizFiyat = 0.0; }


                try
                {
                    // İndirim oranını bul ve temizle
                    string discountText = productElement.FindElement(By.XPath(".//div[contains(@class, 'product-price_basketDiscount__FPRKK')]")).Text;

                    // İndirim oranındaki istenmeyen metinleri temizle
                    discountText = discountText.Replace("%", "").Replace("Sepette", "").Replace("İndirimli", "").Trim();

                    // İndirim oranını sayıya dönüştür ve atama yap
                    product.IndirimOrani = Convert.ToDouble(discountText);
                }
                catch (NoSuchElementException)
                {
                    product.IndirimOrani = 0.0; // Eğer element bulunamazsa varsayılan değer
                }
                catch (FormatException)
                {
                    product.IndirimOrani = 0.0; // Dize doğru formatta değilse varsayılan değer
                }


                try
                {
                    // İndirimli fiyatı div'den bul ve strong elementinin içindeki metni al
                    string discountedPriceText = productElement.FindElement(By.XPath(".//div[contains(@class, 'product-price_checkPrice__NMY9e')]//strong")).Text;
                    discountedPriceText = discountedPriceText.Replace("TL", "").Replace(".", ",");
                    // Mesaj kutusunda indirimli fiyatı göster

                    // TL işaretini ve gereksiz boşlukları temizle, ardından double'a dönüştür
                    product.IndirimliFiyat = Convert.ToDouble(discountedPriceText.Trim());
                }
                catch (NoSuchElementException)
                {
                    product.IndirimliFiyat = 0.0; // Eğer element bulunamazsa varsayılan değer
                }
                catch (FormatException)
                {
                    product.IndirimliFiyat = 0.0; // Dize doğru formatta değilse varsayılan değer
                }


                // Toplam değerlendirme sayısını bul
                try
                {
                    string reviewCountText = productElement.FindElement(By.XPath(".//div[contains(@class, 'rating-custom_count')]")).Text;
                    reviewCountText = reviewCountText.Replace("(", "").Replace(")", "").Trim();
                    product.TotalDegerlendirmeSayisi = Convert.ToInt32(reviewCountText.Trim());
                }
                catch (NoSuchElementException) { product.TotalDegerlendirmeSayisi = 0; }
                // // Ürünü listeye ekle
                productsList.Add(product);
            }
            foreach (var item in productsList)
            {
                Yorumlar(driver,item);
            }
            foreach (var item in productsList)
            {
                listBox1.Items.Add(item.UrunAdi);
            }
            // Ürün listesini yazdır
            foreach (var p in productsList)
            {
                MessageBox.Show($"Marka: {p.MarkaAdi}, \nÜrün: {p.UrunAdi}, \nLink: {p.UrunLink}, \nİndirimsiz Fiyat: {p.IndirimsizFiyat} TL, \nİndirim Oranı: %{p.IndirimOrani}, \nİndirimli Fiyat: {p.IndirimliFiyat} TL, \nDeğerlendirme Sayısı: {p.TotalDegerlendirmeSayisi}");
            }
            driver.Quit();
        }





        private void btn_Ara_Click(object sender, EventArgs e)
        {
            // Clear the listbox before populating new data
            listBox1.Items.Clear();

            string adres = textBox1.Text;
            VeriGetir(adres);

            // Display the total count of products found
        }
        private void Yorumlar(IWebDriver driver,BoynerProducts product)
        {
            System.Threading.Thread.Sleep(2000);

            driver.Navigate().GoToUrl(/*product.UrunLink*/"https://www.boyner.com.tr/tommy-jeans-bisiklet-yaka-duz-siyah-kadin-t-shirt-tjw-bxy-badge-tee-ext-p-1780408");
            driver.Manage().Window.Maximize();

            string emre = "";
            System.Threading.Thread.Sleep(2000);

            try
            {
                //var button = driver.FindElement(By.XPath("//div[contains(@class, 'tabs_tab__gmnw2')]//div[contains(@class, 'tabs_item__CbCRV')][1]//button"));
                //button.Click();

                IWebElement button = driver.FindElement(By.XPath("//div[contains(@class, 'tabs_tab__gmnw2')]//div[contains(@class, 'tabs_item__CbCRV')][1]//button"));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].click();", button);
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

            //try
            //{

            //    IWebElement button = driver.FindElement(By.XPath("//a[contains(@class, 'score-summary_showAll')]"));
            //    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //    js.ExecuteScript("arguments[0].click();", button);
            //    MessageBox.Show("Clicked 'Tüm Değerlendirmeler' button successfully.");
            //}
            //catch (NoSuchElementException)
            //{
            //    MessageBox.Show("'Tüm Değerlendirmeler' button not found.");
            //}
            //catch (ElementClickInterceptedException)
            //{
            //    MessageBox.Show("Click on 'Tüm Değerlendirmeler' button intercepted.");
            //}


            //// Yorum tarihlerini çek
            //try
            //{
            //    // Yorum tarihlerini bul
            //    var reviewDates = driver.FindElements(By.XPath("//div[contains(@class, 'review-item_date__HSf_X')]"));

            //    foreach (var reviewDate in reviewDates)
            //    {
            //        emre += reviewDate.Text + " ,";
            //        // Yorum tarihini göster
            //    }
            //    MessageBox.Show(reviewDates.Count + " Yorum Tarihi: " + emre);

            //}
            //catch (NoSuchElementException)
            //{
            //    MessageBox.Show("Yorum tarihleri bulunamadı.");
            //}

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


            Thread.Sleep(2000);

        }
        private void YeniBoyner2_Load(object sender, EventArgs e)
        {

        }

        
    }
}


