using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrYFam.Assets.Scripts.NodesPosCalculator
{
    public interface ITraversalStrategy
    {
        IEnumerable<T> Traverse<T>(IList<T> list);
        bool IsLeftToRight { get; }
    }
}
