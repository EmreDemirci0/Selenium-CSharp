using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace HepsiburadaVeri
{
    public partial class Form1 : Form
    {
        private IWebDriver driver;
        public Form1()
        {
            InitializeComponent();
            //VeriGetir();
            //VeriGetir(adres);
        }


        void VeriGetir2(string adres)
        {
            if (!Uri.IsWellFormedUriString(adres, UriKind.Absolute))
            {
                MessageBox.Show("Geçersiz URL formatı. Lütfen geçerli bir URL giriniz.");
                return;
            }

            // ChromeOptions kullanarak gizli modda tarayıcı başlatabilirsiniz
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless"); // Tarayıcıyı arka planda çalıştırır (görünmez mod)

            // WebDriver ile Chrome tarayıcısını başlatın
            driver = new ChromeDriver(options);

            try
            {
                // Belirtilen URL'ye git
                driver.Navigate().GoToUrl(adres);

                // Sayfanın tam yüklenmesi için bekleme süresi koyun
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => driver.FindElement(By.XPath("//h1")));

                // Ürün başlığını, satıcı adını ve fiyatı çek
                var productTitle = driver.FindElement(By.XPath("//h1[@class='pr-new-br']/span")).Text;
                var productMarka = driver.FindElement(By.XPath("//h1[@class='pr-new-br']/a")).Text;
                var productPrice = driver.FindElement(By.XPath("//span[@class='prc-dsc']")).Text;

                // Bu değerleri formdaki label'lara aktar
                lbl_urunAd.Text = !string.IsNullOrEmpty(productTitle) ? productTitle : "Başlık bulunamadı";
                lbl_saticiAd.Text = !string.IsNullOrEmpty(productMarka) ? productMarka : "Marka bulunamadı";
                lbl_fiyat.Text = !string.IsNullOrEmpty(productPrice) ? productPrice : "Fiyat bulunamadı";

             
                // Reyting gibi başka verileri çekmek için XPath'i kullanabilirsiniz
                var rating = driver.FindElement(By.XPath("//div[contains(@class, 'rating-line-count')]")).Text;
                lbl_reyting.Text = !string.IsNullOrEmpty(rating) ? rating : "Reyting bulunamadı";

                var degerlendirme = driver.FindElement(By.XPath("//span[contains(@class, 'total-review-count')]")).Text;
                lbl_degerlendirmeSayisi.Text = !string.IsNullOrEmpty(degerlendirme) ? degerlendirme : "Değerlendirme bulunamadı";

                var sorucevap = driver.FindElement(By.XPath("//span[contains(@class, 'answered-questions-count')]")).Text;
                lbl_soruCevap.Text = !string.IsNullOrEmpty(sorucevap) ? sorucevap : "Soru-Cevap bulunamadı";
            }
            catch (NoSuchElementException ex)
            {
                MessageBox.Show("Element bulunamadı: " + ex.Message);
            }
            catch (WebDriverException ex)
            {
                MessageBox.Show("Tarayıcı hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Beklenmeyen hata: " + ex.Message);
            }
            finally
            {
                // Tarayıcıyı kapat
                driver.Quit();
            }
        }
        void VeriGetir(string adres)
        {
            if (!Uri.IsWellFormedUriString(adres, UriKind.Absolute))
            {
                MessageBox.Show("Geçersiz URL formatı. Lütfen geçerli bir URL giriniz.");
                return;
            }

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless"); // Tarayıcıyı arka planda çalıştırır (görünmez mod)
            driver = new ChromeDriver(options);

            try
            {
                driver.Navigate().GoToUrl(adres);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                //wait.Until(driver => driver.FindElement(By.XPath("//div[@class='srch-prdcts-cntnr srch-prdcts-cntnr-V2']")));
                


                // Get all product names in the list using XPath for the product title attribute
                //bunu asagı alıcaz
                var productElements = driver.FindElements(By.XPath("//div[@class='p-card-wrppr with-campaign-view']"));

                foreach (var productElement in productElements)
                {
                    var productName = productElement.GetAttribute("title");
                    if (!string.IsNullOrEmpty(productName))
                    {
                        // Add the product name to the ListBox
                        listBox1.Items.Add(productName);
                    }
                }

                MessageBox.Show($"Toplam ürün sayısı: {listBox1.Items.Count}");
            }
            catch (NoSuchElementException ex)
            {
                MessageBox.Show("Element bulunamadı: " + ex.Message);
            }
            catch (WebDriverException ex)
            {
                MessageBox.Show("Tarayıcı hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Beklenmeyen hata: " + ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string adres = "https://www.trendyol.com/apple/iphone-15-pro-max-256-gb-mavi-titanyum-p-762254884?boutiqueId=638145&merchantId=968";
            VeriGetir(adres);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string adres = " https://www.trendyol.com/pasabahce/nova-su-mesrubat-kahve-yani-bardak-takimi-18-parca-fma07090-045oa-228-p-70901734?boutiqueId=61&merchantId=564754&sav=true";
            VeriGetir(adres);
        }

        private void btn_ara_Click(object sender, EventArgs e)
        {
            //listBox1.Items.Clear();
            VeriGetir(textBox1.Text);
          
            MessageBox.Show("Count:"+listBox1.Items.Count);
            lbl_degerlendirmeSayisi.Visible = true;
            lbl_fiyat.Visible = true;
            lbl_reyting.Visible = true;
            lbl_saticiAd.Visible = true;
            lbl_soruCevap.Visible = true;
            lbl_urunAd.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            label4.Visible = true;
            label8.Visible = true;
        }
    }
}
