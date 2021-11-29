using System;
using System.IO;
using System.Text.Json;
using System.Text;

namespace ConsoleFileManager
{
    class Program
    {

        //Дефолтные установки приложения.
        static UserOptions userparam = new UserOptions();

        //Пути до пользовательского каталога с настройками и ошибками
        static StringBuilder pathToErrors = new StringBuilder();
        static StringBuilder pathToSettings = new StringBuilder();

        static void Main()
        {
            // Приветствие
            Console.WriteLine("Добро пожаловать в файловый менеджер!");
            Console.WriteLine("Нажмите любую клавишу для продолжения");
            Console.ReadKey();

            //Построение пути до пользовательских настроек     
            pathToSettings.Append(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            pathToSettings.Append(@"\ConsoleFileManager\Settings");
            string filePath = pathToSettings.ToString() + @"\appsettings.json";

            pathToErrors.Append(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            pathToErrors.Append(@"\ConsoleFileManager\Errors");
            Directory.CreateDirectory(pathToErrors.ToString());

            if (File.Exists(filePath))
            {
                //Проверка на "битые" настройки. В случае их неисправности, файл удаляется, настройки создаются "дефолтными"
                try
                {
                    //если пользовательский файл существует, то считываем настройки из него и заменяем дефолтные
                    string str = File.ReadAllText(filePath);
                    UserOptions userparamFromFile = JsonSerializer.Deserialize<UserOptions>(str);
                    userparam.UserName = userparamFromFile.UserName;
                    userparam.LastPathToDirectory = userparamFromFile.LastPathToDirectory;
                    userparam.FilesAndDirScale = userparamFromFile.FilesAndDirScale;
                    userparam.CurrentPage = userparamFromFile.CurrentPage;
                }
                catch (Exception ex)
                {
                    SaveErrors(ex);
                    File.Delete(filePath);
                    userparam = new UserOptions();
                }
            }
            else
            {
                //если пользовательского файла не существует, дефолтные сохраняем в файл.
                Directory.CreateDirectory(pathToSettings.ToString());
                File.Create(filePath).Close();
                string jsonser = JsonSerializer.Serialize(userparam);
                File.WriteAllText(filePath, jsonser);
            }

            //Отрисовка файлового менеджера с каталогами
            bool programStatus = true;
            while (programStatus)
            {
                //Очитска консоли перед выводом актуальных данных
                Console.Clear();

                // вывод на экран текущей директории
                Console.WriteLine(userparam.LastPathToDirectory + @"\");
                MenuDrawings.DrawHorizontalLine();

                //получение списка файлов и папок, включая вложенные, на основании параметров пользователя
                string[] listOfFilesDir = FilesAndDirectories.GetDirectories(userparam.LastPathToDirectory);

                //вывод страницы каталога
                FilesAndDirectories.ShowPage(userparam.FilesAndDirScale, userparam.CurrentPage, listOfFilesDir);

                MenuDrawings.DrawHorizontalLine();

                Console.Write("Введите команду: ");

                string userCommand = Console.ReadLine();

                ParseUserCommand(userCommand);

                //Сохранение пользовательских настроек после каждой отрисовки поля
                SaveUserOptions(filePath);
            }
        }

        //Метод для обработки команды пользователя
        static void ParseUserCommand(string _usercommand)
        {
            string[] str = new string[0];

            //Проверка на корректность введения команды. Необходим только 0 элемент
            try
            {
                string[] a = _usercommand.Split(' ');
                Array.Resize(ref str, a.Length);
                str = a;
            }
            catch (Exception ex)
            {
                SaveErrors(ex);
                Console.WriteLine("Введена неверная команда");
                Console.ReadKey();
            }


            switch (str[0])
            {
                case "exit":
                    {
                        Console.Write("Вы уверены, что хотите закончить работу в приложении? y/n ");
                        string s = Console.ReadLine();
                        if (s.ToUpper() == "Y")
                        {
                            Environment.Exit(0);
                        }
                        else if (s.ToUpper() == "N")
                        {
                            Console.WriteLine("Вы продолжите работу с приложением. Нажмите любую клавишу.");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("Команда введена неправильно. Нажмите любую клавишу.");
                            Console.ReadKey();
                        }
                        break;
                    }
                case "cd":
                    {

                        //Исключение команды cd и пробела
                        string s = "";
                        for (int i = 3; i < _usercommand.Length; i++)
                        {
                            s += _usercommand[i];
                        }

                        try
                        {
                            if (Directory.Exists(s))
                            {
                                userparam.LastPathToDirectory = s;
                            }
                            else
                            {
                                //Анализ команды пользователя на указание относительного адреса каталога
                                string[] dirs = Directory.GetDirectories(userparam.LastPathToDirectory);
                                foreach (string b in dirs)
                                {
                                    string[] dirsnames = b.Split(@"\");

                                    if (s.ToUpper() == dirsnames[^1].ToUpper())
                                    {
                                        userparam.LastPathToDirectory = $"{userparam.LastPathToDirectory}\\{s}";
                                        userparam.CurrentPage = 1;
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SaveErrors(ex);
                            Console.WriteLine("Неверно указана директория.");
                            Console.ReadKey();
                        }
                        break;
                    }
                case "page":
                    {
                        //Проверка на корректность введения команды
                        try
                        {
                            string s = "";
                            for (int i = (_usercommand.Length - 1); i >= 0; i--)
                            {
                                if (_usercommand[i] != ' ')
                                {
                                    s = _usercommand[i] + s;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            int pageNumber = Convert.ToInt32(s);

                            // проверка на наличие указанной страницы каталога в массиве. Изменение в пользовательских настройках текущей страницы каталога
                            string[] fd = FilesAndDirectories.GetDirectories(userparam.LastPathToDirectory);

                            //Целое число от деления в зависимости от настройки количества строк для вывода. А также анализ "хвоста"
                            int maxPageCount = fd.Length / userparam.FilesAndDirScale;
                            if (fd.Length % userparam.FilesAndDirScale > 0)
                            {
                                maxPageCount = maxPageCount + 1;
                            }

                            if (pageNumber <= maxPageCount)
                            {
                                userparam.CurrentPage = pageNumber;
                            }
                            else
                            {
                                Console.WriteLine("Указанной страницы не существует.");
                                Console.ReadKey();
                            }
                        }
                        catch (Exception ex)
                        {
                            SaveErrors(ex);
                            Console.WriteLine("Неверно указан номер страницы. Нажмите любую клавишу.");
                            Console.ReadKey();
                        }
                        break;
                    }
                case "info":
                    {
                        try
                        {
                            //Исключение команды и пробела
                            string path = "";
                            for (int i = 5; i < _usercommand.Length; i++)
                            {
                                path += _usercommand[i];
                            }

                            path.Trim();

                            FileAttributes fattPath = File.GetAttributes(path);

                            if ((fattPath & FileAttributes.Directory) == FileAttributes.Directory)
                            {

                                string size = Convert.ToString(GetDirSize(path));
                                string creationTime = Convert.ToString(GetDirCreationDate(path));
                                string lastChangeTime = Convert.ToString(GetDirLastChangeDate(path));

                            }
                            else if ((fattPath & FileAttributes.Archive) == FileAttributes.Archive)
                            {
                                FileInfo fileInfo = new FileInfo(path);
                                string size = Convert.ToString(fileInfo.Length);
                                string creationTime = Convert.ToString(fileInfo.CreationTime);
                                string lastChangeTime = Convert.ToString(fileInfo.LastWriteTime);
                                MenuDrawings.DrawInfo($"{path}", $"Размер - {size}", $"Дата создания - {creationTime}", $"Дата последнего изменения - {lastChangeTime}");
                            }

                        }
                        catch (Exception ex)
                        {
                            SaveErrors(ex);
                            Console.WriteLine("Непредвиденная ошибка");
                        }

                        break;
                    }
                case "copy":
                    {
                        try
                        {
                            //Исключение команды и пробела
                            string fromPath = "";
                            for (int i = 5; i < _usercommand.Length; i++)
                            {
                                fromPath += _usercommand[i];
                            }

                            Console.Write("Введите адрес копирования: ");

                            string toPath = Console.ReadLine().Trim();
                            fromPath.Trim();

                            FileAttributes fattFromPath = File.GetAttributes(fromPath);
                         
                            if ((fattFromPath & FileAttributes.Directory) == FileAttributes.Directory)
                            {
                                CopyDir(fromPath, toPath);
                            }
                            else if ((fattFromPath & FileAttributes.Archive) == FileAttributes.Archive)
                            {
                                File.Copy(fromPath, toPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            SaveErrors(ex);
                            Console.WriteLine("Ошибка указания адреса копирования");
                            Console.ReadKey();
                        }
                        break;
                    }
                case "move":
                    {
                        try
                        {
                            string fromPath = "";
                            for (int i = 5; i < _usercommand.Length; i++)
                            {
                                fromPath += _usercommand[i];
                            }

                            fromPath.Trim();
                            Console.Write("Введите адрес перемещения: ");

                            string toPath = Console.ReadLine().Trim();

                            FileAttributes fattFromPath = File.GetAttributes(fromPath);
                         
                            if ((fattFromPath & FileAttributes.Directory) == FileAttributes.Directory)
                            {
                                Directory.CreateDirectory(toPath);
                                Directory.Move(fromPath, toPath);
                            }
                            else if ((fattFromPath & FileAttributes.Archive) == FileAttributes.Archive)
                            {
                                File.Move(fromPath, toPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            SaveErrors(ex);
                            Console.WriteLine("Ошибка указания адреса перемещения");
                            Console.ReadKey();
                        }
                        break;
                    }
                case "del":
                    {
                        try
                        {
                            string path = "";
                            for (int i = 4; i < _usercommand.Length; i++)
                            {
                                path += _usercommand[i];
                            }

                            path.Trim();

                            FileAttributes fattPath = File.GetAttributes(path);
                            if ((fattPath & FileAttributes.Directory) == FileAttributes.Directory)
                            {
                                Directory.Delete(path);
                            }
                            else if ((fattPath & FileAttributes.Archive) == FileAttributes.Archive)
                            {
                                File.Delete(path);
                            }
                        }
                        catch (Exception ex)
                        {
                            SaveErrors(ex);
                            Console.WriteLine("Ошибка указания адреса перемещения");
                            Console.ReadKey();
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Введена несуществующая команда");
                        Console.ReadKey();
                        break;
                    }
            }
        }

        //Метод для сохранения пользовательских настроек в файл
        public static void SaveUserOptions(string _filePath)
        {
            string jsonser = JsonSerializer.Serialize(userparam);
            File.WriteAllText(_filePath, jsonser);
        }

        //Метод сохранения отловленных ошибок. В названии - тип ошибки и дата, в файле - текст ошибки
        public static void SaveErrors(Exception e)
        {
            string errorName = $"{pathToErrors}\\{e.GetType().ToString()}-{DateTime.Today.ToShortTimeString()}.txt";
            File.Create(errorName).Close();
            File.WriteAllText(errorName, e.Message.ToString());
        }

        //Копирование каталога (пофайловое копирование с сохранением дерева каталогов)
        public static void CopyDir(string _fromDir, string _toDir)
        {
            {
                Directory.CreateDirectory(_toDir);
                foreach (string s1 in Directory.GetFiles(_fromDir))
                {
                    string s2 = _toDir + "\\" + Path.GetFileName(s1);
                    File.Copy(s1, s2);
                }
                foreach (string s in Directory.GetDirectories(_fromDir))
                {
                    CopyDir(s, _toDir + "\\" + Path.GetFileName(s));
                }
            }
        }

        //Получение размера директории. Директория самостоятельного размера директории не имеет
        static long GetDirSize(string _path)
        {
            long size = 0;

            string[] files = Directory.GetFiles(_path);

            foreach (string file in files)
            {
                size += (new FileInfo(file)).Length;
            }

            string[] dirs = Directory.GetDirectories(_path);

            foreach (string dir in dirs)
            {
                size += GetDirSize(dir);
            }

            return size;
        }

        //Метод получения даты создания каталога (по самому раннему файлу в директории)
        static DateTime GetDirCreationDate(string _path)
        {
            DateTime result = DateTime.Now;

            string[] files = Directory.GetFiles(_path);

            foreach (string file in files)
            {
                if (result > new FileInfo(file).CreationTime)
                {
                    result = new FileInfo(file).CreationTime;
                }
            }

            string[] dirs = Directory.GetDirectories(_path);

            foreach (string dir in dirs)
            {
                result = GetDirCreationDate(dir);
            }

            return result;
        }

        //Метод получения даты последнего изменения каталога (по самому позднему изменению)
        static DateTime GetDirLastChangeDate(string _path)
        {
            DateTime result = GetDirCreationDate(_path);

            string[] files = Directory.GetFiles(_path);

            foreach (string file in files)
            {
                if (result < new FileInfo(file).LastWriteTime)
                {
                    result = new FileInfo(file).LastWriteTime;
                }
            }

            string[] dirs = Directory.GetDirectories(_path);

            foreach (string dir in dirs)
            {
                result = GetDirCreationDate(dir);
            }

            return result;
        }
    }
}
