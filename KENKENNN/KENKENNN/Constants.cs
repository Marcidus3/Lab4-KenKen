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

        //Помечалки состояния клетки
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
            {9, Color.FromArgb(054, 203, 155)},
            {10, Color.FromArgb(247, 181, 199)},
            {11, Color.FromArgb(247, 196, 181)},
            {12, Color.FromArgb(181, 247, 228 )},
            {13, Color.FromArgb(181, 233, 247)},
        };
        public static readonly Dictionary<string, Color> RegionColors2 = new Dictionary<string, Color>
        {
            {"a", Color.FromArgb(181, 199, 247 )},
            {"b", Color.FromArgb(196, 181, 247 )},
            {"c", Color.FromArgb(229, 181, 247)},
            {"d", Color.FromArgb(247, 181, 232)},
            {"e", Color.FromArgb(247, 181, 199)},
            {"f", Color.FromArgb(247, 196, 181)},
            {"g", Color.FromArgb(181, 247, 228 )},
            {"h", Color.FromArgb(181, 233, 247)},
            {"i", Color.FromArgb(181, 247, 191)},
            {"j", Color.FromArgb(054, 203, 155)},
            {"k", Color.FromArgb(247, 181, 199)},
            {"l", Color.FromArgb(247, 196, 181)},
            {"m", Color.FromArgb(181, 247, 228 )},
            {"n", Color.FromArgb(181, 233, 247)},
        };
    }
}
