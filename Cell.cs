using System.Collections.Generic;

namespace KENKENNN
{
    public class Cell
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public List<int> Candidates { get; set; }
        // Пока клетка пустая, данные значение будет 'FreeCell'
        public int Answer { get; set; } = Constants.FreeCell;

        public Cell(int rowIx, int colIx)
        {
            RowIndex = rowIx;
            ColumnIndex = colIx;
        }
    }
}
