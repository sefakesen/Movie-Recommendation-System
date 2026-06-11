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
    public partial class kayitform : Form
    {
        // Normal ve hover renkleri
        Color KirmiziHover = Color.FromArgb(229, 9, 20); // Netflix kırmızısı
        Color ButonNormalRenk;

        public string KullaniciAdi { get; private set; }
        public string FavoriTur { get; private set; }
        public kayitform()
        {
            InitializeComponent();

            this.Load += kayitform_Load;

            ButonNormalRenk = this.BackColor;

            button1.BackColor = ButonNormalRenk;
            button3.BackColor = ButonNormalRenk;
            button2.BackColor = ButonNormalRenk;

            button1.FlatStyle = FlatStyle.Flat;
            button3.FlatStyle = FlatStyle.Flat;
            button2.FlatStyle = FlatStyle.Flat;

            button1.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderSize = 0;

            // Ortak hover eventleri
            button1.MouseEnter += AltButon_MouseEnter;
            button1.MouseLeave += AltButon_MouseLeave;

            button3.MouseEnter += AltButon_MouseEnter;
            button3.MouseLeave += AltButon_MouseLeave;

            button2.MouseEnter += AltButon_MouseEnter;
            button2.MouseLeave += AltButon_MouseLeave;
        }

        private void PaneliOrtala()
        {
            // panelCenter bizim ortadaki panelimizin adı
            int x = (this.ClientSize.Width - panel2.Width) / 2;
            int y = (this.ClientSize.Height - panel2.Height) / 2;

            panel2.Left = x;
            panel2.Top = y;
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



        public void SetTurler(IEnumerable<string> turler)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(
                turler
                    .Distinct()
                    .OrderBy(t => t)
                    .ToArray()
            );
        }

        private void kayitform_Load(object sender, EventArgs e)
        {
            this.AutoScaleMode = AutoScaleMode.None;

            PaneliOrtala();

        }

        private void kayitform_Resize(object sender, EventArgs e)
        {
            PaneliOrtala();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string user = textBox1.Text.Trim();
            string pass = textBox2.Text;
            string pass2 = textBox3.Text;
            string tur = comboBox1.SelectedItem != null ? comboBox1.SelectedItem.ToString() : "";

            // 1) Basit kontroller
            if (string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(pass) ||
                string.IsNullOrWhiteSpace(pass2))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre alanlarını doldurun.");
                return;
            }

            if (pass != pass2)
            {
                MessageBox.Show("Şifreler birbiriyle uyuşmuyor!");
                return;
            }

            if (string.IsNullOrWhiteSpace(tur))
            {
                MessageBox.Show("Lütfen en sevdiğiniz film türünü seçin.");
                return;
            }

            // 2) Kullanıcı adında ; kullanmasını engelle (CSV bozulmasın)
            if (user.Contains(";") || pass.Contains(";"))
            {
                MessageBox.Show("Kullanıcı adı ve şifre ';' karakteri içeremez.");
                return;
            }

            // 3) users.csv dosyasına kaydet
            string path = Path.Combine(Application.StartupPath, "users.csv");

            // Dosya yoksa önce başlık satırı ile oluştur
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "Username;Password;FavoriteGenre" + Environment.NewLine);
            }

            // 4) Aynı kullanıcı adı var mı kontrol et
            var lines = File.ReadAllLines(path).Skip(1); // başlığı atla
            bool varMi = lines.Any(line =>
            {
                var parts = line.Split(';');
                if (parts.Length < 1) return false;
                return parts[0].Equals(user, StringComparison.OrdinalIgnoreCase);
            });

            if (varMi)
            {
                MessageBox.Show("Bu kullanıcı adı zaten kayıtlı. Lütfen başka bir kullanıcı adı deneyin.");
                return;
            }

            // 5) Yeni kullanıcıyı ekle
            string row = $"{user};{pass};{tur}";
            File.AppendAllText(path, row + Environment.NewLine);

            // 6) Ana forma geri bilgi ver
            this.KullaniciAdi = user;
            this.FavoriTur = tur;

            MessageBox.Show("Kayıt başarılı! Hoş geldin, " + user + ".");

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

        private void button3_Click(object sender, EventArgs e)
        {
            using (var gf = new girisform())
            {
                // Giriş formunu modal aç
                if (gf.ShowDialog(this) == DialogResult.OK)
                {
                    // girisform başarıyla kapandıysa buraya düşer

                    // 1) kayitform’un kendi KullaniciAdi property’sine giriş yapanı yaz
                    this.KullaniciAdi = gf.KullaniciAdi;

                    // 2) Favori tür girişte bilinmiyorsa boş bırakabilirsin
                    this.FavoriTur = null;  // veya dokunma, sen bilirsin

                    // 3) kayitform da Film formuna "OK" dönsün
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}