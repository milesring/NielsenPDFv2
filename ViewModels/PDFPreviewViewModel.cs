using System;
using System.Collections.Generic;
using System.Text;

namespace NielsenPDFv2.ViewModels
{
    public class PDFPreviewViewModel : ViewModel
    {
        #region Locals
        private string mouseFocused = "None";
        #endregion
        public PDFPreviewViewModel()
        {

        }

        #region Properties
        public string MouseFocused
        {
            get { return mouseFocused; }
            set
            {
                mouseFocused = value;
                OnPropertyChanged(nameof(MouseFocused));
            }
        }
        #endregion

    }
}
