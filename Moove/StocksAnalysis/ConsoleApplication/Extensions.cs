using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ConsoleApplication
{
    public static class Extensions
    {
        public static IEnumerable<TSource> TakeSkip<TSource>(this IEnumerable<TSource> source, int count)
        {
            IEnumerable<TSource> buffer = source;
            IEnumerable<TSource> sample;
            do
            {

                sample = buffer.Take(count);
                buffer = buffer.Skip(count);
                yield return (TSource)sample;

            } while (buffer.Count() > 0);


        }
        
        public static IEnumerable<TSource> TakeSkip<TSource>(this List<TSource> source, int count)
        {
            return TakeSkip(source.AsEnumerable(), count);
        }

        public static void Dump(this IEnumerable<string> source)
        {
            source.ToList().ForEach(x => Console.WriteLine(x.Length >= 250 ? "Name too big" : x ));
        }

    }
}

