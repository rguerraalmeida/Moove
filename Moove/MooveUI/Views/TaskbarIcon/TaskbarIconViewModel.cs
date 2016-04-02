using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Moove.PresentationFramework.Extensions;
using Moove.PresentationFramework.ViewModels;

namespace MooveUI.Views.TaskbarIcon
{
    public class TaskbarIconViewModel : BindableBaseExtension
    {
        public ObservableCollection<SingleIndexViewModel> TaskbarIconsCollection { get; set; }

        public TaskbarIconViewModel()
        {
            TaskbarIconsCollection = new ObservableCollection<SingleIndexViewModel>();

            var subscription = Observable.Merge(TaskbarIconsCollection.Select(t => t.OnAnyPropertyChanges()))
                .Subscribe(x => Console.WriteLine("{0} is {1}", x.Ticker, x.PercentChange));

            //EU
            TaskbarIconsCollection.Add(new SingleIndexViewModel("germany-30", "DAX"));
            TaskbarIconsCollection.Add(new SingleIndexViewModel("spain-35", "IBEX"));
            TaskbarIconsCollection.Add(new SingleIndexViewModel("psi-20", "PSI 20"));

            
        }
    }
}
