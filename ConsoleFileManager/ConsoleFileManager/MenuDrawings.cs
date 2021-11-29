using System;
using System.Collections.Generic;
using System.Text;

//В классе содержатся методы для отрисовки в консоли пользовательского интерфейса.

namespace ConsoleFileManager
{
    class MenuDrawings
    {
        static public void DrawHorizontalLine ()
        {
            for (int i = 0; i < Console.BufferWidth; i++)
            {
                Console.Write('-');
            }
            Console.WriteLine(System.Environment.NewLine);
        }

        static public void DrawInfo(string _name, string _size, string _creationTime, string _lastChangeTime)
        {
            Console.WriteLine($"{_name}");
            Console.WriteLine($"{_size}");
            Console.WriteLine($"{_creationTime}");
            Console.WriteLine($"{_lastChangeTime}");
            Console.WriteLine("Нажмите любую клавишу");
            Console.ReadKey();
        }
    }
}
