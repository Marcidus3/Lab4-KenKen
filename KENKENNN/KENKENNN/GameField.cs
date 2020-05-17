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
using System.Runtime.InteropServices;

namespace KENKENNN
{
    public partial class GameField : Form
    {
      
        // Экземпляр класса рандомизатора
        private Random randomGenerator = new Random();

        // Создаем массив текстбоксов 
        private TextBox[][] textCell = new TextBox[MapSize][];

        // Создем массив, в который будем хранить информацию о том
        // Задействованы ли клеточки в областях
        private int[,] map = new int[MapSize, MapSize];
        // Создаем массив с ответами
        private int[,] correct = new int[MapSize, MapSize];

        private string[,] regionData = new string[MapSize, MapSize];

        private string[,] regionOperator = new string[MapSize, MapSize];

        Dictionary<string, Tuple<string, int>> RegInf = new Dictionary<string, Tuple<string, int>>();

        fileManager fileManager;

        //Создаем новую задачу
        private Problem problem;


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public GameField()
        {
            //Инифиализируем задачу
            problem = new Problem();
            InitializeComponent();
            AllocConsole();
            InitializeBoard();

        }


        #region GameCreation
        private void InitializeBoard()
        {
            
            //Прозодимся по всем клеточкам поля
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    //Помечаем их как свободные
                    map[i, j] = FreeCell;
                }
            }
            /*
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
            */

            //Создание отображения поля игры
            CreateMap();
            //Создание регионов
           // GetRegions();
          //  GenerateRegions();
           
            Generate();
        }

        // Функция создания поля игры
        private void CreateMap()
        {
            // Задаем размер окна
            Width = MapSize * CellSize + CellSize / 2; 
            Height = MapSize * CellSize + 2 * CellSize; 

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
          //  CompleteButton.Location = new Point(0, Height - 2 * CellSize);
          //  AutoComleteButton.Location = new Point(Width / 2, Height - 2 * CellSize);
        }

        // Функиця генерации разделения игрового поля на области 


        private void GetPuzzle()
        {
            fileManager = new fileManager();
            fileManager.GetData(@"C:\Users\ruichernob\Desktop\Projects\Lab4-KenKen\KENKENNN\KENKENNN\Data\Puzzle.xlsx");

            //В цикле просматриваем таблицу  
            for (int i = 0; i < MapSize ; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    correct[i, j] = Convert.ToInt32(fileManager.sheet.Cells[i+1, j+1].value);
                    Console.Write(correct[i,j].ToString()+"  ");
                }
                Console.Write("\n");
            }
            fileManager.CloseFile();

          }
        private void GetRegions()
        {
            fileManager = new fileManager();
            fileManager.GetData(@"C:\Users\ruichernob\Desktop\Projects\Lab4-KenKen\KENKENNN\KENKENNN\Data\Regions1.xlsx");

            //В цикле просматриваем таблицу  
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    regionData [i,j]= fileManager.sheet.Cells[i + 1, j + 1].value.ToString();
                    Console.Write(regionData[i, j] + "  ");
                }
                Console.Write("\n");
            }
            fileManager.CloseFile();
        }
        private void GetRegionData()
        {
            fileManager = new fileManager();
            fileManager.GetData(@"C:\Users\ruichernob\Desktop\Projects\Lab4-KenKen\KENKENNN\KENKENNN\Data\RegionData3.xlsx");

            int value;
            string operat;
            string name;
            //В цикле просматриваем таблицу  
     
              for (int j = 1; j <= 12; j++)
                {
                  name = (fileManager.sheet.Cells[1, j]).value.ToString();
                    operat = fileManager.sheet.Cells[2, j ].value.ToString();
                    value = Convert.ToInt32(fileManager.sheet.Cells[3, j].value);
                    var temp = new Tuple<string, int>(operat, value);
                    RegInf.Add(name,temp);
                    Console.Write(name+"   "+operat+"   "+value.ToString());
                    Console.Write("\n");
            }
                
            fileManager.CloseFile();
        }

        private void Generate()
        {
            GetPuzzle();
            GetRegions();
            GetRegionData();

            foreach (var regionAbbreviature in RegInf.Keys)
            {
                var regionInfo = RegInf[regionAbbreviature];
                var regionOp = Operator.Const;
                var regionVal = regionInfo.Item2;
                var regionHint = string.Empty;

                switch (regionInfo.Item1)
                {
                    case "sum":
                        regionOp = Operator.Add;
                        regionHint = "+";
                        break;
                    case "sub":
                        regionOp = Operator.Sub;
                        regionHint = "-";
                        break;
                    case "multi":
                        regionOp = Operator.Mul;
                        regionHint = "*";
                        break;
                    case "div":
                        regionHint = "/";
                        break;
                    default:
                        break;
                }

                var region = new Region(regionOp, regionVal);
                bool primaryCellHintAdded = false;

                for (int i = 0; i < MapSize; i++)
                {
                    for (int j = 0; j < MapSize; j++)
                    {
                        if (regionData[i, j] == regionAbbreviature)
                        {
                            if (!primaryCellHintAdded)
                            {
                                textCell[i][j].Text = $"{regionVal} {regionHint}";
                                primaryCellHintAdded = true;
                            }

                            textCell[i][j].BackColor = RegionColors2[regionData[i, j]];
                            OccupyCell(i, j);

                            problem.Regions[i, j] = region;
                        }
                    }
                }
            }
        }
        /*
        private void GetRegions()
        {
            var lines = File.ReadAllLines(@"../../Data/RegionData.txt");
            var cells = new string[lines.Length][];

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = lines[i].Split(
                    new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            }

             int currentReg=1;
            int countReg = 1;
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                   
                    regionData[i, j] = int.Parse(cells[i][j]);

                    if (currentReg != regionData[i, j])
                    {
                        countReg++;
                        currentReg++;
                    }
                }
            }

            var lines1 = File.ReadAllLines(@"../../Data/OperatorData.txt");
            var cells1 = new string[lines1.Length][];

            for (int i = 0; i < cells1.Length; i++)
            {
                cells1[i] = lines1[i].Split(
                    new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            }

        
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {

                    regionOperator[i, j] = cells1[i][j].ToString();
                  
                   
                }
            }


               int[] regValues=new int[] { 8, 9 ,9 ,3, 9, 3 ,1, 5, 12 ,5, 8, 4, 1 };
            int[,] regValMatrix = new int[MapSize, MapSize];



            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    regValMatrix[i, j] = regValues[regionData[i,j]-1];// MessageBox.Show(regValMatrix[i, j].ToString());
                }
            }


                  var lines2 = File.ReadAllLines(@"../../Data/ValueData.txt");
            var cells2 = new string[lines.Length][];

            for (int i = 0; i < cells.Length; i++)
            {
                cells1[i] = lines[i].Split(
                    new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            }


            Operator opvalue;

            countReg = 0;
         


                for (int i = 0; i < MapSize; i++)
                {
                    for (int j = 0; j < MapSize; j++)
                    {
                        //
                        switch (regionOperator[i, j])
                        {
                            case "+":
                                opvalue = Operator.Add;
                                textCell[i][j].Text = regValMatrix[i, j].ToString() + "(+)";
                                break;
                            case "-":
                                opvalue = Operator.Sub;
                                textCell[i][j].Text = regValMatrix[i, j].ToString() + "(-)";
                                break;
                            case "*":
                                opvalue = Operator.Mul;
                                textCell[i][j].Text = regValMatrix[i, j].ToString() + "(*)";
                                break;
                            case "/":
                                opvalue = Operator.Div;
                                textCell[i][j].Text = regValMatrix[i, j].ToString() + "(/)";
                                break;
                            default:
                                textCell[i][j].Text = regValMatrix[i, j].ToString();
                                opvalue = Operator.Const;
                                break;
                        }

                        AddToReg(opvalue, regValMatrix[i, j], i, j);
                        //



                    }
                }
            


        }*/

        private void AddToReg(Operator regionOperation, int regionValue,int index, int jindex)
        {
             
            var region = new Region(regionOperation, regionValue);
            problem.Regions[index, jindex] = region;
          //  foreach (var reg in problem.Regions)
          //  {

              //  Console.Write(region.Value.ToString() + region.Operator.ToString() + "\n");
           // }
           // Console.Write("-----\n");
        }


        private void GenerateRegions()
        {
            // Счетчик количества областей
            var regionsCount = 0;

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    //Красим клетку 
                    var cellColor = RegionColors[regionsCount];

                    //Создаем для данной клетки новый объект 
                    var cell = new Cell(i, j);

                    //Новые объекты - соседние клетки, которые будут в регионе
                    Cell neighbourCcell1 = null, neighbourCell2 = null;

                    // Данное значение отвечает за основную клетку области
                    cell.Answer = correct[i, j];

                    // Если клетка не относится ни к одной из областей
                    if (CellIsFree(cell))
                    {
                        // Красим клетку в цвет, соответствующий региону
                        textCell[i][j].BackColor = cellColor;

                        //Помечаем клетку как занятую
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
                                //Помечаем клетку как занятую
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
                                //Помечаем клетку как занятую
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
            //Рандомно выбираем операцию
            var regionOperation = (Operator)randomGenerator.Next((int)Operator.Add, (int)Operator.Div);
            var operationValue = string.Empty;

            //Основное значение региона
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
                    //Если оператор +
                    case Operator.Add:
                        operationValue = "(+)";
                     
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
                        //Если оператор - 
                    case Operator.Sub:
                        operationValue = "(-)";
                        int max, sum;
                        
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
                            {
                                regionOperation = Operator.Add;
                                operationValue = "(+)";
                                regionValue = sum;
                            }
                        }
                        break;
                        //Если оператор *
                    case Operator.Mul:
                        operationValue = "(*)";
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
                        //Если оператор /
                    case Operator.Div:
                        operationValue = "(/)";
                        break;
                    default:
                        break;
                }
            }

            //Заполняем значения основной клетки региона полученным
            textCell[primaryCell.RowIndex][primaryCell.ColumnIndex].Text = regionValue.ToString() + operationValue.ToString(); 

            //Создаем новый регион с данными значениями и оператором
            var region = new Region(regionOperation, regionValue);
        
            //Добавляем регион в список регионов
            problem.Regions[primaryCell.RowIndex, primaryCell.ColumnIndex] = region;

            if (neighbourCell1 != null)
            {
                //Помечаем клетку рядом, как входящую в регион
                problem.Regions[neighbourCell1.RowIndex, neighbourCell1.ColumnIndex] = region;
            }
            if (neighbourCell2 != null)
            {
                //Помечаем клетку рядом, как входящую в регион
                problem.Regions[neighbourCell2.RowIndex, neighbourCell2.ColumnIndex] = region;
            }
        }

        //Функция определения состояния клетки
        private bool CellIsFree(int rowIndex, int columnIndex)
        {
            return map[rowIndex, columnIndex] == FreeCell;
        }
        //Функция определения состояния клетки
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

        #region Buttons
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
            var res = problem.Solve();
            //Проходимся по всем решениям
            if (res != null)
            {
                for (int rowIx = 0; rowIx < MapSize; rowIx++)
                {
                    for (int colIx = 0; colIx < MapSize; colIx++)
                    {
                        var cell = textCell[rowIx][colIx];
                        //Заполняем клетки
                        cell.Text = res.State[rowIx, colIx].ToString();
                        cell.Font = new Font(FontFamily.GenericMonospace, 25, FontStyle.Bold);
                    }
                }
            }
        }
        #endregion
    }
}
