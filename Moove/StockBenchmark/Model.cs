using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockBenchmark
{
    public static class AppParameters
	{
        public const string CNNSTRING = "Server=db4free.net;Database=moovebenchmark;Uid=mbadmin;Pwd=zezito;";
	}

    [Serializable]
    public class StockInfo : ID
    {
        public CompanyInfo CompanyInfo
        {
            get;
            set;
        }

        public List<HistoricalPrice> HistoricalPrices
        {
            get;
            set;
        }

        public List<Tick> Ticks
        {
            get;
            set;
        }

        public List<Bar> Ticks1m
        {
            get;
            set;
        }

        public HistoricalPrice TodayPrice
        {
            get;
            set;
        }

        public StockInfo()
        {
            this.CompanyInfo = new CompanyInfo();
            this.TodayPrice = new HistoricalPrice();
            this.HistoricalPrices = new List<HistoricalPrice>();
            this.Ticks = new List<Tick>();
        }
    }

    [Serializable]
    public class CompanyInfo : ID
    {
        public CompanyProfile CompanyProfile
        {
            get;
            set;
        }
        public CompanyInfo()
        {
            this.CompanyProfile = new CompanyProfile();
        }
    }

    [Serializable]
    public class CompanyProfile
    {
        public string Address
        {
            get;
            set;
        }

        public string BusinessSummary
        {
            get;
            set;
        }

        public string CompanyName
        {
            get;
            set;
        }

        public string CorporateGovernance
        {
            get;
            set;
        }

        public Industry? Industry
        {
            get;
            set;
        }

        public string IndustryName
        {
            get;
            set;
        }

        public Sector? Sector
        {
            get;
            set;
        }

        public string SectorName
        {
            get;
            set;
        }

        public CompanyProfile()
        {
        }
    }

    public class HistoricalPrice : DailyPrice
    {
        public long AvgVolume
        {
            get;
            set;
        }

        public double Change
        {
            get;
            set;
        }

        public double ChangePercent
        {
            get;
            set;
        }

        public double Gap
        {
            get;
            set;
        }

        public double GapPercent
        {
            get;
            set;
        }

        public double PreviousClose
        {
            get;
            set;
        }

        public double RelativeVolume
        {
            get;
            set;
        }

        public HistoricalPrice()
        {
        }
    }

    [Serializable]
    public class DailyPrice : Bar
    {
        public double IntradayChange
        {
            get;
            set;
        }

        public double IntradayChangePercent
        {
            get;
            set;
        }

        public DailyPrice()
        {
        }
    }

    [Serializable]
    public class Bar : Tick
    {
        public BarPeriod BarPeriod
        {
            get;
            set;
        }

        public double Close
        {
            get;
            set;
        }

        public double CloseAdjusted
        {
            get;
            set;
        }

        public string CloseAdjustmentType
        {
            get;
            set;
        }

        public double High
        {
            get;
            set;
        }

        public double Low
        {
            get;
            set;
        }

        public Period Period
        {
            get;
            set;
        }

        public long Volume
        {
            get;
            set;
        }

        public Bar()
        {
        }
    }

    [Serializable]
    public class Tick : ID
    {
        public double Open
        {
            get;
            set;
        }

        public double Price
        {
            get;
            set;
        }

        public int Size
        {
            get;
            set;
        }

        public DateTime TradeTime
        {
            get;
            set;
        }

        //public Tick()
        //{
        //}

        //public override string ToString()
        //{
        //    string[] shortTimeString = new string[] { "Time:", this.TradeTime.ToShortTimeString(), ";", "Price:", this.Price.ToString(), ";", "Size:", this.Size.ToString(), ";", "Open:", this.Open.ToString(), ";" };
        //    return string.Concat(shortTimeString);
        //}
    }

    [Serializable]
    public class ID
    {
        public bool IsChanged
        {
            get;
            set;
        }

        public string Ticker
        {
            get;
            set;
        }

        public ID()
        {
        }

        public void CommitChanges()
        {
            this.IsChanged = false;
        }

        public void MarkAsChanged()
        {
            this.IsChanged = true;
        }
    }

    public class NasdaqSymbol
    {
        public string ADR_TSO
        {
            get;
            set;
        }

        public string Exchange
        {
            get;
            set;
        }

        public string Industry
        {
            get;
            set;
        }

        public int IPOYear
        {
            get;
            set;
        }

        public double LastSale
        {
            get;
            set;
        }

        public float MarketCap
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Sector
        {
            get;
            set;
        }

        public string Ticker
        {
            get;
            set;
        }

        public NasdaqSymbol()
        {
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.Ticker))
            {
                return this.Ticker;
            }
            return this.ToString();
        }
    }


    [Serializable]
    public class Symbol
    {
        public short Attempts
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }

        public string Exchange
        {
            get;
            set;
        }

        public string Industry
        {
            get;
            set;
        }

        public double MarketCap
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string QuoteType
        {
            get;
            set;
        }

        public bool Sample
        {
            get;
            set;
        }

        public string Sector
        {
            get;
            set;
        }

        public string Ticker
        {
            get;
            set;
        }

        public bool Tradeable
        {
            get;
            set;
        }

        public Symbol()
        {
        }
    }

    [Serializable]
    public class TodayPrice
    {
        public long AvgVolume
        {
            get;
            set;
        }

        public double Change
        {
            get;
            set;
        }

        public double ChangePercent
        {
            get;
            set;
        }

        public double Gap
        {
            get;
            set;
        }

        public double GapPercent
        {
            get;
            set;
        }

        public double IntradayChange
        {
            get;
            set;
        }

        public double IntradayChangePercent
        {
            get;
            set;
        }

        public double Last
        {
            get;
            set;
        }

        public double Open
        {
            get;
            set;
        }

        public double PreviousClose
        {
            get;
            set;
        }

        public double RelativeVolume
        {
            get;
            set;
        }

        public string Ticker
        {
            get;
            set;
        }

        public long Volume
        {
            get;
            set;
        }

        public TodayPrice()
        {
        }
    }

    public enum BarPeriod
    {
        None = 0,
        OneMinute = 1,
        FiveMinutes = 5,
        TenMinutes = 10,
        FifteenMinutes = 15,
        HalfHour = 30,
        OneHour = 60,
        TwoHour = 120,
        FourHours = 240,
        Day = 1440,
        Week = 10080,
        Month = 43830
    }

    public enum Period
    {
        D,
        W,
        M
    }

    public enum Industry
    {
        Chemicals_Major_Diversified = 110,
        Synthetics = 111,
        Agricultural_Chemicals = 112,
        Specialty_Chemicals = 113,
        Major_Integrated_Oil_And_Gas = 120,
        Independent_Oil_And_Gas = 121,
        Oil_And_Gas_Refining_And_Marketing = 122,
        Oil_And_Gas_Drilling_And_Exploration = 123,
        Oil_And_Gas_Equipment_And_Services = 124,
        Oil_And_Gas_Pipelines = 125,
        Steel_And_Iron = 130,
        Copper = 131,
        Aluminum = 132,
        Industrial_Metals_And_Minerals = 133,
        Gold = 134,
        Silver = 135,
        Nonmetallic_Mineral_Mining = 136,
        Conglomerates = 210,
        Appliances = 310,
        Home_Furnishings_And_Fixtures = 311,
        Housewares_And_Accessories = 312,
        Business_Equipment = 313,
        Electronic_Equipment = 314,
        Toys_And_Games = 315,
        Sporting_Goods = 316,
        Recreational_Goods_Other = 317,
        Photographic_Equipment_And_Supplies = 318,
        Textile_Apparel_Clothing = 320,
        Textile_Apparel_Footwear_And_Accessories = 321,
        Rubber_And_Plastics = 322,
        Personal_Products = 323,
        Paper_And_Paper_Products = 324,
        Packaging_And_Containers = 325,
        Cleaning_Products = 326,
        Office_Supplies = 327,
        Auto_Manufacturers_Major = 330,
        Trucks_And_Other_Vehicles = 331,
        Recreational_Vehicles = 332,
        Auto_Parts = 333,
        Food_Major_Diversified = 340,
        Farm_Products = 341,
        Processed_And_Packaged_Goods = 342,
        Meat_Products = 343,
        Dairy_Products = 344,
        Confectioners = 345,
        Beverages_Brewers = 346,
        Beverages_Wineries_And_Distillers = 347,
        Beverages_Soft_Drinks = 348,
        Cigarettes = 350,
        Tobacco_Products_Other = 351,
        Money_Center_Banks = 410,
        Regional_Northeast_Banks = 411,
        Regional_Mid_Atlantic_Banks = 412,
        Regional_Southeast_Banks = 413,
        Regional_Midwest_Banks = 414,
        Regional_Southwest_Banks = 415,
        Regional_Pacific_Banks = 416,
        Foreign_Money_Center_Banks = 417,
        Foreign_Regional_Banks = 418,
        Savings_And_Loans = 419,
        Investment_Brokerage_National = 420,
        Investment_Brokerage_Regional = 421,
        Asset_Management = 422,
        Diversified_Investments = 423,
        Credit_Services = 424,
        Closed_End_Fund_Debt = 425,
        Closed_End_Fund_Equity = 426,
        Closed_End_Fund_Foreign = 427,
        Life_Insurance = 430,
        Accident_And_Health_Insurance = 431,
        Property_And_Casualty_Insurance = 432,
        Surety_And_Title_Insurance = 433,
        Insurance_Brokers = 434,
        REIT_Diversified = 440,
        REIT_Office = 441,
        REIT_Healthcare_Facilities = 442,
        REIT_Hotel_Motel = 443,
        REIT_Industrial = 444,
        REIT_Residential = 445,
        REIT_Retail = 446,
        Mortgage_Investment = 447,
        Property_Management = 448,
        Real_Estate_Development = 449,
        Drug_Manufacturers_Major = 510,
        Drug_Manufacturers_Other = 511,
        Drugs_Generic = 512,
        Drug_Delivery = 513,
        Drug_Related_Products = 514,
        Biotechnology = 515,
        Diagnostic_Substances = 516,
        Medical_Instruments_And_Supplies = 520,
        Medical_Appliances_And_Equipment = 521,
        Health_Care_Plans = 522,
        Long_Term_Care_Facilities = 523,
        Hospitals = 524,
        Medical_Laboratories_And_Research = 525,
        Home_Health_Care = 526,
        Medical_Practitioners = 527,
        Specialized_Health_Services = 528,
        Aerospace_Defense_Major_Diversified = 610,
        Aerospace_Defense_Products_And_Services = 611,
        Farm_And_Construction_Machinery = 620,
        Industrial_Equipment_And_Components = 621,
        Diversified_Machinery = 622,
        Pollution_And_Treatment_Controls = 623,
        Machine_Tools_And_Accessories = 624,
        Small_Tools_And_Accessories = 625,
        Metal_Fabrication = 626,
        Industrial_Electrical_Equipment = 627,
        Textile_Industrial = 628,
        Residential_Construction = 630,
        Manufactured_Housing = 631,
        Lumber_Wood_Production = 632,
        Cement = 633,
        General_Building_Materials = 634,
        Heavy_Construction = 635,
        General_Contractors = 636,
        Waste_Management = 637,
        Lodging = 710,
        Resorts_And_Casinos = 711,
        Restaurants = 712,
        Specialty_Eateries = 713,
        Gaming_Activities = 714,
        Sporting_Activities = 715,
        General_Entertainment = 716,
        Advertising_Agencies = 720,
        Marketing_Services = 721,
        Entertainment_Diversified = 722,
        Broadcasting_TV = 723,
        Broadcasting_Radio = 724,
        CATV_Systems = 725,
        Movie_Production_Theaters = 726,
        Publishing_Newspapers = 727,
        Publishing_Periodicals = 728,
        Publishing_Books = 729,
        Apparel_Stores = 730,
        Department_Stores = 731,
        Discount_Variety_Stores = 732,
        Drug_Stores = 733,
        Grocery_Stores = 734,
        Electronics_Stores = 735,
        Home_Improvement_Stores = 736,
        Home_Furnishing_Stores = 737,
        Auto_Parts_Stores = 738,
        Catalog_And_Mail_Order_Houses = 739,
        Sporting_Goods_Stores = 740,
        Toy_And_Hobby_Stores = 741,
        Jewelry_Stores = 742,
        Music_And_Video_Stores = 743,
        Auto_Dealerships = 744,
        Specialty_Retail_Other = 745,
        Auto_Parts_Wholesale = 750,
        Building_Materials_Wholesale = 751,
        Industrial_Equipment_Wholesale = 752,
        Electronics_Wholesale = 753,
        Medical_Equipment_Wholesale = 754,
        Computers_Wholesale = 755,
        Drugs_Wholesale = 756,
        Food_Wholesale = 757,
        Basic_Materials_Wholesale = 758,
        Wholesale_Other = 759,
        Business_Services = 760,
        Rental_And_Leasing_Services = 761,
        Personal_Services = 762,
        Consumer_Services = 763,
        Staffing_And_Outsourcing_Services = 764,
        Security_And_Protection_Services = 765,
        Education_And_Training_Services = 766,
        Technical_Services = 767,
        Research_Services = 768,
        Management_Services = 769,
        Major_Airlines = 770,
        Regional_Airlines = 771,
        Air_Services_Other = 772,
        Air_Delivery_And_Freight_Services = 773,
        Trucking = 774,
        Shipping = 775,
        Railroads = 776,
        Diversified_Computer_Systems = 810,
        Personal_Computers = 811,
        Computer_Based_Systems = 812,
        Data_Storage_Devices = 813,
        Networking_And_Communication_Devices = 814,
        Computer_Peripherals = 815,
        Multimedia_And_Graphics_Software = 820,
        Application_Software = 821,
        Technical_And_System_Software = 822,
        Security_Software_And_Services = 823,
        Information_Technology_Services = 824,
        Healthcare_Information_Services = 825,
        Business_Software_And_Services = 826,
        Information_And_Delivery_Services = 827,
        Semiconductor_Broad_Line = 830,
        Semiconductor_Memory_Chips = 831,
        Semiconductor_Specialized = 832,
        Semiconductor_Integrated_Circuits = 833,
        Semiconductor_Equipment_And_Materials = 834,
        Printed_Circuit_Boards = 835,
        Diversified_Electronics = 836,
        Scientific_And_Technical_Instruments = 837,
        Wireless_Communications = 840,
        Communication_Equipment = 841,
        Processing_Systems_And_Products = 842,
        Long_Distance_Carriers = 843,
        Telecom_Services_Domestic = 844,
        Telecom_Services_Foreign = 845,
        Diversified_Communication_Services = 846,
        Internet_Service_Providers = 850,
        Internet_Information_Providers = 851,
        Internet_Software_And_Services = 852,
        Foreign_Utilities = 910,
        Electric_Utilities = 911,
        Gas_Utilities = 912,
        Diversified_Utilities = 913,
        Water_Utilities = 914
    }

    public enum Sector
    {
        Basic_Materials = 1,
        Conglomerates = 2,
        Consumer_Goods = 3,
        Financial = 4,
        Healthcare = 5,
        Industrial_Goods = 6,
        Services = 7,
        Technology = 8,
        Utilities = 9
    }

}
