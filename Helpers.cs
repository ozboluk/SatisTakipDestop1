using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baybars
{
public static class Helpers
    {
        public static class StringExtensions
        {
            public static string Right(string str, int length)
            {
                try
                {
                    return str.Substring(str.Length - length, length);
                }
                catch 
                {
                    return str.Substring(str.Length - length, length);

                }
               
            }
        }
    }
}
