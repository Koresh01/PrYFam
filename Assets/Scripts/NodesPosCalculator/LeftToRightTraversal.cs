using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrYFam
{
    public class LeftToRightTraversal : ITraversalStrategy
    {
        public IEnumerable<T> Traverse<T>(IList<T> list) => list;
        public bool IsLeftToRight => true;
    }
}
