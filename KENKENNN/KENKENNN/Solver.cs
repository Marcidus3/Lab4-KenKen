using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KENKENNN
{
    class Solver
    {
        //Не то
        /*
        public bool IsInRow(int[,] map,int size,int row,int number) 
        {
            for(int i=0;i< size; i++)
            {
                if (map[row, i] == number)
                    return true;
            }

            return false;
        }

        public bool IsInCol(int[,] map, int size, int col, int number)
        {
            for (int i = 0; i < size; i++)
            {
                if (map[i, col] == number)
                    return true;
            }

            return false;
        }

    

        public bool IsItOk(int[,] map, int size, int col, int number, int row)
        {
            return !IsInRow(map, size, row, number) && !IsInCol(map, size, col, number) ;
        }

        public bool Solve(int[,] map, int size)
        {
            for(int row = 0; row < size; row++)
            {
                for(int col = 0; col < size; col++)
                {
                    if (map[row, col] == 0)
                    {
                        for(int num = 1;num<= size; num++)
                        {
                            if (IsItOk(map, size, col, num, row))
                            {
                                map[row, col] = num;

                                if (Solve(map,size))
                                {
                                    return true;
                                }
                                else
                                {
                                    map[row, col] = 0;
                                }
                            }
                        }

                        return false;
                    }
                }
            }
            return true;
        }

    */
    }
}
