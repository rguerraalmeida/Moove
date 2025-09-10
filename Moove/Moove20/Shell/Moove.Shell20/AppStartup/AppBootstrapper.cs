using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Moove.Shell20.AppStartup
{
    public class AppBootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            RegisterTypesWithDependencies();
            RegisterViews();
        }

        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
        }

        protected override DependencyObject CreateShell()
        {
            // Use the container to create an instance of the shell.
            MainWindow mainWindow = Container.TryResolve<MainWindow>();

            // Display the shell's root visual.
            mainWindow.Show();
            return mainWindow;
        }

        private void RegisterViews()
        {
            //this.Container.RegisterType<Object, Modules.Earnings.EarningsWindow>();
            //this.Container.RegisterType<Object, Modules.Earnings.EarningsViewModel>();
        }

        private void RegisterTypesWithDependencies()
        {
            
        }
    }
}
