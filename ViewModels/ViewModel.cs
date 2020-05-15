using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NielsenPDFv2.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
