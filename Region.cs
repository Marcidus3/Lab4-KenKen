using System.Collections.Generic;
using static KENKENNN.EnumUtils;

namespace KENKENNN
{
    public class Region
    {
        public int Size => Cells?.Count ?? 0;
        public Operator Operation { get; set; }
        public List<Cell> Cells { get; set; } = new List<Cell>();
        public int RegionValue { get; set; }

        public Region() { }
        public Region(int regionValue, Operator operation, List<Cell> neighbors)
        {
            RegionValue = regionValue;
            Operation = operation;
            Cells = neighbors;
        }
    }
}
