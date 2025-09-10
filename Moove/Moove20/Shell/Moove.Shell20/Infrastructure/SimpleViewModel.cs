using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moove.Shell20.Infrastructure
{
    public class SimpleViewModel :  NotificationObject, IDisposable
    {
        #region Private Variables

        protected IEventAggregator _eventAggregator;
        protected IRegionManager _regionManager;
        protected IUnityContainer _container;

        #endregion

        #region Constructors
        //#TODO
        //depois de investigar uma pattern para Navigation considerar também integra-la aqui
        public SimpleViewModel() { }

        public SimpleViewModel(IEventAggregator eventAggregator)
            : this()
        {
            _eventAggregator = eventAggregator;
        }

        public SimpleViewModel(IRegionManager regionManager)
            : this()
        {
            _regionManager = regionManager;
        }

        public SimpleViewModel(IUnityContainer container)
            : this()
        {
            _container = container;
        }

        public SimpleViewModel(IEventAggregator eventAggregator, IUnityContainer container)
            : this()
        {
            _eventAggregator = eventAggregator;
            _container = container;
        }

        public SimpleViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
            : this()
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
        }

        public SimpleViewModel(IRegionManager regionManager, IUnityContainer container)
            : this()
        {
            _regionManager = regionManager;
            _container = container;
        }

        public SimpleViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IUnityContainer container)
            : this()
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _container = container;
        }

        #endregion

        #region IsBusyMembers

        //helps manage the busy state when there are several background actions
        protected Dictionary<string, bool> busyOperations = new Dictionary<string, bool>();

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value; base.RaisePropertyChanged(() => this.IsBusy);
            }
        }

        private string _isBusyMessage = "Working...";
        public virtual string IsBusyMessage
        {
            get { return _isBusyMessage; }
            set
            {
                _isBusyMessage = value;
                base.RaisePropertyChanged(() => this.IsBusyMessage);
            }
        }

        public void AddBusyOperation(string key)
        {
            IsBusy = true;
            busyOperations.Add(key, true);
        }

        public void AddBusyOperation(string key, string message)
        {
            IsBusy = true;
            IsBusyMessage = message;
            busyOperations.Add(key, true);
        }

        public void RemoveBusyOperation(string key)
        {
            busyOperations.Remove(key);
            IsBusy = busyOperations.Keys.Count > 0;
        }


        #endregion

        #region IDisposable Members
        /// <summary>
        /// Implementation of the dispose method
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }
        /// <summary>
        /// The child class should implement a personal dispose procedure
        /// </summary>
        protected virtual void OnDispose()
        {
            //do nothing because abstract
        }

        #endregion

        #region CanExecute Members

        protected bool AllowCanExecute(object commandArg)
        {
            return !_isBusy;
        }
        #endregion
    }
}
