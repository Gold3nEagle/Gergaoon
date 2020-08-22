using System;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.System.Combiner;
using UnityEngine;
using Random = System.Random;

namespace SweetSugar.Scripts.System
{
    public static class LinqUtils
    {
        public static bool AllNull<T>(this IEnumerable<T> seq)
        {
            var result = seq.All(i => i == null || i.Equals(null) || !seq.Any());
            return result;
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> seq)
        {
            var result = seq.Where(i => i != null && !i.Equals(null));
            return result;
        }

        public static T TryGetElement<T>(this IEnumerable<T> seq, int index, T ifnull=default(T))
        {
            if (index < seq.Count())
            {
                var element = seq.ToArray()[index];
                return element;
            }
            return ifnull;
        }  


        public static IEnumerable<T> ForEachY<T>(this IEnumerable<T> seq, Action<T> action)
        {
            seq.ToList().ForEach(action);
            var result = seq;
            return result;
        }
    
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            Random rnd = new Random();
            return source.OrderBy<T, int>((item) => rnd.Next());
        }


        public static ItemTemplate GetElement(this ItemTemplate[] seq, int x, int y)
        {
            return seq.First(i => i.position == new Vector2(x, y));
        }
    
        public static T ElementAtOrDefault<T>(this IList<T> list, int index, T @default)
        {
            return index >= 0 && index < list.Count ? list[index] : @default;
        }
    
        public static T Addd<T>(this List<T> list, T newItem)
        {
            list.Add(newItem);
            return newItem;
        }
    
        public static T NextRandom<T>(this IEnumerable<T> source)
        {
            Random gen = new Random((int)DateTime.Now.Ticks);
            return source.Skip(gen.Next(0, source.Count() - 1) - 1).Take(1).FirstOrDefault();
        }

        public static IEnumerable<T> SelectRandom<T>(this IEnumerable<T> source)
        {
            List<T> Remaining = new List<T>(source);
            while (Remaining.Count >= 1)
            {
                T temp = NextRandom(Remaining);
                Remaining.Remove(temp);
                yield return temp;
            }
        }
    }
}