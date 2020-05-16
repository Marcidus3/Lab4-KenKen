using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace KENKENNN
{
    //Менеждер по работе с файлами
    class fileManager
    {
        //Лист, открытый на данный момент
        public Excel.Worksheet sheet;

        Excel.Application xlApp;
        Excel.Workbook xlWorkBook;

        //Данный метод открывает файл по данному пути
        public void GetData(string path)
        {
 
            xlApp = new Excel.Application();
            Excel.Worksheet xlWorkSheet;

            //Открываем книгу Excel по данному пути
            xlWorkBook = xlApp.Workbooks.Open(path);
            //Открываем первый лист
            xlWorkSheet = xlWorkBook.Worksheets.get_Item(1);
            //Передаем его в переменную - лист на данный момент
            sheet = xlWorkSheet;
           

        }

        //Метод для прекращения работы с файлом
        public void CloseFile()
        {
            xlWorkBook.Close(1);
            xlApp.Quit();
            sheet = null;

        }


    }
}
