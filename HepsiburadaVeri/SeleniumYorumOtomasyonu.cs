using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HepsiburadaVeri
{
    public partial class SeleniumYorumOtomasyonu : Form
    {
        List<BoynerCategory> categoryList= new List<BoynerCategory>();
        
        public SeleniumYorumOtomasyonu()
        {
            InitializeComponent();
         
        }
        private void btn_Ara_Click(object sender, EventArgs e)
        {
            // Clear the listbox before populating new data
            //listBox1.Items.Clear();

            //string adres = textBox1.Text;
            UrunGetir();

            // Display the total count of products found
        }

        //Kategoriler
        private async void KategorileriBul()
        {
            string url = "https://statics.boyner.com.tr/bynsitemap/category.xml";

            // HttpClient ile webden XML'i çek
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // XML'i string olarak al
                    string xmlContent = await client.GetStringAsync(url);

                    // XmlDocument ile parse et
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlContent);

                    

                    // Namespace yönetici ekliyoruz
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("s", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    // <urlset> içindeki <url> etiketlerini namespace ile seçiyoruz
                    XmlNodeList urlNodes = xmlDoc.SelectNodes("//s:urlset/s:url", nsmgr);

                    if (urlNodes.Count == 0)
                    {
                        MessageBox.Show("Hiçbir <url> etiketi bulunamadı.");
                    }
                    else
                    {
                        foreach (XmlNode node in urlNodes)
                        {
                            string loc = node["loc"].InnerText;
                            string lastmod = node["lastmod"].InnerText;
                            string changefreq = node["changefreq"].InnerText;
                            string priority = node["priority"].InnerText;


                            BoynerCategory category = new BoynerCategory();
                            category.KategoryLinki = loc;
                            category.changeFreq = changefreq;
                            category.LastMod = DateTime.Parse(lastmod);
                            category.priority = priority;
                            categoryList.Add(category);
                            //MessageBox.Show($"URL: {loc}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message);
                }
            }
        }

        //Urunler
        private void UrunGetir()
        {
            listBox1.Items.Clear();
            IWebDriver driver = new ChromeDriver();
          
            
            KategorileriBul();
            MessageBox.Show("Kategoriler Yüklendi");
            foreach (BoynerCategory item in categoryList)
            {
                driver.Navigate().GoToUrl(/*item.KategoryLinki*/"https://www.boyner.com.tr/13-14-yas-mont-c-12969866");
                driver.Manage().Window.Maximize();
                List<BoynerProducts> productsList = new List<BoynerProducts>();
                ScrollKadarGezUrunler(driver);
                UrunleriBul(driver, productsList);

                foreach (BoynerProducts item2 in productsList)
                {
                    UrunDetayGetir(driver, item2);
                }


            }

            driver.Quit();
        }
        private void ScrollKadarGezUrunler(IWebDriver driver)
        {
            while (true)
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                // Sayfanın scroll pozisyonunu ve toplam yüksekliğini al
                object scrollPositionObj = js.ExecuteScript("return window.pageYOffset;");
                object scrollHeightObj = js.ExecuteScript("return document.body.scrollHeight;");
                object windowHeightObj = js.ExecuteScript("return window.innerHeight;");

                long scrollPosition = Convert.ToInt64(scrollPositionObj);
                long scrollHeight = Convert.ToInt64(scrollHeightObj);
                long windowHeight = Convert.ToInt64(windowHeightObj);

                // Sayfa aşağı kaydırılıyor
                js.ExecuteScript("window.scrollBy(0,50)", "");
                Thread.Sleep(50);

                // Eğer scroll pozisyonu ve sayfa yüksekliği birbirine eşitse veya son kaydırmaya yaklaşıldıysa break
                if (scrollPosition + windowHeight >= scrollHeight - 5)
                {
                    break;
                }
            }
        }
        private void UrunleriBul(IWebDriver driver, List<BoynerProducts> productsList)
        {

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
                listBox1.Items.Add(item.UrunAdi);
            }


            // Ürün listesini yazdır
            //foreach (var p in productsList)
            //{
            //    MessageBox.Show($"Marka: {p.MarkaAdi}, \nÜrün: {p.UrunAdi}, \nLink: {p.UrunLink}, \nİndirimsiz Fiyat: {p.IndirimsizFiyat} TL, \nİndirim Oranı: %{p.IndirimOrani}, \nİndirimli Fiyat: {p.IndirimliFiyat} TL, \nDeğerlendirme Sayısı: {p.TotalDegerlendirmeSayisi}, \nDeğerlendirme Sayısı: {p.TotalDegerlendirmeSayisi}");
            //}

        }

        //Yorumlar
        private void UrunDetayGetir(IWebDriver driver, BoynerProducts product)
        {
            System.Threading.Thread.Sleep(1000);

            driver.Navigate().GoToUrl(product.UrunLink);
            driver.Manage().Window.Maximize();

            System.Threading.Thread.Sleep(1000);

            YorumPanelineGit(driver);
            if (degerlendirmeVarMi)
            {
                Thread.Sleep(2000);

                ScrollKadarGezYorumlar(driver);
                YorumTarihIslemleri(driver, product);

               
            }

            Thread.Sleep(2000);

        }
        bool degerlendirmeVarMi = true;
        private void YorumPanelineGit(IWebDriver driver)
        {
            //Yorumlar sekmesini aç
            try
            {
                //var button = driver.FindElement(By.XPath("//div[contains(@class, 'tabs_tab__gmnw2')]//div[contains(@class, 'tabs_item__CbCRV')][1]//button"));
                //button.Click();

                IWebElement button = driver.FindElement(By.XPath("//div[contains(@class, 'tabs_tab__gmnw2')]//div[contains(@class, 'tabs_item__CbCRV')][1]//button"));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].click();", button);
                //MessageBox.Show("Clicked 'Tüm Değerlendirmeler' button successfully.");
                degerlendirmeVarMi = true;

            }
            catch (NoSuchElementException)
            {
                //MessageBox.Show("'Tüm Değerlendirmeler' button not found.");
                degerlendirmeVarMi = false;
            }
            catch (ElementClickInterceptedException)
            {
                MessageBox.Show("Click on 'Tüm Değerlendirmeler' button intercepted.");
                degerlendirmeVarMi = false;
            }

            System.Threading.Thread.Sleep(1000);
            //Tüm yorumları göster butonuna bas
            try
            {

                IWebElement button = driver.FindElement(By.XPath("//a[contains(@class, 'score-summary_showAll')]"));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].click();", button);
                //MessageBox.Show("Clicked 'Tüm Değerlendirmeler' button successfully.");
                degerlendirmeVarMi = true;
            }
            catch (NoSuchElementException)
            {
                //MessageBox.Show("'Tüm Değerlendirmeler' button not found.");
                degerlendirmeVarMi = false;
            }
            catch (ElementClickInterceptedException)
            {
                MessageBox.Show("Click on 'Tüm Değerlendirmeler' button intercepted.");
                degerlendirmeVarMi = false;
            }
        }
        private void YorumTarihIslemleri(IWebDriver driver, BoynerProducts product)
        {
            string tarihler = "";
            try
            {
                // Yorum tarihlerini bul
                var reviewDates = driver.FindElements(By.XPath("//div[contains(@class, 'review-item_date__HSf_X')]"));
                var now = DateTime.Now;
                var oneMonthAgo = now.AddMonths(-1); // 1 ay önce
                var SonBirAyDegerlendirmeSayisiCount = 0;
                foreach (var reviewDate in reviewDates)
                {
                    // Yorum tarihini DateTime tipine çevir
                    DateTime parsedDate;
                    bool isParsed = DateTime.TryParse(reviewDate.Text, out parsedDate);

                    if (isParsed && parsedDate >= oneMonthAgo)
                    {
                        // Yorum tarihi 1 ay içinde ise MessageBox'a ekle
                        tarihler += parsedDate.ToString("dd.MM.yyyy") + " ";
                        SonBirAyDegerlendirmeSayisiCount++;
                    }
                }

                if (!string.IsNullOrEmpty(tarihler))
                {
                    //MessageBox.Show(reviewDates.Count + " Adet Total Yorum Var. Son bir yıldaki Yorumların Sayısı: " + SonBirAyDegerlendirmeSayisiCount + "\nTarihler:" + tarihler);
                    product.SonBirAyDegerlendirmeSayisi = SonBirAyDegerlendirmeSayisiCount;

                    //MessageBox.Show($"Marka: {product.MarkaAdi}, " +
                    //    $"\nÜrün: {product.UrunAdi}, " +
                    //    $"\nLink: {product.UrunLink}, " +
                    //    $"\nİndirimsiz Fiyat: {product.IndirimsizFiyat} TL, " +
                    //    $"\nİndirim Oranı: %{product.IndirimOrani}, " +
                    //    $"\nİndirimli Fiyat: {product.IndirimliFiyat} TL, " +
                    //    $"\nDeğerlendirme Sayısı: {product.TotalDegerlendirmeSayisi}, " +
                    //    $"\nSon Bir Ay Değerlendirme Sayısı : {product.SonBirAyDegerlendirmeSayisi}");

                }
                else
                {
                    product.SonBirAyDegerlendirmeSayisi = 0;
                    //MessageBox.Show("Son 1 ay içinde yazılan yorum bulunamadı.");
                }
            }
            catch (NoSuchElementException)
            {
                MessageBox.Show("Yorum tarihleri bulunamadı.");
            }
            catch (FormatException)
            {
                MessageBox.Show("Tarih formatı hatalı.");
            }
        }
        private void ScrollKadarGezYorumlar(IWebDriver driver)
        {
            while (true)
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                // Sayfanın scroll pozisyonunu ve toplam yüksekliğini al
                object scrollPositionObj = js.ExecuteScript("return window.pageYOffset;");
                object scrollHeightObj = js.ExecuteScript("return document.body.scrollHeight;");
                object windowHeightObj = js.ExecuteScript("return window.innerHeight;");

                long scrollPosition = Convert.ToInt64(scrollPositionObj);
                long scrollHeight = Convert.ToInt64(scrollHeightObj);
                long windowHeight = Convert.ToInt64(windowHeightObj);

                // Sayfa aşağı kaydırılıyor
                js.ExecuteScript("window.scrollBy(0,50)", "");
                Thread.Sleep(50);

                // Eğer scroll pozisyonu ve sayfa yüksekliği birbirine eşitse veya son kaydırmaya yaklaşıldıysa break
                if (scrollPosition + windowHeight >= scrollHeight - 5)
                {
                    break;
                }
            }
        }

        private void YeniBoyner4_Load(object sender, EventArgs e)
        {

        }
    }
}


