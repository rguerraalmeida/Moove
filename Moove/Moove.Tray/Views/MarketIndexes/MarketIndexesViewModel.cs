using Moove.PresentationFramework.ViewModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Moove.Tray.Views.MarketIndexes
{
    public class MarketIndexesViewModel : BindableBase
    {
        public ObservableCollection<SingleIndexViewModel> MarketIndexesColletion { get; set; }

        public MarketIndexesViewModel()
        {
            MarketIndexesColletion = new ObservableCollection<SingleIndexViewModel>();

            //EU
            MarketIndexesColletion.Add(new SingleIndexViewModel("germany-30", "DAX"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("spain-35", "IBEX"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("psi-20", "PSI 20"));


            //USA
            MarketIndexesColletion.Add(new SingleIndexViewModel("us-30", "Dow Jones"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("nasdaq-composite", "Nasdaq"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("us-spx-500", "SP 500"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("nq-100", "Nasdaq 100"));

            //EU
            MarketIndexesColletion.Add(new SingleIndexViewModel("france-40", "CAC"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("uk-100", "FTSE UK"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("it-mib-40", "ITA MIB"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("eu-stoxx50", "EU-Stoxx50"));

            //RUSSIA
            MarketIndexesColletion.Add(new SingleIndexViewModel("mcx", "RUS MICEX"));
            MarketIndexesColletion.Add(new SingleIndexViewModel("rtsi", "RUS RTSI"));
        }
    }
}
