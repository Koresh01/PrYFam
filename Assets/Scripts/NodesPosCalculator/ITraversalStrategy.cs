using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrYFam
{
    /// <summary>
    /// Интерфейс, определяющий стратегию обхода коллекции узлов.
    /// Позволяет гибко задавать порядок обхода (например, слева направо или справа налево).
    /// </summary>
    public interface ITraversalStrategy
    {
        /// <summary>
        /// Выполняет обход элементов коллекции в заданном порядке.
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции.</typeparam>
        /// <param name="list">Коллекция элементов для обхода.</param>
        /// <returns>Коллекция элементов в порядке, определённом стратегией.</returns>
        IEnumerable<T> Traverse<T>(IList<T> list);

        /// <summary>
        /// Указывает, выполняется ли обход слева направо.
        /// </summary>
        /// <remarks>
        /// Значение true означает обход слева направо, false — справа налево.
        /// </remarks>
        bool IsLeftToRight { get; }
    }
}
