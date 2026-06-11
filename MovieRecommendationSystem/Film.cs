using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MovieRecommendationSystem
{
    public partial class Film : Form
    {
        // Normal ve hover renkleri
        Color KirmiziHover = Color.FromArgb(229, 9, 20); // Netflix kırmızısı
        Color ButonNormalRenk;

        // Son önerilen filmi detay formuna göndermek için tuttuğum değişken
        movie sonSecilenFilm;

        // Oturum açmış kullanıcının adı (giriş / kayıt sonrası dolduruluyor)
        string aktifKullanici = null;

        // Kullanıcının favori türü (kayıt formundan geliyor, istersek kullanıyoruz)
        string aktifFavoriTur = null;

        // 🔴 ORTAK İZLEME LİSTESİ
        // Uygulamadaki her yer buradan izleme listesine erişecek
        public static List<movie> IzlemeListesi = new List<movie>();

        // -------------------------------------------------------------
        // 📌 Film model sınıfı: CSV'den okuduğum sütunları burada tutuyorum
        // -------------------------------------------------------------
        public class movie
        {
            public string PosterLink { get; set; }  // Filmin poster URL'si
            public string Title { get; set; }       // Filmin adı
            public int Year { get; set; }           // Çıkış yılı
            public string Genre { get; set; }       // Tür (örn: "Action, Drama")
            public double Rating { get; set; }      // IMDb puanı
            public string Actors { get; set; }      // Oyuncular (virgülle ayrılmış)
            public string Overview { get; set; }    // Kısa özet
            public int Votes { get; set; }          // IMDb oy sayısı (popülerlik için)

        }

        // Tüm filmleri bellekte tuttuğum ana liste
        List<movie> Filmler = new List<movie>();

        public Film()
        {
            InitializeComponent();

            ButonNormalRenk = this.BackColor;

            button1.BackColor = ButonNormalRenk;
            button3.BackColor = ButonNormalRenk;
            button2.BackColor = ButonNormalRenk;
            button4.BackColor = ButonNormalRenk;
            button5.BackColor = ButonNormalRenk;
            button6.BackColor = ButonNormalRenk;
            button7.BackColor = ButonNormalRenk;
            button8.BackColor = ButonNormalRenk;
            button9.BackColor = ButonNormalRenk;
            button10.BackColor = ButonNormalRenk;

            button1.FlatStyle = FlatStyle.Flat;
            button3.FlatStyle = FlatStyle.Flat;
            button2.FlatStyle = FlatStyle.Flat;
            button4.FlatStyle = FlatStyle.Flat;
            button5.FlatStyle = FlatStyle.Flat;
            button6.FlatStyle = FlatStyle.Flat;
            button7.FlatStyle = FlatStyle.Flat;
            button8.FlatStyle = FlatStyle.Flat;
            button9.FlatStyle = FlatStyle.Flat;
            button10.FlatStyle = FlatStyle.Flat;

            button1.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;
            button4.FlatAppearance.BorderSize = 0;
            button5.FlatAppearance.BorderSize = 0;
            button6.FlatAppearance.BorderSize = 0;
            button7.FlatAppearance.BorderSize = 0;
            button8.FlatAppearance.BorderSize = 0;
            button9.FlatAppearance.BorderSize = 0;
            button10.FlatAppearance.BorderSize = 0;

            // Ortak hover eventleri
            button1.MouseEnter += AltButon_MouseEnter;
            button1.MouseLeave += AltButon_MouseLeave;

            button3.MouseEnter += AltButon_MouseEnter;
            button3.MouseLeave += AltButon_MouseLeave;

            button2.MouseEnter += AltButon_MouseEnter;
            button2.MouseLeave += AltButon_MouseLeave;

            button4.MouseEnter += AltButon_MouseEnter;
            button4.MouseLeave += AltButon_MouseLeave;

            button5.MouseEnter += AltButon_MouseEnter;
            button5.MouseLeave += AltButon_MouseLeave;

            button6.MouseEnter += AltButon_MouseEnter;
            button6.MouseLeave += AltButon_MouseLeave;

            button7.MouseEnter += AltButon_MouseEnter;
            button7.MouseLeave += AltButon_MouseLeave;

            button8.MouseEnter += AltButon_MouseEnter;
            button8.MouseLeave += AltButon_MouseLeave;

            button9.MouseEnter += AltButon_MouseEnter;
            button9.MouseLeave += AltButon_MouseLeave;

            button10.MouseEnter += AltButon_MouseEnter;
            button10.MouseLeave += AltButon_MouseLeave;
        }

        private void AltButon_MouseEnter(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = KirmiziHover;
                btn.ForeColor = Color.White;
            }
        }

        private void AltButon_MouseLeave(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                btn.BackColor = ButonNormalRenk;
                btn.ForeColor = Color.White; // yazılar beyaz kalsın
            }
        }

        // -------------------------------------------------------------
        // 👤 Kullanıcı arayüzünü güncelle:
        //    - Misafir ise: Kayıt butonu açık, Profil gizli
        //    - Giriş yaptıysa: Profil açık, Kayıt gizli
        // -------------------------------------------------------------
        private void GuncelleKullaniciArayuzu()
        {
            if (string.IsNullOrEmpty(aktifKullanici))
            {
                // HENÜZ GİRİŞ YOK → Sadece Kayıt butonu gözüksün
                button7.Visible = true;   // Kayıt Formu
                button8.Visible = false;  // Profil (şimdilik gizli)

                label8.Text = "Misafir";
            }
            else
            {
                // GİRİŞ YAPILDI → Profil gözüksün, kayıt butonu gizlensin
                button7.Visible = false;
                button8.Visible = true;

                label8.Text = "Hoş geldin, " + aktifKullanici;
            }
        }

        // -------------------------------------------------------------
        // 🎬 Seçilen filmi ana ekranda gösteren fonksiyon
        // -------------------------------------------------------------
        private void FilmGöster(movie film)
        {
            // Film bilgilerini label içine yazdırıyorum
            label3.Text =
                $"Film: {film.Title}\n" +
                $"Tür: {film.Genre}\n" +
                $"Yıl: {film.Year}\n" +
                $"IMDb: {film.Rating}\n" +
                $"Oyuncular: {film.Actors}";

            // Poster URL'sinden resmi PictureBox'a yüklüyorum
            try
            {
                pictureBox1.Load(film.PosterLink);
            }
            catch
            {
                // Poster yüklenemezse boş bırakıyorum
                pictureBox1.Image = null;
            }

            // Detay formunda kullanmak için son seçilen filmi saklıyorum
            sonSecilenFilm = film;
        }

        // -------------------------------------------------------------
        // 📂 CSV'DEN FİLM VERİLERİNİ OKUMA
        // imdb_top_1000.csv dosyasını TextFieldParser ile okuyorum
        // -------------------------------------------------------------
        private void CSVYukle()
        {
            // Uygulama klasöründeki CSV yolunu oluşturuyorum
            string path = Path.Combine(Application.StartupPath, "imdb_top_1000.csv");

            Filmler = new List<movie>();

            using (TextFieldParser parser = new TextFieldParser(path))
            {
                // CSV formatını ayarlıyorum
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true; // Alanlar tırnak içinde olabilir

                bool first = true; // İlk satır başlık olduğu için atlayacağım

                while (!parser.EndOfData)
                {
                    var p = parser.ReadFields();

                    if (first)
                    {
                        // İlk satır: kolon isimleri → atla
                        first = false;
                        continue;
                    }

                    // Güvenlik: beklediğimizden az kolon olursa satırı atla
                    if (p.Length < 16)
                        continue;

                    // Küçük bir temizleme fonksiyonu (satır sonu vs. silmek için)
                    string clean(string s)
                    {
                        if (s == null) return "";
                        return s.Replace("\r", "")
                                .Replace("\n", "")
                                .Trim();
                    }

                    // Yıl bilgisini parse et
                    int year = 0;
                    int.TryParse(clean(p[2]), out year);

                    // IMDb puanını parse et (virgül/nokta uyumu için Replace kullanıyorum)
                    double rating = 0;
                    double.TryParse(
                        clean(p[6]).Replace(",", "."),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out rating);

                    // Tür bilgisini temizle
                    string genre = clean(p[5]);

                    // Oyuncuları birleştir ve fazlalıkları temizle
                    string actors = $"{p[10]}, {p[11]}, {p[12]}, {p[13]}";
                    actors = string.Join(",",
                        actors.Split(',')
                              .Select(a => clean(a))
                              .Where(a => a.Length > 0));

                    // No_of_Votes (popülerlik için kullanacağım)
                    int votes = 0;
                    int.TryParse(clean(p[14]), out votes);

                    // Okunan satırdan bir movie nesnesi oluşturup listeye ekliyorum
                    Filmler.Add(new movie
                    {
                        PosterLink = clean(p[0]),
                        Title = clean(p[1]),
                        Year = year,
                        Genre = genre,
                        Rating = rating,
                        Overview = clean(p[7]),
                        Actors = actors,
                        Votes = votes
                    });
                }
            }
        }

        // -------------------------------------------------------------
        // 📌 (Şimdilik kullanmıyorum ama dursun) Tek satır CSV parse fonksiyonu
        // -------------------------------------------------------------
        public static string[] SplitCsvLine(string line)
        {
            List<string> result = new List<string>();
            bool insideQuotes = false;
            string current = "";

            foreach (char c in line)
            {
                if (c == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (c == ',' && !insideQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            result.Add(current);
            return result.ToArray();
        }

        // -------------------------------------------------------------
        // 🎯 COMBOBOXLARI DOLDURMA
        // Tür, Yıl, IMDb ve Aktör listelerini comboboxlara basıyorum
        // -------------------------------------------------------------
        private void ComboDoldur()
        {
            // Önce tüm comboboxları temizliyorum
            comboBox1.Items.Clear(); // Tür
            comboBox2.Items.Clear(); // Yıl
            comboBox3.Items.Clear(); // IMDb
            comboBox4.Items.Clear(); // Aktör

            // =======================
            // ⭐ 1) TÜRLER (comboBox1)
            // =======================
            var turSet = new HashSet<string>();

            foreach (var f in Filmler)
            {
                if (string.IsNullOrWhiteSpace(f.Genre))
                    continue;

                var turler = f.Genre
                    .Split(',')              // "Action, Drama"
                    .Select(t => t.Trim());  // "Action" / "Drama"

                foreach (var t in turler)
                {
                    if (!string.IsNullOrEmpty(t))
                        turSet.Add(t);
                }
            }

            comboBox1.Items.AddRange(turSet
                .OrderBy(t => t)
                .ToArray());

            // =======================
            // ⭐ 2) YILLAR (comboBox2)
            // =======================
            var yilList = Filmler
                .Where(f => f.Year > 0)               // 0 olmayan yıllar
                .Select(f => f.Year.ToString())
                .Distinct()
                .OrderBy(y => y)
                .ToArray();

            comboBox2.Items.AddRange(yilList);

            // ==========================
            // ⭐ 3) IMDb (comboBox3) → 1'den 9'a kadar
            // ==========================
            for (int i = 1; i <= 9; i++)
            {
                comboBox3.Items.Add(i.ToString());
            }

            // ==========================
            // ⭐ 4) AKTÖRLER (comboBox4)
            // ==========================
            var aktorSet = new HashSet<string>();

            foreach (var f in Filmler)
            {
                if (string.IsNullOrWhiteSpace(f.Actors))
                    continue;

                var aktorler = f.Actors
                    .Split(',')               // "Brad Pitt, Morgan Freeman"
                    .Select(a => a.Trim());   // "Brad Pitt" / "Morgan Freeman"

                foreach (var a in aktorler)
                {
                    if (!string.IsNullOrEmpty(a))
                        aktorSet.Add(a);
                }
            }

            comboBox4.Items.AddRange(aktorSet
                .OrderBy(a => a)
                .ToArray());
        }

        // -------------------------------------------------------------
        // 🔄 FORM YÜKLENDİĞİNDE ÇALIŞAN KOD
        // CSV yükleme, combobox doldurma, buton stilleri vs.
        // -------------------------------------------------------------
        private void Form1_Load(object sender, EventArgs e)
        {
            CSVYukle();     // Filmleri CSV'den oku
            ComboDoldur();  // Comboboxları doldur

            // Uygulama ilk açıldığında kullanıcı yok → Misafir modu
            GuncelleKullaniciArayuzu();
        }

        // -------------------------------------------------------------
        // 🎯 BUTON 3 → Kullanıcının seçtiği filtrelere göre film öner
        // -------------------------------------------------------------
        private void button3_Click(object sender, EventArgs e)
        {
            // 1) Comboboxlardan seçilen değerleri alıyorum
            string secilenTur = comboBox1.Text.Trim();   // Ör: "Drama"
            string secilenYil = comboBox2.Text.Trim();   // Ör: "2000"
            string secilenImdb = comboBox3.Text.Trim();  // Ör: "8"
            string secilenAktor = comboBox4.Text.Trim(); // Ör: "Morgan Freeman"

            // 2) Yıl ve IMDb değerlerini sayıya çeviriyorum (başarısız olursa 0 kalır)
            int yilDeger = 0;
            int.TryParse(secilenYil, out yilDeger);

            double imdbDeger = 0;
            double.TryParse(
                secilenImdb.Replace(",", "."),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out imdbDeger);

            // 3) Başlangıçta tüm filmler uygun kabul ediliyor
            List<movie> uygun = Filmler.ToList();

            // ⭐ TÜR FİLTRESİ
            if (!string.IsNullOrWhiteSpace(secilenTur))
            {
                string turKucuk = secilenTur.ToLower();

                uygun = uygun
                    .Where(f =>
                        !string.IsNullOrWhiteSpace(f.Genre) &&
                        f.Genre
                            .ToLower()
                            .Split(',')            // "Action, Adventure, Drama"
                            .Select(t => t.Trim()) // ["action","adventure","drama"]
                            .Contains(turKucuk))   // seçilen tür bu listede mi?
                    .ToList();
            }

            // ⭐ YIL FİLTRESİ
            if (yilDeger != 0)
            {
                uygun = uygun
                    .Where(f => f.Year == yilDeger)
                    .ToList();
            }

            // ⭐ IMDb FİLTRESİ (seçilen puanın üzerinde film)
            if (imdbDeger != 0)
            {
                uygun = uygun
                    .Where(f => f.Rating >= imdbDeger)
                    .ToList();
            }

            // ⭐ AKTÖR FİLTRESİ
            if (!string.IsNullOrWhiteSpace(secilenAktor))
            {
                string aktorKucuk = secilenAktor.ToLower();
                uygun = uygun
                    .Where(f =>
                        !string.IsNullOrWhiteSpace(f.Actors) &&
                        f.Actors.ToLower().Contains(aktorKucuk))
                    .ToList();
            }

            // 4) Filtre sonrası hiç film kalmadıysa kullanıcıya bilgi ver
            if (uygun.Count == 0)
            {
                MessageBox.Show("Seçtiğiniz kriterlere uygun film bulunamadı.");
                return;
            }

            // 5) Uygun filmler arasından rastgele bir tanesini seçiyorum
            Random rnd = new Random();
            movie secilen = uygun[rnd.Next(uygun.Count)];

            // 6) Seçilen filmi ekranda göster
            FilmGöster(secilen);
        }

        // -------------------------------------------------------------
        // 🔎 ARAMA BUTONU → Arama sonuç formunu aç
        // -------------------------------------------------------------
        private void button2_Click(object sender, EventArgs e)
        {
            aramasonuc frm = new aramasonuc();
            frm.aramaMetni = textBox1.Text; // kullanıcı ne yazdıysa onu gönderiyorum
            frm.Filmler = Filmler;          // tüm film listesini iletiyorum
            frm.Show();
            this.Hide();
        }
        

        // -------------------------------------------------------------
        // ❤️ FAVORİLER SAYFASINA GEÇ
        // -------------------------------------------------------------
        private void button4_Click(object sender, EventArgs e)
        {
            var profilForm = new kullanici();

            // Kullanıcı adını gönder
            profilForm.KullaniciAdi = aktifKullanici;

            // 🔴 BURASI ÖNEMLİ:
            // Profil formuna ortak izleme listesini veriyoruz
            profilForm.IzlemeListesi = Film.IzlemeListesi;

            profilForm.ShowDialog(this);
        }

        // -------------------------------------------------------------
        // 🎬 TAMAMEN RASTGELE FİLM ÖNER (Hiç filtre yokken kullanılabilir)
        // -------------------------------------------------------------
        private void button5_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            var film = Filmler[rnd.Next(Filmler.Count)];
            FilmGöster(film);
        }

        // -------------------------------------------------------------
        // 🧹 TEMİZLE BUTONU → Filtreleri ve filmi sıfırla
        // -------------------------------------------------------------
        private void button6_Click(object sender, EventArgs e)
        {
            // Combobox seçimlerini sıfırla
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;

            // İçlerine yazılmış bir şey varsa onları da temizle
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox4.Text = "";

            // Ekrandaki film bilgisini temizle
            label3.Text = "";
            pictureBox1.Image = null;

            // Kod tarafında da son seçilen filmi sıfırla
            sonSecilenFilm = null;
        }

        // -------------------------------------------------------------
        // 📝 KAYIT OL BUTONU → Kayıt formunu aç, kullanıcıyı al, arayüzü güncelle
        // -------------------------------------------------------------
        private void button7_Click(object sender, EventArgs e)
        {
            // Tüm filmlerdeki türlerden tekil bir liste oluşturuyorum
            var turler = Filmler
                .Where(f => !string.IsNullOrWhiteSpace(f.Genre))
                .SelectMany(f => f.Genre.Split(','))
                .Select(t => t.Trim())
                .Where(t => t.Length > 0)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            using (kayitform frm = new kayitform())
            {
                // Türleri kayıt formuna gönderiyorum (ComboBox için)
                frm.SetTurler(turler);

                // Kayıt formunu modal olarak aç
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    // Kayıt başarılı olduğunda kayıt formundan kullanıcı adını ve favori türü alıyorum
                    string ad = frm.KullaniciAdi;
                    string tur = frm.FavoriTur;

                    // Aktif kullanıcıyı ayarla
                    aktifKullanici = ad;
                    aktifFavoriTur = tur;

                    // Butonları ve kullanıcı label'ını güncelle
                    GuncelleKullaniciArayuzu();

                    // Kullanıcının favori türü ana formda da combobox'ta otomatik seçilsin
                    if (!string.IsNullOrEmpty(tur))
                    {
                        int index = comboBox1.Items.IndexOf(tur);
                        if (index >= 0)
                            comboBox1.SelectedIndex = index;
                    }
                }
            }
        }

        // -------------------------------------------------------------
        // 👤 PROFİL BUTONU → Profil formunu aç
        // -------------------------------------------------------------
        private void button8_Click(object sender, EventArgs e)
        {
            var profilForm = new kullanici();

            // Kullanıcı adını gönder
            profilForm.KullaniciAdi = aktifKullanici;

            // 🔴 BURASI ÖNEMLİ:
            // Profil formuna ortak izleme listesini veriyoruz
            profilForm.IzlemeListesi = Film.IzlemeListesi;

            profilForm.ShowDialog(this);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // 1) Önce ekranda bir film var mı kontrol et
            if (sonSecilenFilm == null)
            {
                MessageBox.Show("Önce bir film seçmelisin. (Öner veya Rastgele butonunu kullan)");
                return;
            }

            // 2) Bu film daha önce izleme listesine eklenmiş mi?
            //    Basitçe Title'a göre kontrol ediyorum
            bool zatenVar = IzlemeListesi.Any(f => f.Title == sonSecilenFilm.Title);

            if (zatenVar)
            {
                MessageBox.Show("Bu film zaten izleme listende var.");
                return;
            }

            // 3) Listeye ekle
            IzlemeListesi.Add(sonSecilenFilm);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sonSecilenFilm == null)
            {
                MessageBox.Show("Önce bir film önerin!");
                return;
            }

            // Detay formunu aç ve filmi gönder
            FilmDetay frm = new FilmDetay();
            frm.SelectedMovie = sonSecilenFilm;
            frm.Show();
        }
    }
}
