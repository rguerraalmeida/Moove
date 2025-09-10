using Moove.Shell20.AppStartup;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Moove.Shell20
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if (DEBUG)
            RunInDebugMode();
#else
            RunInReleaseMode();
#endif
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private static void RunInDebugMode()
        {
            ConfigureLogger();
            SetTheme();
            
            AppBootstrapper bootstrapper = new AppBootstrapper();
            bootstrapper.Run();
        }

        private static void RunInReleaseMode()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            try
            {
                ConfigureLogger();
                SetTheme();
                AppBootstrapper bootstrapper = new AppBootstrapper();
                bootstrapper.Run();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private static void SetTheme()
        {
            //Telerik Code
            //StyleManager.ApplicationTheme = new Expression_DarkTheme();
        }

        private static void ConfigureLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private static void HandleException(Exception ex)
        {
            if (ex == null) return;

            MessageBox.Show(ex.Message);
            MessageBox.Show("An unhandled exception occurred, and the application is terminating. For more information, see your Application log.");
            Environment.Exit(1);
        }
    }
}
