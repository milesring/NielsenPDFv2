using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using SQLite;

namespace NielsenPDFv2.Models
{
    public class Contract : INotifyPropertyChanged
    {
        #region Locals
        private string contractName;
        private string contractNum;
        private string lastUsedDirectory = string.Empty;
        //private ObservableCollection<Report> reports = new ObservableCollection<Report>();
        #endregion

        #region Properties
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string ContractName
        {
            get
            {
                return contractName;
            }
            set
            {
                contractName = value;
                OnPropertyChanged(nameof(ContractName));
            }
        }
        public string ContractNumber
        {
            get { return contractNum; }
            set
            {
                contractNum = value;
                OnPropertyChanged(nameof(ContractNumber));
            }
        }
        public string LastUsedDirectory
        {
            get { return lastUsedDirectory; }
            set
            {
                lastUsedDirectory = value;
                OnPropertyChanged(nameof(LastUsedDirectory));
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


        public
            override string ToString() => ContractName + ": " + ContractNumber;
    }
}
