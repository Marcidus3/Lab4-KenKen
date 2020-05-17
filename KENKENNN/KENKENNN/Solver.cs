using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static KENKENNN.EnumUtils;

namespace KENKENNN
{
    //Класс объекта - доски
    public class KenKenBoard
    {
        //Массив, в который будет записываться предполагаемый ответ 
        public int[,] State { get; private set; } = new int[Constants.MapSize, Constants.MapSize];

        //Массивы отслеживают наличие чисел в строке/столбце
        //Первый индекс - номер строки, второй - само число 
        bool[,] rowHash = new bool[Constants.MapSize, Constants.MapSize];    
        //Первый индекс - номер строки, второй - само число 
        bool[,] colHash = new bool[Constants.MapSize, Constants.MapSize];

        //Метод, определяющий, можно ли расположить данное число на данных координатах
        public bool CanPlace(Point p, int val)
        {
            //Необходимое для индексации преобразование
            int valMinusOne = val - 1;

            //Если данные ряды и стобцы уже заняты (=true), то расположить нельзя
            return !rowHash[p.Y, valMinusOne] && !colHash[p.X, valMinusOne];
        }


        //Отменяет знятость сток/столбцов
        public void UnsetPlace(Point p)
        {
            //Необходимое для индексации преобразование
            int valMinusOne = State[p.X, p.Y] - 1;

            //Теперь данные строки и столбцы свободны 
            rowHash[p.Y, valMinusOne] = false;
            colHash[p.X, valMinusOne] = false;
            State[p.X, p.Y] = 0;
        }
        //Попытка занять данным значением клетку
        public bool TryPlace(Point p, int val)
        {
            //Необходимое для индексации преобразование
            int valMinusOne = val - 1;

            //Если априори нельзя поставить, то сразу выходим из функции
            if (!CanPlace(p, val))
            {
               
                return false;
            }

            
            //Если можно, то помечаем строку и стобец как недоступные для данного числа 
            rowHash[p.Y, valMinusOne] = true;
            colHash[p.X, valMinusOne] = true;
            //Отмечаем в матрице ответов как занятое
            State[p.X, p.Y] = val;

            //----------
            return true;
        }
    }





    //Класс объекта - игровой регион
    public class Region
    {
        //Действие, которое выполняется на регионе (+,-..)
        public Operator Operator { get; set; }

        //Значение, полученное в ходе выполнения операции на регионе
        public int Value { get; set; }

        //Ячейки в регионе
        public List<Point> Cells { get; private set; } = new List<Point>();

        //Решения региона
        public List<List<int>> Solutions { get; private set; }

        public Region(Operator op, int val)
        {
            Operator = op;
            Value = val;
        }

        public override string ToString()
        {
            return $"[SubProblem: Operator={Operator}, Value={Value}, Squares={Cells}]";
        }

        //Фильтрация и уменьшение количества решений в соответствии с данным полем
        public Region Prune(KenKenBoard board)
        {
            //Берем новый регион
            var result = new Region(Operator, Value);
            
            //Создаем для данного региона новый список решений
            result.Solutions = new List<List<int>>();

            //Создаем новые ячейки в данном регионе
            result.Cells = Cells;

            //Просмотр каждого решения
            foreach (var solution in Solutions)
            {
                bool success = true;
             
                //Проходимся по клеткам данного региона

                for (int i = 0; i < Cells.Count; i++)
                {  
                    //Если хоть одну клетку решения нельзя расположить на доске
                    if (!board.CanPlace(Cells[i], solution[i]))
                    {
                        //То выходим из цикла, это решение не подошло
                        //Его не добавим к списку отфильтрованных
                        success = false;
                        break;
                    }
                }
                //Если получилось расположить, то добавялем в список 
                if (success)
                    result.Solutions.Add(solution);
            }
            //Если ни одно решение не подошло, то возвращаем null, иначе готовый решенный регион
            return Solutions.Any() ? result : null;
        }

        //Проверка на правильность предлагаемого решения
        public static bool IsValid(int val)
        {
            return val > 0 && val <= Constants.MapSize;
        }

        //Здесь происходит перебор всех возможных решений областей с законом - сложение
        bool AddExplore(List<int> solutionCandidates, int currentVal)
        {
            
            //Данное значение всегда должно быть меньше значения области, так как в противном случае сумма не получится
            if (currentVal > Value)
                return true;

            //Если список решений пуст, то создаем новый
            if (solutionCandidates == null)
                solutionCandidates = new List<int>();

            //Если в списке решений на данный момент остается одно свободное место, ищем финальное число
            if (solutionCandidates.Count == Cells.Count - 1)
            {
                
                //Т.к. ищем решения для суммы, вычитаем из значения региона данное значение
                int val = Value - currentVal;

                //Если полученное число может пристутствовать в решении, то
                if (IsValid(val))
                {
                    List<int> copy = new List<int>(solutionCandidates);
                    copy.Add(val);
                    //Добавляем в общий список решений данное
                    Solutions.Add(copy);
                   
                    return true;
                }
                //Если числа, которые предлагается добавить к решению слишком большие, то вернется false
                return val <= Constants.MapSize;
            }

            //Подставляем числа из доступных
            for (int i = Constants.MapSize; i >= 1; i--)
            {
                //Сразу добавляем число в список
                solutionCandidates.Add(i);
                //Рекурсивно вызываем функцию
                bool ret = AddExplore(solutionCandidates, currentVal + i);
                //Предпоследнее полученное число убираем
                solutionCandidates.RemoveAt(solutionCandidates.Count - 1);
                //Прерываем просмотр потенциальных значений, так как они больше не будут подходить
                if (!ret)
                    break;
            }
            return true;
        }
        //Здесь происходит перебор всех возможных решений областей с законом - сложение
        bool MultExplore(List<int> solutionCandidates, int currentVal)
        {
            //Данное значение всегда должно быть меньше значения области, так как в противном случае произведение не получится
            if (currentVal > Value)
                return true;


            //Если список решений пуст, то создаем новый
            if (solutionCandidates == null)
                solutionCandidates = new List<int>();

            //Если в списке решений на данный момент остается одно свободное место, ищем финальное число
            if (solutionCandidates.Count == Cells.Count - 1)
            {
                //Находим число как отношение, так как оператор - произведение
                int val = Value / currentVal;
                //Если полученное число может пристутствовать в решении, то
                if (IsValid(val))
                {   
                    if (Value % currentVal != 0)
                        return true;
                    List<int> copy = new List<int>(solutionCandidates);
                    copy.Add(val);
                    //Добавляем в общий список решений данное
                    Solutions.Add(copy);

                    return true;
                }
                return val <= Constants.MapSize;
            }
            //Подставляем числа из доступных
            for (int i = Constants.MapSize; i >= 1; i--)
            {
                //Сразу добавляем число в список
                solutionCandidates.Add(i);
                //Рекурсивно вызываем функцию
                bool ret = MultExplore(solutionCandidates, currentVal * i);
                //Предпоследнее полученное число убираем
                solutionCandidates.RemoveAt(solutionCandidates.Count - 1);
                //Прерываем просмотр потенциальных значений, так как они больше не будут подходить
                if (!ret)
                    break;
            }
            return true;
        }

        //Генерация возможных решений
        public void GenerateSolutions()
        {
           
            //Если уже есть решения, больше создавать не надо, выходим
            if (Solutions != null)
            {
                
                return;
            }
               

            //Объявляем список решений
            Solutions = new List<List<int>>();
            

            //Смотрим, какой закон у данного региона (+,-...)
            switch (Operator)
            {
                //Если значения в данном регионе складываются
                case Operator.Add:
                    //Рассматриваем все возможные решения области
                    AddExplore(null, 0);
                    break;
                    //Если значения в регионе вычитаются
                case Operator.Sub:
                    //Для всех доступных значений 
                    for (int a = 1; a <= Constants.MapSize; a++)
                    {
                        //Находим разность значения региона и данного
                        int b = Math.Abs(Value - a);
                        //Если получилось подходящее значение, то
                        if (IsValid(b))
                        {
                            //Добавляем эти значения в список ответов
                            Solutions.Add(new List<int> { a, b });
                            Solutions.Add(new List<int> { b, a });
                        }
                    }
                    break;
                    //Если значения в регионе делятся
                case Operator.Div:
                    //Проходимся по всем доступным на поле значениям 
                    for (int a = 1; a <= Constants.MapSize; a++)
                    {
                        //Находим отношение данного знаечния и значения региона
                        int b = a / Value;
                        //Если значение подходящее, то добавляем его в список решений
                        if (IsValid(b) &&
                            (a % Value == 0))
                        {
                            Solutions.Add(new List<int> { a, b });
                            Solutions.Add(new List<int> { b, a });
                        }
                    }
                    break;
                    //Если значения в регионе умножаются
                case Operator.Mul:
                    //Перебором находим варианты решений
                    MultExplore(null, 1);
                    break;
                case Operator.Const:
                    //Если регион состоит из одной клетки, то туда по правилам записываем уже имеющееся значение    
                    Solutions.Add(new List<int> { Value });
                    break;
            }
        }

    }

    //Класс объекта - задание
    public class Problem
    {
        //Задание состоит из списка регионов
        List<Region> AllRegions = new List<Region>();

        // матрица регионов [MapSize x MapSize]
        public Region[,] Regions { get; } =
            new Region[Constants.MapSize, Constants.MapSize];

        //Фильтрация решений, которые больше не подходят для данной доски
        //Принимает ситуацию на доске на данный момент
        public Problem Prune(KenKenBoard board)
        {
            var result = new Problem();

            //Для всех регионов
            for (int i = 1; i < AllRegions.Count; i++)
            {
                //Объявляем данный регион 
                var region = AllRegions[i];

                //Для данного региона отбрасываем недоходящие решения
                var resultRegion = region.Prune(board);

                //Если для данного региона не найдено решений
                //То такой вариант решения не подошел
                if (resultRegion == null)
                    return null;

                //Иначе добавляем решение такого региона в список
                result.AllRegions.Add(resultRegion);
            }

            return result;
        }

        //Эта функция решает задачу, когда найдены доступные решения, отсортированы, отфильтрованы
        public KenKenBoard Solve(KenKenBoard board)
        {
            //Есть ли в списке регионы
            if (!AllRegions.Any())
                return board;

            //Берем один регион
            var region = AllRegions[0];

            //Берем его клетки
            var cells = region.Cells;

            //Проходим через все возможные решения для данного региона
            foreach (var solution in region.Solutions)
            {
                //Проходим по каждому отдельному предлагаемому значению
                for (int i = 0; i < solution.Count; i++)
                {
                    //Пробуем расположить данное значение на поле
                    board.TryPlace(cells[i], solution[i]);
                }

                //Отфильтруем доску с данным решением
                var newProb = Prune(board);


                //Если полученное решенное задание не пустое
                if (newProb != null)
                {
                    newProb.Sort();

                    //Рекурсивно продолжаем искать решения для следующих областей
                    var ret = newProb.Solve(board);
                    if (ret != null)
                        return ret;
                }

                //Если вышло так, что алгоритм не смог найти подходящее решение
                //То все, что осталось от него на доске, очищаем
                foreach (Point p in cells)
                    board.UnsetPlace(p);
            }

           
            return null;
        }

        //Функция генерации решений, подготавливает почву для решения задачи
        public KenKenBoard Solve()
        {

            //Проверка всех клеток на причастие к региону
            for (int x = 0; x < Constants.MapSize; x++)
            {
                for (int y = 0; y < Constants.MapSize; y++)
                {
                    if (Regions[x, y] != null)
                    {
                       
                        //Если все ок, то побавляем данную клетку к списку клеток региона
                        Regions[x, y].Cells.Add(new Point(x, y));
                       
                    }
                  
                }
            }
         
            //Генерируем решения для всех регионов
            for (int x = 0; x < Constants.MapSize; x++)
            {
                for (int y = 0; y < Constants.MapSize; y++)
                {
                    Regions[x, y].GenerateSolutions();

                }
            }
       
            //Когда варианты ответов сгенерированы

            //Создаем словарь регионов
            var regions = new Dictionary<Region, int>();
           
            //Временная переменная
            Region tempSub;

            //Проходимся по всему игровому полю
            for (int x = 0; x < Constants.MapSize; x++)
            {
                for (int y = 0; y < Constants.MapSize; y++)
                {
                    //Передаем во временную переменную регион на данных координатах
                    tempSub = Regions[x, y];

                    //Если такого региона нет в словаре
                    if (!regions.ContainsKey(tempSub))
                    {
                        

                        //То доабвляем его
                        regions.Add(tempSub, tempSub.Cells.Count);
                    }
                }
            }

            //!!!!!!!!!!!!!
            //Проходимся по списку регионов
            foreach (var val in regions.Keys)
            {
               
                //Отбрасываем невозможные решения 

                //Для этого проходимся по всем возможным
                foreach (var solution in new List<List<int>>(val.Solutions))
                {
                 
                    //Временная переменная для хранения игрового поля на данный момент
                    var tempBoard = new KenKenBoard();
                    //Смотрим все доступные решения
                    for (int i = 0; i < solution.Count; i++)
                    {

                        //Тут что-то идет не так
                        //
                        //
                        //
                        //
                        Console.Write("\n"+solution[i]);
                        //Если при попытке расопложить такое значение в данном положении не вышло
                        if (!tempBoard.TryPlace(val.Cells[i], solution[i]))
                        {
                            
                            //То такое решение удаляем и выходим. Оно точно не будет использоваться
                            val.Solutions.Remove(solution);
                            break;
                        }

                        //
                        //
                        //
                    }
                }
               
                //После фильтрации этого региона, добавляем его к списку регионов
                AllRegions.Add(val);
                
            }

            Sort();
            
            //Пробуем решить задачу с полученными ресурсами, передаем пустое поле
            return Solve(new KenKenBoard());
        }

        //Сортирует список регионов по количеству валидных решений
        void Sort()
        {
            AllRegions.Sort(
                new Comparison<Region>((x, y) =>
                {
                    return x.Solutions.Count.CompareTo(y.Solutions.Count);
                }
            ));
        }
    }
}

