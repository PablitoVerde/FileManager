using System;
using System.Collections.Generic;
using System.Text;


//Класс для создания и хранения пользовательских данных

namespace ConsoleFileManager
{
    class UserOptions
    {
        private string userName, lastPathToDirectory;
        private int filesAndDirScale, currentPage;

        public UserOptions(string _userName, string _lastPathToDirectory, int _filesAndDirScale, int _currentPage)
        {
            userName = _userName;
            lastPathToDirectory = _lastPathToDirectory;
            filesAndDirScale = _filesAndDirScale;
            currentPage = _currentPage;
        }

        public UserOptions()
        {
            userName = Environment.MachineName;
            lastPathToDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            filesAndDirScale = 30;
            currentPage = 1;
        }

        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }

        public string LastPathToDirectory
        {
            get
            {
                return lastPathToDirectory;
            }
            set
            {
                lastPathToDirectory = value;
            }
        }

        public int FilesAndDirScale
        {
            get
            {
                return filesAndDirScale;
            }
            set
            {
                filesAndDirScale = value;
            }
        }

        public int CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                currentPage = value;
            }
        }
    }
}