using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ConsoleFileManager
{
    class FilesAndDirectories
    {
        //метод по получению списка файлов и директорий
        static public string[] GetDirectories(string _path)
        {
            //Контроль ошибок доступа к каталогам
            try
            {

                // получение массива директорий и файлов в директории, а также массива, куда будут также записываться поддиректории
                string[] dirs = Directory.GetDirectories(_path);
                string[] files = Directory.GetFiles(_path);
                string[] dirfilesMass = new string[0];

                //проход директорий и того, что ниже на 1 уровень
                foreach (string s in dirs)
                {
                    string[] str = s.Split(@"\");

                    ChangeDirFilesMass(ref dirfilesMass, $"DIR {str[str.Length - 1]}\\");

                    string[] subdirs = Directory.GetDirectories(s);

                    // проход по поддиректориям
                    foreach (string b in subdirs)
                    {
                        string[] substr = b.Split(@"\");
                        ChangeDirFilesMass(ref dirfilesMass, $" - DIR {substr[substr.Length - 1]}\\");
                    }

                    string[] subfiles = Directory.GetDirectories(s);

                    //проход по файлам в поддиректориях
                    foreach (string f in files)
                    {
                        string[] substr = f.Split(@"\");
                        ChangeDirFilesMass(ref dirfilesMass, $" - FL {substr[substr.Length - 1]}");
                    }
                }

                //Проход по файлам в директории
                foreach (string f in files)
                {
                    string[] str = f.Split(@"\");
                    ChangeDirFilesMass(ref dirfilesMass, $"FL {str[str.Length - 1]}");
                }

                return dirfilesMass;
            }
            catch (Exception ex)
            {
                Program.SaveErrors(ex);
                string[] dirfilesMass2 = new string[0];
                return dirfilesMass2;
            }
        }

        //метод по записи в массив директорий, поддиректорий и файлов нового значения, увеличения массива на 1.
        static public void ChangeDirFilesMass(ref string[] _dirfilesMass, string _newline)
        {
            Array.Resize(ref _dirfilesMass, _dirfilesMass.Length + 1);
            _dirfilesMass[_dirfilesMass.Length - 1] = _newline;
        }


        //вывод на экран определенной страницы из списка файлов и директорий
        static public void ShowPage(int _scale, int _page, string[] _dirfilesMass)
        {
            for (int i = (_scale * _page - _scale); i < _scale * _page; i++)
            {
                if (i < _dirfilesMass.Length)
                {
                    Console.WriteLine(_dirfilesMass[i]);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
