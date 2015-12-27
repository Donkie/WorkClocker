using System;
using System.Windows;
using WorkClocker.Helpers;

namespace WorkClocker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }

        [STAThread]
        public static void Main()
        {
            const string res = "WorkClocker.References.MouseKeyboardActivityMonitor.dll";
            EmbeddedAssembly.Load(res, "MouseKeyboardActivityMonitor.dll");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var application = new App();
            application.InitializeComponent();
            application.Run();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            WorkClocker.Properties.Settings.Default.Save();
        }
    }
}
