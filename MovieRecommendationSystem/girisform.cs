using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieRecommendationSystem
{
    public partial class girisform : Form
    {
        // Normal ve hover renkleri
        Color KirmiziHover = Color.FromArgb(229, 9, 20); // Netflix kırmızısı
        Color ButonNormalRenk;

        public string KullaniciAdi { get; private set; }
        public string FavoriTur { get; private set; }
        public girisform()
        {
            InitializeComponent();

            ButonNormalRenk = this.BackColor;

            button1.BackColor = ButonNormalRenk;
            button2.BackColor = ButonNormalRenk;

            button1.FlatStyle = FlatStyle.Flat;
            button2.FlatStyle = FlatStyle.Flat;

            button1.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;

            // Ortak hover eventleri
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

        private void girisform_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string kullanici = textBox1.Text.Trim();
            string sifre = textBox2.Text.Trim();

            // Burada kendi doğrulamanı yap (dosya, db vs.)
            // Ben basit bir kontrol bırakıyorum:
            if (string.IsNullOrEmpty(kullanici) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Kullanıcı adı ve şifre boş olamaz.");
                return;
            }

            // TODO: burada gerçekten kayıtlı mı kontrol etmen lazım

            // Giriş başarılı → bu form üzerinden kullanıcıyı yukarı taşıyoruz
            this.KullaniciAdi = kullanici;

            // Üst forma “her şey yolunda” demek için:
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
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
