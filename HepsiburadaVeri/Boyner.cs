using System;
using System.Linq;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace HepsiburadaVeri
{
    public partial class Boyner : Form
    {
        private IWebDriver driver;

        public Boyner()
        {
            InitializeComponent();
            textBox1.Text = "https://www.boyner.com.tr/kiz-cocuk-11-14-yas-c-14374544";
        }

        private void btn_Ara_Click(object sender, EventArgs e)
        {
            // Clear the listbox before populating new data
            listBox1.Items.Clear();

            string adres = textBox1.Text;
            VeriGetir(adres);

            // Display the total count of products found
            MessageBox.Show("Toplam ürün sayısı: " + listBox1.Items.Count);
        }

        void VeriGetir(string adres)
        {
            if (!Uri.IsWellFormedUriString(adres, UriKind.Absolute))
            {
                MessageBox.Show("Geçersiz URL formatı. Lütfen geçerli bir URL giriniz.");
                return;
            }

            ChromeOptions options = new ChromeOptions();
            driver = new ChromeDriver(options);

            try
            {
                driver.Navigate().GoToUrl(adres);

                // Program başlar başlamaz 5 saniye beklet
                System.Threading.Thread.Sleep(5000);

                // Popup kapatma butonunu bul ve tıkla
                try
                {
                    var closeButton = driver.FindElement(By.XPath("//span[contains(@class, 'ins-close-button')]"));
                    if (closeButton.Displayed && closeButton.Enabled)
                    {
                        closeButton.Click();
                        MessageBox.Show("Popup kapatıldı.");
                    }
                }
                catch (NoSuchElementException)
                {
                    MessageBox.Show("Popup bulunamadı.");
                }

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                // Wait for the product list container to be loaded
                wait.Until(driver => driver.FindElement(By.Id("productLists")));

                // totalResults'ı burada try bloğunun dışında tanımlıyoruz
                

                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                int i = 0;

                // Dinamik olarak yeni ürünleri yüklemek için sayfa kaydırma
                while (true)
                {
                    i += 400;
                    js.ExecuteScript($"window.scrollTo(0, {i});");

                    // Sayfanın tam yüklenmesi için daha uzun süre bekle
                    System.Threading.Thread.Sleep(2000); // Bekleme süresini artırabilirsiniz.

                    try
                    {
                        // Her kaydırmadan sonra elementleri tekrar bul
                        var finalProductElements = driver.FindElements(By.XPath("//div[contains(@class, 'listProductItem')]"));

                        foreach (var productElement in finalProductElements)
                        {
                            // Her döngüde elementi yeniden buluyoruz, eski element referansını kullanmıyoruz
                            try
                            {
                                var productNameElement = productElement.FindElement(By.XPath(".//h3[contains(@class, 'product-item_name')]"));
                                var productName = productNameElement.Text;

                                if (!string.IsNullOrEmpty(productName) && !listBox1.Items.Contains(productName))
                                {
                                    listBox1.Items.Add(productName);
                                    label1.Text = listBox1.Items.Count.ToString();
                                }
                            }
                            catch (StaleElementReferenceException)
                            {
                                // Eğer stale element hatası alırsak, işlemi tekrar dene
                                continue;
                            }
                        }
                    }
                    catch (StaleElementReferenceException)
                    {
                        // Eğer tüm elementler kaybolursa işlemi tekrar dene
                        continue;
                    }

                    // Eğer toplam ürün sayısına ulaştıysan döngüyü kır
                    if (IsScrollAtBottom()/*listBox1.Items.Count == Convert.ToInt32(totalResults)*/)
                    {
                        break;
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
            catch (FormatException ex)
            {
                MessageBox.Show("Format hatası: " + ex.Message);
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

        private void Boyner_Load(object sender, EventArgs e)
        {

        }
        bool IsScrollAtBottom()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // Scroll pozisyonu ve sayfa yüksekliğini double olarak döndürüyoruz
            var scrollPosition = Convert.ToDouble(js.ExecuteScript("return window.scrollY + window.innerHeight;"));
            var pageHeight = Convert.ToDouble(js.ExecuteScript("return document.body.scrollHeight;"));

            // Eğer scroll en aşağıda ise true döndür
            if (scrollPosition >= pageHeight)
            {
                MessageBox.Show("Sondayız");
                return true; // Sayfa en alt kısımda
            }
            else
            {
                return false; // Sayfa henüz en alt kısımda değil
            }
        }
    }
}
