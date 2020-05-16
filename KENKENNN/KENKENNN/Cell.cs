using System.Collections.Generic;

namespace KENKENNN
{
    public class Cell
    {
        //Координаты клетки
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        //Список потенциальных значений для данной клетки
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
