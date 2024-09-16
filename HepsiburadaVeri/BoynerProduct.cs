using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HepsiburadaVeri
{
    public struct BoynerProducts
    {
        public string MarkaAdi { get; set; }
        public string UrunAdi { get; set; }
        public string UrunLink { get; set; }
        public double IndirimsizFiyat { get; set; }
        public double IndirimOrani { get; set; }
        public double IndirimliFiyat { get; set; }
        public int TotalDegerlendirmeSayisi { get; set; }
        public int SonBirAyDegerlendirmeSayisi { get; set; }
    }
    public struct BoynerCategory
    {
        public string KategoryLinki{ get; set; }//LOC
        public DateTime LastMod{ get; set; }
        public string changeFreq{ get; set; }
        public string priority { get; set; }
    }
}
