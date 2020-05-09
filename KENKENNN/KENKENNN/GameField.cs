// --------------------------------------------------------------------------------------
//  Задание
//
//  Кен-кен
//  - это математическая и логическая головоломка.Необходимо заполнить сетку цифрами так,
//  чтобы в каждой строке и в каждом столбце они не повторялись.Число в углу каждого
//  выделенного блока является результатом арифметической операции над цифрами в этом
//  блоке.В отличие от судоку-убийцы(сум-до-ку), цифры внутри блока могут повторяться.
//
//  Должна решаться сама или пользователем
// --------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using static KENKENNN.EnumUtils;
using static KENKENNN.Constants;

namespace KENKENNN
{
    public partial class GameField : Form
    {
        #region Private members
        // Экземпляр класса рандомизатора
        private Random randomGenerator = new Random();

        // Создаем массив текстбоксов 
        private TextBox[][] textCell = new TextBox[MapSize][];

        // Создем массив, в который будем хранить информацию о том
        // Задействованы ли клеточки в областях
        private int[,] map;
        // Создаем массив с ответами
        private int[,] correct;

        private List<Region> regions = new List<Region>();
        #endregion

        #region Construction
        /// <summary>
        /// Construction.
        /// </summary>
        public GameField()
        {
            InitializeComponent();
            InitializeBoard();
        }
        #endregion

        #region Auxiliary methods
        private void InitializeBoard()
        {
            // инициализация массива для областей
            correct = new int[MapSize, MapSize];
            map = new int[MapSize, MapSize];

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    map[i, j] = FreeCell;
                }
            }

            // Открытие и прочтение файла, на основе которого генерируется игра
            var lines = File.ReadAllLines(@"..\123.txt");
            var cells = new string[lines.Length][];

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = lines[i].Split(
                    new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            }
     
            // Заполнение матрицы ответов
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    correct[i, j] = int.Parse(cells[i][j]);
                }
            }

            CreateMap();
            GenerateParts();
        }

        // Функиця создания поля игры
        private void CreateMap()
        {
            // Задаем размер окна
            Width = MapSize * CellSize + CellSize / 2; //(MapSize + 1 ) * (CellSize-6)+3;
            Height = MapSize * CellSize + 2 * CellSize; //(MapSize + 3) * CellSize-10;

            // Создаем клеточки игрового поля
            for (int i = 0; i < MapSize; i++)
            {
                textCell[i] = new TextBox[MapSize];
                for (int j = 0; j < MapSize; j++)
                {
                    textCell[i][j] = new TextBox();
                    textCell[i][j].Size = new Size(CellSize, CellSize);
                    textCell[i][j].Multiline = true;
                    textCell[i][j].Location = new Point(j * CellSize, i * CellSize);
                    textCell[i][j].Text = correct[i, j].ToString();
                    textCell[i][j].Font = new Font(textCell[i][j].Font.FontFamily, 16);
                    textCell[i][j].Text = string.Empty;

                    Controls.Add(textCell[i][j]);
                }
            }

            // Задаем расположение кнопок
            CompleteButton.Location = new Point(0, Height - 2 * CellSize);
            AutoComleteButton.Location = new Point(Width / 2, Height - 2 * CellSize);
        }

        // Функиця генерации разделения игрового поля на области 
        private void GenerateParts()
        {
            // Счетчик количества областей
            var regionsCount = 0;

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    var cellColor = RegionColors[regionsCount];
                    var region = new Region();
                    var cell = new Cell(i, j);
                    Cell neighbourCcell1 = null, neighbourCell2 = null;

                    // Данное значение отвечает за основную клетку области
                    cell.Answer = correct[i, j];

                    // Если клетка не относится ни к одной из областей
                    if (CellIsFree(cell))
                    {
                        region.Cells.Add(cell);
                        
                        // Красим клетку в цвет, соответствующий региону
                        textCell[i][j].BackColor = cellColor;
                        OccupyCell(cell);

                        // Если клетка не на границе & клетка рядом свободна
                        if (i < MapSize - 1)
                        {
                            neighbourCcell1 = new Cell(i + 1, j);
                            if (CellIsFree(neighbourCcell1))
                            {
                                region.Cells.Add(neighbourCcell1);

                                // Помечаем значение соседней клетки, записываем туда 
                                // Правильный ответ в этой клетке
                                neighbourCcell1.Answer = correct[i + 1, j];

                                // Красим в цвет основной клетки
                                textCell[i + 1][j].BackColor = cellColor;
                                OccupyCell(neighbourCcell1);
                            }
                        }

                        // Если клетка не на границе & клетка рядом свободна
                        if (j < MapSize - 1)
                        {
                            neighbourCell2 = new Cell(i, j + 1);
                            if (CellIsFree(neighbourCell2))
                            {
                                region.Cells.Add(neighbourCell2);

                                // Помечаем значение соседней клетки, записываем туда 
                                // Правильный ответ в этой клетке
                                neighbourCell2.Answer = correct[i, j + 1];

                                // Красим в цвет основной клетки
                                textCell[i][j + 1].BackColor = cellColor;
                                OccupyCell(neighbourCell2);
                            }
                        }

                        //Создание правила для сформированной области
                        MakeRule(cell, neighbourCcell1, neighbourCell2, region);
                        regionsCount++;
                    }
                }
            }
        }

        // Создание правила для области игрового поля
        // Сюда передаются значения основной и соседних клеточек, а так же их индексы
        private void MakeRule(Cell primaryCell, Cell neighbourCell1, Cell neighbourCell2, Region region)
        {
            // Рандомизатор целых чисел, выдает число от 1 до 4х
            var regionOperation = (Operator)randomGenerator.Next((int)Operator.Add, (int)Operator.Div);
            var operationValue = string.Empty;
            var regionValue = primaryCell.Answer;

            // Если область состоит только из одной клеточки, то заполняем 
            // ее числом из ответа, как указано в правилах игры
            if (!CellIsFree(neighbourCell1) || !CellIsFree(neighbourCell2))
            {
                switch (regionOperation)
                {
                    case Operator.Add:
                        operationValue = "(+)";
                        // Если рандомизатор выдает число 1, то правило для области - сложение
                        if (CellIsFree(neighbourCell1))
                        {
                            regionValue += neighbourCell2.Answer;
                        }
                        else if (CellIsFree(neighbourCell2))
                        {
                            regionValue += neighbourCell1.Answer;
                        }
                        else
                        {
                            regionValue +=
                                (neighbourCell1.Answer + neighbourCell2.Answer);
                        }
                        break;
                    case Operator.Sub:
                        operationValue = "(-)";
                        // Если рандомизатор выдает число 2, то правило для области - вычитание
                        if (CellIsFree(neighbourCell1))
                        {
                            regionValue -= neighbourCell2.Answer;
                        }
                        else if (CellIsFree(neighbourCell2))
                        {
                            regionValue -= neighbourCell1.Answer;
                        }
                        else
                        {
                            regionValue -=
                                (neighbourCell1.Answer + neighbourCell2.Answer);
                        }
                        break;
                    case Operator.Mul:
                        operationValue = "(*)";
                        // Если рандомизатор выдает число 3, то правило для области - произведение
                        if (CellIsFree(neighbourCell1))
                        {
                            regionValue *= neighbourCell2.Answer;
                        }
                        else if (CellIsFree(neighbourCell2))
                        {
                            regionValue *= neighbourCell1.Answer;
                        }
                        else
                        {
                            regionValue *=
                                (neighbourCell1.Answer * neighbourCell2.Answer);
                        }
                        break;
                    case Operator.Div:
                        operationValue = "(/)";
                        // Если рандомизатор выдает число 4, то правило для области - division
                        // To Do.
                        break;
                    default:
                        // nothing to do.
                        break;
                }
            }

            textCell[primaryCell.RowIndex][primaryCell.ColumnIndex].Text = $"{regionValue}{operationValue}";
            
            // populate region of cells with data.
            region.RegionValue = regionValue;
            region.Operation = regionOperation;

            regions.Add(region);
        }
        #endregion

        #region Event Handlers
        private bool CellIsFree(int rowIndex, int columnIndex)
        {
            return map[rowIndex, columnIndex] == FreeCell;
        }

        private bool CellIsFree(Cell cell)
        {
            return cell == null ||
                map[cell.RowIndex, cell.ColumnIndex] == FreeCell;
        }

        // Помечаем клетку как задействованную в области
        private void OccupyCell(int rowIndex, int columnIndex)
        {
            map[rowIndex, columnIndex] = OccupiedCell;
        }

        private void OccupyCell(Cell cell)
        {
            map[cell.RowIndex, cell.ColumnIndex] = OccupiedCell;
        }

        private void Form1_Load(object sender, EventArgs e) {}

        // При нажатии на кнопку завершения
        private void CompleteButton_Click(object sender, EventArgs e)
        {
            int f = 0;
            string[,] answer = new string[MapSize, MapSize];

            for(int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    answer[i, j] = (textCell[i][j].Text);
                }
            }

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if (answer[i, j] != correct[i, j].ToString())
                        f = 1;
                }
            }

            MessageBox.Show(f == 1 ? "Incorrect!" : "Correct!");
        }

        //Автоматическое решение
        private void AutoComleteButton_Click(object sender, EventArgs e)
        {
            
        }
        #endregion
    }
}
