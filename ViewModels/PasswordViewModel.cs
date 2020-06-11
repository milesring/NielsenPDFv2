using System;
using System.Collections.Generic;
using System.Text;

namespace NielsenPDFv2.ViewModels
{
    class PasswordViewModel:ViewModel
    {
        #region Locals
        private string password;
        #endregion

        #region Properties
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        #endregion
    }
}
