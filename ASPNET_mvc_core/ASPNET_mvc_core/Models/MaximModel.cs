using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNET_mvc_core.Models
{
    public class MaximModel
    {
        private static List<string> _destinies;
        public static string Destiny {
            get
            {
                if (_destinies == null)
                {
                    _destinies = new List<string>();
                    return "кирдым";
                }
                if (_destinies.Count == 0)
                    return "кирдым";
                return "/playlist/" + _destinies[RandomUtils.Range(0, _destinies.Count)];
            }
            set
            {
                if (_destinies == null)
                    _destinies = new List<string>();
                _destinies.Add(value);
            }
        }
    }
}
