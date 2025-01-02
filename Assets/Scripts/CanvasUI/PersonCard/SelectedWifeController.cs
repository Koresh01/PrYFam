using System.Collections.Generic;
using UnityEngine;

namespace PrYFam
{
    /// <summary>
    /// Хранит индекс текущей выбранной жены. Это важно, ведь теперь древо поддерживает многоженство.
    /// </summary>
    class SelectedWifeController : MonoBehaviour
    {
        int inx = 0;


        /// <summary>
        /// Смещает индекс.
        /// </summary>
        public void NextWife(List<Member> wifes)
        {
            inx = (inx+1) % wifes.Count;
        }

        /// <summary>
        /// Возвращает актуальный индекс выбранной жены.
        /// </summary>
        public int GetInx(List<Member> wifes)
        {
            inx = inx % wifes.Count;
            return inx;
        }
    }
}
