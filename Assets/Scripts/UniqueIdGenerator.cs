using System;

namespace PrYFam
{
    public static class UniqueIdGenerator
    {
        /// <summary>
        /// Создает уникальный идентификатор.
        /// </summary>
        /// <returns>Уникальная строка идентификатора.</returns>
        public static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
