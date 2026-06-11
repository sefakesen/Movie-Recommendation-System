using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MovieRecommendationSystem.Film;

namespace MovieRecommendationSystem
{

    public partial class kullanici : Form
    {
        Color KirmiziNormal = Color.FromArgb(25, 25, 25);
        Color KirmiziHover = Color.FromArgb(255, 40, 40);

        // Ana formdan doldurulacak: kullanıcı adı
        public string KullaniciAdi { get; set; }

        // Ana formdan doldurulacak: izleme listesi (movie listesi)
        public List<movie> IzlemeListesi { get; set; } = new List<movie>();

        public kullanici()
        {
            InitializeComponent();
        }

        private void kullanici_Load(object sender, EventArgs e)
        {
            // Üstteki "Merhaba, ..." yazısını ayarla
            if (string.IsNullOrEmpty(KullaniciAdi))
                label1.Text = "Merhaba, Misafir";
            else
                label1.Text = "Merhaba, " + KullaniciAdi;

            // İzleme listesi sayısını yaz
            label3.Text = $"İzleme listende {IzlemeListesi.Count} film var";

            // Kartları çiz
            IzlemeKartlariniDoldur();

            button1.BackColor = KirmiziNormal;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;

            button1.MouseEnter += btnKapat_MouseEnter;
            button1.MouseLeave += btnKapat_MouseLeave;
        }

        private void btnKapat_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = KirmiziHover;
        }

        private void btnKapat_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = KirmiziNormal;
        }


        private void IzlemeKartlariniDoldur()
        {
            flowLayoutPanel1.Controls.Clear();

            // Henüz film yoksa basit bir mesaj gösterelim
            if (IzlemeListesi == null || IzlemeListesi.Count == 0)
            {
                Label bosLabel = new Label();
                bosLabel.Text = "Henüz izleme listene film eklemedin.";
                bosLabel.ForeColor = Color.Gray;
                bosLabel.Font = new Font("Segoe UI", 11, FontStyle.Italic);
                bosLabel.AutoSize = true;
                bosLabel.Margin = new Padding(20);

                flowLayoutPanel1.Controls.Add(bosLabel);
                return;
            }

            // Her film için bir "kart" panel oluştur
            foreach (var film in IzlemeListesi)
            {
                Panel kart = new Panel();
                kart.Width = 200;
                kart.Height = 280;
                kart.BackColor = Color.FromArgb(35, 35, 35);
                kart.Margin = new Padding(10);
                kart.Padding = new Padding(5);

                // Poster
                PictureBox pic = new PictureBox();
                pic.Width = 190;
                pic.Height = 190;
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Top = 5;
                pic.Left = 5;

                try
                {
                    if (!string.IsNullOrWhiteSpace(film.PosterLink))
                        pic.Load(film.PosterLink);
                }
                catch
                {
                    // Poster yüklenemezse boş bırak
                }

                // Başlık
                Label lblBaslik = new Label();
                lblBaslik.Text = film.Title;
                lblBaslik.ForeColor = Color.White;
                lblBaslik.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                lblBaslik.AutoSize = false;
                lblBaslik.Width = 190;
                lblBaslik.Height = 35;
                lblBaslik.Top = pic.Bottom + 5;
                lblBaslik.Left = 5;
                lblBaslik.TextAlign = ContentAlignment.TopLeft;

                // Yıl + IMDb
                Label lblAlt = new Label();
                lblAlt.Text = $"{film.Year}  •  IMDb: {film.Rating}";
                lblAlt.ForeColor = Color.Gainsboro;
                lblAlt.Font = new Font("Segoe UI", 8, FontStyle.Regular);
                lblAlt.AutoSize = false;
                lblAlt.Width = 190;
                lblAlt.Height = 20;
                lblAlt.Top = lblBaslik.Bottom + 2;
                lblAlt.Left = 5;

                // Kartın içine ekle
                kart.Controls.Add(pic);
                kart.Controls.Add(lblBaslik);
                kart.Controls.Add(lblAlt);

                // Kartı flowLayoutPanel içine ekle
                flowLayoutPanel1.Controls.Add(kart);

                // İstersen kartlara tıklama olayı ekleyebilirsin:
                // kart.Tag = film;
                // kart.Click += (s, e) => { ... detay formu aç ... };
            }
        }

        private void button1_Click(object sender, EventArgs e)
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