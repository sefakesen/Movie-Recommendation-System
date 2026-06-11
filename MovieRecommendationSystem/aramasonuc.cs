using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static MovieRecommendationSystem.Film; // movie sınıfı

namespace MovieRecommendationSystem
{
    public partial class aramasonuc : Form
    {
        string aktifKullanici = null;

        // Normal ve hover renkleri
        Color KirmiziHover = Color.FromArgb(229, 9, 20); // Netflix kırmızısı
        Color ButonNormalRenk;

        public string aramaMetni { get; set; }
        public List<movie> Filmler { get; set; }

        // ARAMA SONUCU (ekranda gösterilen)
        private List<movie> AramaSonuclari = new List<movie>();

        // ARAMANIN HAM HALİ (istersen ileride "Sıfırla" için kullanırsın)
        private List<movie> TemelSonuc = new List<movie>();

        // Kolon sıralama takibi (şu an kullanmıyoruz ama dursun)
        private bool sonSiralamaArtan = true;
        private string sonSiralamaKolon = "";

        public aramasonuc()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal;
            this.Opacity = 1;
            this.ShowInTaskbar = true;

            ButonNormalRenk = this.BackColor;

            button1.BackColor = ButonNormalRenk;
            button3.BackColor = ButonNormalRenk;
            button2.BackColor = ButonNormalRenk;
            button4.BackColor = ButonNormalRenk;
            button5.BackColor = ButonNormalRenk;
            button6.BackColor = ButonNormalRenk;

            button1.FlatStyle = FlatStyle.Flat;
            button3.FlatStyle = FlatStyle.Flat;
            button2.FlatStyle = FlatStyle.Flat;
            button4.FlatStyle = FlatStyle.Flat;
            button5.FlatStyle = FlatStyle.Flat;
            button6.FlatStyle = FlatStyle.Flat;


            button1.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;
            button4.FlatAppearance.BorderSize = 0;
            button5.FlatAppearance.BorderSize = 0;
            button6.FlatAppearance.BorderSize = 0;

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

        // -------------------------------------------------
        //  Netflix tarzı film kartlarını çizen fonksiyon
        // -------------------------------------------------
        private void KartlariCiz(List<movie> liste = null)
        {
            // Parametre verilmezse mevcut arama sonuçlarını kullan
            var kaynak = liste ?? AramaSonuclari;

            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.Controls.Clear();

            foreach (var film in kaynak)
            {
                // --- Kart paneli ---
                Panel card = new Panel();
                card.Width = 170;
                card.Height = 260;
                card.Margin = new Padding(10);
                card.BackColor = Color.FromArgb(30, 30, 30);
                card.Cursor = Cursors.Hand;
                card.Tag = film; // hangi filme ait olduğunu tut

                // --- Poster ---
                PictureBox pic = new PictureBox();
                pic.Dock = DockStyle.Top;
                pic.Height = 210;
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.BackColor = Color.Black;
                pic.Tag = film;

                try
                {
                    // Poster'i asenkron yükle (form kilitlenmesin)
                    pic.LoadAsync(film.PosterLink);
                }
                catch
                {
                    // Yüklenemezse siyah kalsın
                }

                // --- Başlık + IMDb ---
                Label lbl = new Label();
                lbl.Dock = DockStyle.Fill;
                lbl.ForeColor = Color.White;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                lbl.Padding = new Padding(3);
                lbl.Tag = film;
                lbl.AutoEllipsis = true;

                lbl.Text = film.Title + Environment.NewLine +
                           $"IMDb: {film.Rating:0.0}";

                // ---- Ortak tıklama olayı → detay formu aç ----
                EventHandler clickHandler = (s, e) =>
                {
                    var ctrl = (Control)s;
                    var secilenFilm = (movie)ctrl.Tag;

                    FilmDetay frm = new FilmDetay();
                    frm.SelectedMovie = secilenFilm;
                    frm.Show();
                };

                card.Click += clickHandler;
                pic.Click += clickHandler;
                lbl.Click += clickHandler;

                // Kartın içine poster ve label'ı ekle
                card.Controls.Add(lbl);
                card.Controls.Add(pic);

                // Kartı flowLayoutPanel'e ekle
                flowLayoutPanel1.Controls.Add(card);
            }

            flowLayoutPanel1.ResumeLayout();
        }

        private void GuncelleKullaniciArayuzu()
        {
            if (string.IsNullOrEmpty(aktifKullanici))
            {
                // HENÜZ GİRİŞ YOK → Sadece Kayıt butonu gözüksün
                button6.Visible = false;  // Profil (şimdilik gizli)
            }
            else
            {
                // GİRİŞ YAPILDI → Profil gözüksün, kayıt butonu gizlensin
                button6.Visible = true;
            }
        }

        // -------------------------------------------------
        // FORM LOAD → Arama + TemelSonuc ve AramaSonuclari doldur
        // -------------------------------------------------
        private void aramasonuc_Load(object sender, EventArgs e)
        {
            GuncelleKullaniciArayuzu();

            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.Activate();

            // Güvenlik: ana form film listesini gönderdi mi?
            if (Filmler == null || Filmler.Count == 0)
            {
                MessageBox.Show("Film listesi alınamadı.");
                return;
            }

            string key = (aramaMetni ?? "").ToLower();

            var sonuc = Filmler.Where(f =>
                (f.Title ?? "").ToLower().Contains(key) ||
                (f.Genre ?? "").ToLower().Contains(key) ||
                f.Year.ToString().Contains(key) ||
                (f.Actors ?? "").ToLower().Contains(key)
            ).ToList();

            AramaSonuclari = sonuc;
            TemelSonuc = sonuc.ToList(); // yedeğini tut (ileride lazım olabilir)

            if (AramaSonuclari.Count == 0)
            {
                MessageBox.Show("Aradığınız kriterlere uygun film bulunamadı.");
                return;
            }

            // Netflix tarzı kartları çiz
            KartlariCiz();
        }

        // -------------------------------------------------
        // BUTON 1 → En yüksek puanlı (IMDb DESC)
        // -------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            if (AramaSonuclari == null || AramaSonuclari.Count == 0)
                return;

            AramaSonuclari = AramaSonuclari
                .OrderByDescending(f => f.Rating)
                .ToList();

            KartlariCiz();
        }

        // -------------------------------------------------
        // BUTON 2 → En popüler (Votes DESC)
        // -------------------------------------------------
        private void button2_Click(object sender, EventArgs e)
        {
            if (AramaSonuclari == null || AramaSonuclari.Count == 0)
                return;

            AramaSonuclari = AramaSonuclari
                .OrderByDescending(f => f.Votes)
                .ToList();

            KartlariCiz();
        }

        // -------------------------------------------------
        // BUTON 3 → En düşük puanlı (IMDb ASC)
        // -------------------------------------------------
        private void button3_Click(object sender, EventArgs e)
        {
            if (AramaSonuclari == null || AramaSonuclari.Count == 0)
                return;

            AramaSonuclari = AramaSonuclari
                .OrderBy(f => f.Rating)
                .ToList();

            KartlariCiz();
        }

        // -------------------------------------------------
        // BUTON 4 (Filtreyi Uygula) → IMDb & Votes filtresi
        //  checkBox1 / numericUpDown1 → IMDb alt sınırı
        //  checkBox2 / numericUpDown2 → Oy sayısı alt sınırı
        // -------------------------------------------------
        private void button4_Click(object sender, EventArgs e)
        {
            if (AramaSonuclari == null || AramaSonuclari.Count == 0)
                return;

            IEnumerable<movie> liste = AramaSonuclari;

            if (checkBox1.Checked)
            {
                double minImdb = (double)numericUpDown1.Value;
                liste = liste.Where(f => f.Rating >= minImdb);
            }

            if (checkBox2.Checked)
            {
                int minOy = (int)numericUpDown2.Value;
                liste = liste.Where(f => f.Votes >= minOy);
            }

            KartlariCiz(liste.ToList());
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var profilForm = new kullanici();

            // 🔴 BURASI ÖNEMLİ:
            // Profil formuna ortak izleme listesini veriyoruz
            profilForm.IzlemeListesi = Film.IzlemeListesi;

            profilForm.ShowDialog(this);

            GuncelleKullaniciArayuzu();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            var anaForm = Application.OpenForms["Film"];
            if (anaForm != null)
            {
                anaForm.Show();       // Ana formu tekrar göster
            }

            this.Close();
        }
    }
}
