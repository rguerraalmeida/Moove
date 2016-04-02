using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MooveUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //this.PrintCultures();
            //this.PrintEnums();

            base.OnStartup(e);
        }


        private void PrintCultures()
        {

            // Create a table of most culture types. 
            CultureTypes[] mostCultureTypes = new CultureTypes[] {
                        CultureTypes.NeutralCultures,
                        CultureTypes.SpecificCultures,
                        CultureTypes.InstalledWin32Cultures,
                        CultureTypes.UserCustomCulture,
                        CultureTypes.ReplacementCultures,
                        CultureTypes.FrameworkCultures,
                        CultureTypes.WindowsOnlyCultures,
                        CultureTypes.AllCultures,
                        };
            CultureInfo[] allCultures;
            CultureTypes combo;

            // Get and enumerate all cultures.
            allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo ci in allCultures)
            {
                // Display the name of each culture.
                Console.WriteLine("Culture: {0}", ci.Name);

                // Get the culture types of each culture. 
                combo = ci.CultureTypes;

                // Display the name of each culture type flag that is set.
                Console.Write("  ");
                foreach (CultureTypes ct in mostCultureTypes)
                    if (0 != (ct & combo))
                        Console.Write("{0} ", ct);
                Console.WriteLine();
            }
        }

        private void PrintEnums()
        {
            
            Console.WriteLine("public class CultureCodes");
            Console.WriteLine("{");
            // Displays several properties of the neutral cultures.
           // Console.WriteLine("CULTURE ISO ISO WIN DISPLAYNAME ENGLISHNAME");
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures).Where(c=> !string.IsNullOrWhiteSpace(c.TextInfo.CultureName)))
            {
                Console.Write("{0,-7}", ci.Name);
                Console.Write(" {0,-3}", ci.TwoLetterISOLanguageName);
                Console.Write(" {0,-3}", ci.ThreeLetterISOLanguageName);
                Console.Write(" {0,-3}", ci.ThreeLetterWindowsLanguageName);
                Console.Write(" {0,-40}", ci.DisplayName);
                Console.WriteLine(" {0,-40}", ci.EnglishName);
                Console.WriteLine(" {0,-40}", ci.TextInfo.CultureName);

                Console.WriteLine("public static string {0} = \"{1}\" ;", ci.TwoLetterISOLanguageName, ci.TextInfo.CultureName);

            }
            Console.WriteLine("}");
        }
    }
}
