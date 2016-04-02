using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Moove.PresentationFramework.Culture
{
    /// <summary>
    /// CultureManager
    /// Uses a singleton pattern described
    /// http://csharpindepth.com/Articles/General/Singleton.aspx#conclusion
    /// </summary>
    public sealed class CultureManager
    {
        public const string EnglishCultureCode = "en-US";
        public const string PortugueseCultureCode = "pt-PT";

        #region Singleton Pattern

        private static readonly CultureManager instance = new CultureManager();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static CultureManager() { }

        private CultureManager() { }

        public static CultureManager Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        private CultureInfo originalCulture;
        private CultureInfo originalUICulture;

        public IEnumerable<CultureInfo> AllCultures
        {
            get
            {
                return CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
            }
        }

        public void RestorePreviousCultureInfo()
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }

        private void ChangeCultureInfo(string cultureCode)
        {
            originalCulture = Thread.CurrentThread.CurrentCulture;
            originalUICulture = Thread.CurrentThread.CurrentUICulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCode);
        }

        private void ChangeCultureToEnglish()
        {
            ChangeCultureInfo(EnglishCultureCode);
        }

        public void ExecuteIn(string cultureCode, Action action)
        {
            ChangeCultureInfo(cultureCode);
            action();
            RestorePreviousCultureInfo();
        }

        public CultureInfo EnglishCulture()
        {
            return new CultureInfo(EnglishCultureCode);
        }

        public CultureInfo PortugueseCulture()
        {
            return new CultureInfo(PortugueseCultureCode);
        }

        public IEnumerable<CultureInfo> GetAllCultures()
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
        }

    }
}
