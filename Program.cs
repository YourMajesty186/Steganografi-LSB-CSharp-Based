using System;
using System.Windows.Forms;

namespace SteganografiApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Menjalankan Form1 dengan aman di bawah kendali namespace SteganografiApp
            Application.Run(new Form1());
        }
    }
}