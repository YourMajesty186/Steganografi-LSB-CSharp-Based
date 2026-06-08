# SteganoPro - LSB Steganography Tool

SteganoPro adalah aplikasi desktop modern berbasis Windows Forms yang dirancang untuk menyembunyikan pesan teks rahasia di dalam file gambar menggunakan metode **Least Significant Bit (LSB)**.

## Fitur Utama
* **Modern UI**: Menggunakan tema *Dark Mode* dengan komponen UI yang *sleek* dan *rounded*.
* **LSB Encoding**: Menyisipkan pesan teks ke dalam channel warna gambar tanpa merusak kualitas visual gambar secara signifikan.
* **LSB Decoding**: Mengekstrak pesan tersembunyi dengan akurasi tinggi.
* **Lightweight**: Dibuat dengan C# dan .NET 8, ringan dan cepat.

## Prasyarat
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
* Visual Studio Code (disarankan) dengan C# Dev Kit.
* Koneksi internet (untuk mengunduh NuGet packages saat pertama kali *restore*).

## Cara Menjalankan
1.  **Clone** atau **Download** repositori ini ke komputer kamu.
2.  Buka terminal di folder proyek (`SteganografiApp`).
3.  Jalankan perintah untuk memulihkan *dependency*:
    ```bash
    dotnet restore
    ```
4.  Jalankan aplikasi:
    ```bash
    dotnet run
    ```

## Cara Penggunaan
1.  **Pilih Gambar**: Klik tombol "Pilih Gambar" dan pilih file berformat `.png`.
2.  **Encode**: Masukkan teks rahasia pada kotak input, lalu klik tombol "Sembunyikan (Encode)". Simpan gambar baru di lokasi yang diinginkan.
3.  **Decode**: Buka gambar yang sudah berisi pesan, lalu klik tombol "Ekstrak (Decode)". Pesan akan muncul di kotak teks.

## Teknologi yang Digunakan
* **C# / .NET 8**
* **WinForms** (Framework GUI)
* **Guna UI v2** (Modern UI Library)
* **System.Drawing.Common** (Manipulasi Bitmap)

## Catatan Penting
* **Format Gambar**: Selalu gunakan format **.png**. Jangan gunakan `.jpg` atau `.jpeg` karena kompresi *lossy* pada format tersebut akan merusak data bit (LSB) dan pesan tidak akan bisa diekstrak kembali.
* **Kapasitas**: Ukuran pesan maksimal bergantung pada resolusi gambar yang digunakan.

## Lisensi
Proyek ini dibuat untuk tujuan pembelajaran dalam matkul Kriptografi.

---
*Dibuat oleh [Nama Kamu]*
