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
using System.Linq;

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
        private int[,] map = new int[MapSize, MapSize];
        // Создаем массив с ответами
        private int[,] correct = new int[MapSize, MapSize];
        private Problem problem;
        #endregion

        #region Construction
        /// <summary>
        /// Construction.
        /// </summary>
        public GameField()
        {
            #region To remove
            //var r = new Region();
            //r.RegionValue = 2;
            //r.Operation = Operator.Sub;
            //r.Cells.Add(new Cell(0, 0));
            //r.Cells.Add(new Cell(0, 0));
            //r.Cells.Add(new Cell(0, 0));

            //try
            //{
            //    r.FindCandidates();

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message); ;
            //}

            //return;
            #endregion
            problem = new Problem();
            InitializeComponent();
            InitializeBoard();
        }
        #endregion

        #region Auxiliary methods
        private void InitializeBoard()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    map[i, j] = FreeCell;
                }
            }

            // Открытие и прочтение файла, на основе которого генерируется игра
            var lines = File.ReadAllLines(@"../../Data/PuzzleData.txt");
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
                    var cell = new Cell(i, j);
                    Cell neighbourCcell1 = null, neighbourCell2 = null;

                    // Данное значение отвечает за основную клетку области
                    cell.Answer = correct[i, j];

                    // Если клетка не относится ни к одной из областей
                    if (CellIsFree(cell))
                    {
                        // Красим клетку в цвет, соответствующий региону
                        textCell[i][j].BackColor = cellColor;
                        OccupyCell(cell);

                        // Если клетка не на границе & клетка рядом свободна
                        if (i < MapSize - 1)
                        {
                            if (CellIsFree(i + 1, j))
                            {
                                neighbourCcell1 = new Cell(i + 1, j);

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
                            if (CellIsFree(i, j + 1))
                            {
                                neighbourCell2 = new Cell(i, j + 1);

                                // Помечаем значение соседней клетки, записываем туда 
                                // Правильный ответ в этой клетке
                                neighbourCell2.Answer = correct[i, j + 1];

                                // Красим в цвет основной клетки
                                textCell[i][j + 1].BackColor = cellColor;
                                OccupyCell(neighbourCell2);
                            }
                        }

                        //Создание правила для сформированной области
                        MakeRule(cell, neighbourCcell1, neighbourCell2);
                        regionsCount++;
                    }
                }
            }
        }

        // Создание правила для области игрового поля
        // Сюда передаются значения основной и соседних клеточек, а так же их индексы
        private void MakeRule(Cell primaryCell, Cell neighbourCell1, Cell neighbourCell2)
        {
            // Рандомизатор целых чисел, выдает число от 1 до 3х
            var regionOperation = (Operator)randomGenerator.Next((int)Operator.Add, (int)Operator.Div);
            var operationValue = string.Empty;
            var regionValue = primaryCell.Answer;

            // Если область состоит только из одной клеточки, то заполняем 
            // ее числом из ответа, как указано в правилах игры
            if (CellIsFree(neighbourCell1) && CellIsFree(neighbourCell2))
            {
                regionOperation = Operator.Const;
            }
            else
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
                        int max, sum;
                        // Если рандомизатор выдает число 2, то правило для области - вычитание
                        if (CellIsFree(neighbourCell1))
                        {
                            sum = primaryCell.Answer + neighbourCell2.Answer;
                            max = Math.Max(primaryCell.Answer, neighbourCell2.Answer);
                            regionValue = max - (sum - max);
                        }
                        else if (CellIsFree(neighbourCell2))
                        {
                            sum = primaryCell.Answer + neighbourCell1.Answer;
                            max = Math.Max(primaryCell.Answer, neighbourCell1.Answer);
                            regionValue = max - (sum - max);
                        }
                        else
                        {
                            sum = primaryCell.Answer + neighbourCell1.Answer + neighbourCell2.Answer;
                            //max = Math.Max(primaryCell.Answer, Math.Max(neighbourCell1.Answer, neighbourCell2.Answer));
                            //regionValue = max - (sum - max);
                            //if (regionValue < 0)
                            {
                                regionOperation = Operator.Add;
                                operationValue = "(+)";
                                regionValue = sum;
                            }
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

            //problem.Regions[0, 0] = problem.Regions[0, 1] = problem.Regions[1, 0] = new Region(Operator.Mul, 8);
            //problem.Regions[0, 2] = problem.Regions[0, 3] = problem.Regions[1, 2] = new Region(Operator.Add, 9);
            //problem.Regions[1, 1] = problem.Regions[2, 1] = new Region(Operator.Sub, 1);
            //problem.Regions[1, 3] = problem.Regions[2, 3] = new Region(Operator.Add, 7);
            //problem.Regions[2, 0] = problem.Regions[3, 0] = new Region(Operator.Mul, 12);
            //problem.Regions[2, 2] = problem.Regions[3, 2] = new Region(Operator.Mul, 2);
            //problem.Regions[3, 1] = new Region(Operator.Const, 3);
            //problem.Regions[3, 3] = new Region(Operator.Const, 1);
            //return;
            var region = new Region(regionOperation, regionValue);
            problem.Regions[primaryCell.RowIndex, primaryCell.ColumnIndex] = region;

            if (neighbourCell1 != null)
            {
                problem.Regions[neighbourCell1.RowIndex, neighbourCell1.ColumnIndex] = region;
            }
            if (neighbourCell2 != null)
            {
                problem.Regions[neighbourCell2.RowIndex, neighbourCell2.ColumnIndex] = region;
            }
        }

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
        #endregion

        #region Event Handlers
        private void Form1_Load(object sender, EventArgs e) {}

        // При нажатии на кнопку завершения
        private void CompleteButton_Click(object sender, EventArgs e)
        {
            int f = 0;

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if (textCell[i][j].Text != correct[i, j].ToString())
                    {
                        f = 1;
                        break;
                    }
                }
            }

            MessageBox.Show(f == 1 ? "Incorrect!" : "Correct!");
        }

        //Автоматическое решение
        private void AutoComleteButton_Click(object sender, EventArgs e)
        {
            var message = "Incorrect!";
            var res = problem.Solve();
            if (res != null)
            {
                for (int rowIx = 0; rowIx < MapSize; rowIx++)
                {
                    for (int colIx = 0; colIx < MapSize; colIx++)
                    {
                        var cell = textCell[rowIx][colIx];
                        cell.Text = res.State[rowIx, colIx].ToString();
                        cell.Font = new Font(FontFamily.GenericMonospace, 25, FontStyle.Bold);
                    }
                }
                message = "Correct!";
            }

            MessageBox.Show(message);
        }
        #endregion
    }
}
