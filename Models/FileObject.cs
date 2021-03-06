﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NielsenPDFv2.Models
{
    public class FileObject : INotifyPropertyChanged
    {
        #region Locals
        private string fileName;
        private string filePath;
        private int fileNum;
        private int numPages;
        private string password;
        private bool passwordProtected;
        #endregion

        public FileObject() { }

        //copy constructor
        public FileObject(FileObject orig)
        {
            FileName = orig.FileName;
            FilePath = orig.FilePath;
            FileNum = orig.FileNum;
            NumPages = orig.NumPages;
            Password = orig.Password;
            PasswordProtected = orig.PasswordProtected;
        }

        #region Properties
        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        public int FileNum
        {
            get { return fileNum; }
            set
            {
                fileNum = value;
                OnPropertyChanged(nameof(FileNum));
            }
        }

        public int NumPages
        {
            get { return numPages; }
            set
            {
                numPages = value;
                OnPropertyChanged(nameof(NumPages));
            }
        }

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public bool PasswordProtected
        {
            get { return passwordProtected; }
            set
            {
                passwordProtected = value;
                OnPropertyChanged(nameof(PasswordProtected));
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}
