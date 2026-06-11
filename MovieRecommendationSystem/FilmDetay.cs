using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MovieRecommendationSystem.Film; // movie sınıfı

namespace MovieRecommendationSystem
{
    public partial class FilmDetay : Form
    {
        // Normal ve hover renkleri
        Color KirmiziHover = Color.FromArgb(229, 9, 20); // Netflix kırmızısı
        Color ButonNormalRenk;

        // Ana form veya arama formu buraya filmi set edecek
        public movie SelectedMovie { get; set; }

        public FilmDetay()
        {
            InitializeComponent();

            this.Load += FilmDetay_Load;

            ButonNormalRenk = this.BackColor;
            button1.BackColor = ButonNormalRenk;
            button2.BackColor = ButonNormalRenk;

            button1.FlatStyle = FlatStyle.Flat;
            button2.FlatStyle = FlatStyle.Flat;

            button1.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;

            button1.MouseEnter += AltButon_MouseEnter;
            button1.MouseLeave += AltButon_MouseLeave;

            button2.MouseEnter += AltButon_MouseEnter;
            button2.MouseLeave += AltButon_MouseLeave;

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

        // Form açılırken çalışacak
        private void FilmDetay_Load(object sender, EventArgs e)
        {

            if (SelectedMovie == null)
            {
                label2.Text = "Film bilgisi bulunamadı";
                label3.Text = "";
                label4.Text = "";
                label5.Text = "";
                pictureBox1.Image = null;
                if (richTextBox1 != null)
                    richTextBox1.Text = "";
                return;
            }

            // Başlık
            label2.Text = SelectedMovie.Title;

            // Tür
            label3.Text = "Tür: " + (SelectedMovie.Genre ?? "-");

            // Yıl
            label4.Text = "Yıl: " + (SelectedMovie.Year == 0
                                      ? "-"
                                      : SelectedMovie.Year.ToString());

            // IMDb + Oy sayısı
            label5.Text = $"IMDb: {SelectedMovie.Rating:0.0}   Oy: {SelectedMovie.Votes}";

            // Özet
            if (richTextBox1 != null)
                richTextBox1.Text = SelectedMovie.Overview ?? "";

            // Poster
            try
            {
                if (!string.IsNullOrWhiteSpace(SelectedMovie.PosterLink))
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox1.Load(SelectedMovie.PosterLink);
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
            catch
            {
                pictureBox1.Image = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Seçili film yoksa güvenlik
    if (SelectedMovie == null)
    {
        MessageBox.Show("Önce bir film seçmelisiniz.");
        return;
    }

    // Aynı filmi iki kez eklememek için küçük bir kontrol
    bool zatenVar = Film.IzlemeListesi
        .Any(f => f.Title == SelectedMovie.Title && f.Year == SelectedMovie.Year);

    if (zatenVar)
    {
        MessageBox.Show("Bu film zaten izleme listenizde.");
        return;
    }

    // Listeye ekle
    Film.IzlemeListesi.Add(SelectedMovie);

    MessageBox.Show("Film izleme listene eklendi!");
        }

        private void button3_Click(object sender, EventArgs e)
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