using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moove20.Entities
{
    [Serializable]
    public class EarningsDate
    {
        public string Company { get; set; }
        public string Ticker { get; set; }
        public DateTime Day { get; set; }
        public string WebDate { get; set; }
        public string Time { get; set; }

        public override string ToString()
        {
            return Ticker.ToString() + " " + Day.ToString();
        }

        /// <summary>
        /// Taken from Microsoft: Guidelines for Overloading Equals() and Operator == (C# Programming Guide)
        /// http://msdn.microsoft.com/en-us/library/ms173147%28v=vs.80%29.aspx
        /// </summary>
        #region Equality && GetHashCode

        public override bool Equals(System.Object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            EarningsDate ea = obj as EarningsDate;
            if ((object)ea == null)
            {
                return false;
            }

            // Return true if the fields match:
            //return base.Equals(obj) && (Ticker == ea.Ticker && Day == ea.Day);
            return (Ticker == ea.Ticker && Day == ea.Day);
        }

        public bool Equals(EarningsDate ea)
        {
            // Return true if the fields match:
            //return base.Equals((EarningsDate)ea) && (Ticker == ea.Ticker && Day == ea.Day);
            return (Ticker == ea.Ticker && Day == ea.Day);
        }

        public override int GetHashCode()
        {
            // return base.GetHashCode() ^ Ticker.GetHashCode() ^ Day.GetHashCode();
            //return base.GetHashCode();
            return string.Concat(Ticker, Day.ToString()).GetHashCode();
        }

        public static bool operator ==(EarningsDate a, EarningsDate b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            //return a.x == b.x && a.y == b.y && a.z == b.z;
            return a.Ticker == b.Ticker && a.Day == b.Day;
        }

        public static bool operator !=(EarningsDate a, EarningsDate b)
        {
            return !(a == b);
        }

        #endregion
    }
}
