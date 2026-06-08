using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Drawing.Imaging;
using Guna.UI2.WinForms;

namespace SteganografiApp
{
    public partial class Form1 : Form
    {
        private Guna2Button btnPilih, btnEncode, btnDecode;
        private Guna2TextBox txtPesan;
        private string selectedImagePath = "";

        public Form1()
        {
            this.Text = "SteganoPro - LSB Tool (Fixed)";
            this.Size = new Size(500, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(28, 30, 40);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            btnPilih = CreateModernButton("Pilih Gambar (.png)", new Point(150, 30), Color.FromArgb(94, 148, 255));
            btnPilih.Click += (s, e) => {
                OpenFileDialog ofd = new OpenFileDialog { Filter = "PNG Files|*.png" };
                if (ofd.ShowDialog() == DialogResult.OK) {
                    selectedImagePath = ofd.FileName;
                    MessageBox.Show("Gambar berhasil dimuat!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            txtPesan = new Guna2TextBox { 
                Location = new Point(50, 100), 
                Size = new Size(400, 120), 
                Multiline = true,
                PlaceholderText = "Ketik pesan untuk di-Encode, atau biarkan kosong untuk melihat hasil Decode...",
                BorderRadius = 10, 
                FillColor = Color.FromArgb(40, 42, 52), 
                ForeColor = Color.White,
                BorderColor = Color.FromArgb(70, 70, 80),
                Font = new Font("Segoe UI", 10)
            };

            btnEncode = CreateModernButton("Sembunyikan (Encode)", new Point(50, 260), Color.FromArgb(46, 204, 113));
            btnEncode.Click += (s, e) => ProcessAction(true);

            btnDecode = CreateModernButton("Ekstrak (Decode)", new Point(270, 260), Color.FromArgb(231, 76, 60));
            btnDecode.Click += (s, e) => ProcessAction(false);

            this.Controls.AddRange(new Control[] { btnPilih, txtPesan, btnEncode, btnDecode });
        }

        private Guna2Button CreateModernButton(string text, Point location, Color color)
        {
            return new Guna2Button { 
                Text = text, Location = location, Size = new Size(180, 45), 
                FillColor = color, BorderRadius = 12, ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold), Animated = true
            };
        }

        private void ProcessAction(bool isEncode)
        {
            if (string.IsNullOrEmpty(selectedImagePath)) { 
                MessageBox.Show("Silakan pilih gambar terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                return; 
            }
            
            try
            {
                if (isEncode) {
                    if (string.IsNullOrEmpty(txtPesan.Text)) {
                        MessageBox.Show("Isi pesan teks terlebih dahulu sebelum melakukan Encode!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    SaveFileDialog sfd = new SaveFileDialog { Filter = "PNG Image|*.png" };
                    if (sfd.ShowDialog() == DialogResult.OK) {
                        EmbedText(selectedImagePath, sfd.FileName, txtPesan.Text);
                        MessageBox.Show("Pesan berhasil disembunyikan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                } else {
                    string hasilDecode = ExtractText(selectedImagePath);
                    txtPesan.Text = hasilDecode;
                    MessageBox.Show("Proses ekstraksi selesai!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- FIXED LOGIKA LSB STEGANOGRAFI ---
        private void EmbedText(string inputPath, string outputPath, string text)
        {
            Bitmap bmp = new Bitmap(inputPath);
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] lenBytes = BitConverter.GetBytes(textBytes.Length); // 4 bytes (32 bits) untuk panjang pesan
            
            // Satukan panjang pesan dan isi teks ke dalam satu payload
            byte[] payload = new byte[lenBytes.Length + textBytes.Length];
            Array.Copy(lenBytes, 0, payload, 0, 4);
            Array.Copy(textBytes, 0, payload, 4, textBytes.Length);

            int bitIndex = 0;
            int totalBits = payload.Length * 8;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (bitIndex >= totalBits) break;

                    Color p = bmp.GetPixel(x, y);
                    int r = p.R, g = p.G, b = p.B;

                    // Sisipkan bit ke R, G, B secara berurutan
                    if (bitIndex < totalBits) { r = (r & ~1) | ((payload[bitIndex / 8] >> (bitIndex % 8)) & 1); bitIndex++; }
                    if (bitIndex < totalBits) { g = (g & ~1) | ((payload[bitIndex / 8] >> (bitIndex % 8)) & 1); bitIndex++; }
                    if (bitIndex < totalBits) { b = (b & ~1) | ((payload[bitIndex / 8] >> (bitIndex % 8)) & 1); bitIndex++; }

                    bmp.SetPixel(x, y, Color.FromArgb(p.A, r, g, b));
                }
                if (bitIndex >= totalBits) break;
            }
            bmp.Save(outputPath, ImageFormat.Png);
            bmp.Dispose();
        }

        private string ExtractText(string imagePath)
        {
            Bitmap bmp = new Bitmap(imagePath);
            
            // Tahap 1: Ambil 32 bit pertama untuk tahu panjang teks (4 byte)
            byte[] lenBytes = new byte[4];
            int bitIndex = 0;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (bitIndex >= 32) break;

                    Color p = bmp.GetPixel(x, y);
                    if (bitIndex < 32) { lenBytes[bitIndex / 8] |= (byte)((p.R & 1) << (bitIndex % 8)); bitIndex++; }
                    if (bitIndex < 32) { lenBytes[bitIndex / 8] |= (byte)((p.G & 1) << (bitIndex % 8)); bitIndex++; }
                    if (bitIndex < 32) { lenBytes[bitIndex / 8] |= (byte)((p.B & 1) << (bitIndex % 8)); bitIndex++; }
                }
                if (bitIndex >= 32) break;
            }

            int messageLength = BitConverter.ToInt32(lenBytes, 0);
            
            // Validasi ukuran agar memori tidak crash jika gambar kosong/tidak ada pesan
            if (messageLength <= 0 || messageLength > (bmp.Width * bmp.Height * 3 / 8)) {
                bmp.Dispose();
                return "[Sistem] Tidak ditemukan pesan valid atau gambar ini bukan gambar LSB steganografi.";
            }

            // Tahap 2: Ambil byte teks berdasarkan ukuran yang sudah didapat
            byte[] textBytes = new byte[messageLength];
            int totalBits = (messageLength + 4) * 8;
            bitIndex = 0; // Reset counter bit total untuk mempermudah sinkronisasi posisi

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (bitIndex >= totalBits) break;

                    Color p = bmp.GetPixel(x, y);
                    int[] channels = { p.R, p.G, p.B };

                    foreach (int channel in channels)
                    {
                        if (bitIndex >= 32 && bitIndex < totalBits)
                        {
                            int textBitIndex = bitIndex - 32;
                            textBytes[textBitIndex / 8] |= (byte)((channel & 1) << (textBitIndex % 8));
                        }
                        bitIndex++;
                    }
                }
                if (bitIndex >= totalBits) break;
            }

            bmp.Dispose();
            return Encoding.UTF8.GetString(textBytes);
        }
    }
}