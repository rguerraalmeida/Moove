using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moove.PresentationFramework.ViewModels
{
    public class BindableBaseExtension : BindableBase
    {
        public void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                base.OnPropertyChanged(name);
            }
        }
    }
}
