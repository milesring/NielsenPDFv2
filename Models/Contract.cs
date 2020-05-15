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


        public object Shallowcopy()
        {
            return MemberwiseClone();
        }


        //ignores ID
        public bool WeakCompare(Contract other)
        {
            if(!ContractName.Equals(other.ContractName))
            {
                return false;
            }
            if(!ContractNumber.Equals(other.ContractNumber))
            {
                return false;
            }
            if (!LastUsedDirectory.Equals(other.LastUsedDirectory))
            {
                return false;
            }
            return true;

        }

        //checks all fields
        public bool FullCompare(Contract other)
        {
            throw new NotImplementedException();
        }

        public
        override string ToString()
        {
            if (string.IsNullOrWhiteSpace(ContractName))
            {
                return ContractNumber;
            }
            if (string.IsNullOrWhiteSpace(ContractNumber))
            {
                return ContractName;
            }
            return ContractName + ": " + ContractNumber;
        }
    }
}
