using System;
using System.Globalization;
using System.Windows.Forms;

namespace ServerStartApp
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var temp = CultureInfo.InstalledUICulture;
            CultureInfo.CurrentCulture = temp;
            CultureInfo.CurrentUICulture = temp;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
