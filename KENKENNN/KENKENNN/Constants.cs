using System.Collections.Generic;
using System.Drawing;

namespace KENKENNN
{
    public static class Constants
    {
        // размер матрицы
        public const int MapSize = 5;

        // Размер одной клетки
        public const int CellSize = 75;

        public const int FreeCell = -1;
        public const int OccupiedCell = 1;

        // Словарь с различными цветами
        public static readonly Dictionary<int, Color> RegionColors = new Dictionary<int, Color>
        {
            {0, Color.FromArgb(181, 199, 247 )},
            {1, Color.FromArgb(196, 181, 247 )},
            {2, Color.FromArgb(229, 181, 247)},
            {3, Color.FromArgb(247, 181, 232)},
            {4, Color.FromArgb(247, 181, 199)},
            {5, Color.FromArgb(247, 196, 181)},
            {6, Color.FromArgb(181, 247, 228 )},
            {7, Color.FromArgb(181, 233, 247)},
            {8, Color.FromArgb(181, 247, 191)},
            {9, Color.FromArgb(014, 203, 005)},
            {10, Color.FromArgb(247, 181, 199)},
            {11, Color.FromArgb(247, 196, 181)},
            {12, Color.FromArgb(181, 247, 228 )},
            {13, Color.FromArgb(181, 233, 247)},
        };
    }
}
