using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace NielsenPDFv2.Models
{
    public class Report : INotifyPropertyChanged
    {
        #region Locals
        private ObservableCollection<string> pdfList = new ObservableCollection<string>();
        private string outputName;
        private string reportName;
        #endregion

        #region Properties
        public ObservableCollection<string> PdfList
        {
            get { return pdfList; }
            set
            {
                pdfList = value;
                OnPropertyChanged(nameof(PdfList));
            }
        }
        public string OutputName
        {
            get { return outputName; }
            set
            {
                outputName = value;
                OnPropertyChanged(nameof(OutputName));
            }
        }
        public string ReportName
        {
            get { return reportName; }
            set
            {
                reportName = value;
                OnPropertyChanged(nameof(ReportName));
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
