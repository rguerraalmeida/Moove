using Moove.Tray.Views.MarketIndexes;
using Prism.Mvvm;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;

namespace Moove.Tray
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class TrayIconViewModel
    {
        TaskbarIcon _notifyIcon;
        MarketIndexesUserControl _marketIndexesUserControl = new MarketIndexesUserControl();
        MarketIndexesViewModel _marketIndexesViewModel = new MarketIndexesViewModel();

        public TrayIconViewModel(TaskbarIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
            this.AttachViewWithViewModel(_marketIndexesUserControl, _marketIndexesViewModel);
        }

        private void AttachViewWithViewModel(ContentControl view, BindableBase viewModel)
        {
            view.DataContext = viewModel;
        }

        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Application.Current.MainWindow == null,
                    CommandAction = () =>
                    {
                        //balloon.BalloonText = customBalloonTitle.Text;
                        //show and close after 2.5 seconds
                        _notifyIcon.ShowCustomBalloon(_marketIndexesUserControl, PopupAnimation.Slide, 5000);
                        
                        //Application.Current.MainWindow = new Moove.Tray.Views.MarketIndexes.MarketIndexesWindow();
                        //Application.Current.MainWindow.Show();
                    }
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => Application.Current.MainWindow.Close(),
                    CanExecuteFunc = () => Application.Current.MainWindow != null
                };
            }
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }
    }


    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
