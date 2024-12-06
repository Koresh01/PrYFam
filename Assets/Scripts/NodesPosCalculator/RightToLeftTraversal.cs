using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrYFam.Assets.Scripts.NodesPosCalculator
{
    public class RightToLeftTraversal : ITraversalStrategy
    {
        public IEnumerable<T> Traverse<T>(IList<T> list) => Enumerable.Reverse(list);
        public bool IsLeftToRight => false;
    }
}
