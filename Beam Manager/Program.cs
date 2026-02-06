// Relative Path: Program.cs

namespace Beam_Manager
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Global Exception Handling
            Application.ThreadException += (s, e) => MessageBox.Show(e.Exception.Message, "Thread Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => MessageBox.Show((e.ExceptionObject as Exception)?.Message, "App Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            ApplicationConfiguration.Initialize();

            // ENABLE NATIVE DARK MODE (NET 10 Feature)
            Application.SetColorMode(SystemColorMode.Dark);

            Application.Run(new Form1());
        }
    }
}