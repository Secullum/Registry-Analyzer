using System;
using System.Windows.Forms;

namespace Registry_Analyzer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new FormRegistry();
            var view = new RegistryView(form);
            var presenter = new RegistryPresenter(view);

            Application.Run(form);
        }
    }
}
