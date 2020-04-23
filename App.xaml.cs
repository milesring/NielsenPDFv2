using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NielsenPDFv2.Data_Access;

namespace NielsenPDFv2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static LocalDB database;

        public static LocalDB Database
        {
            get
            {
                if(database == null)
                {
                    database = new LocalDB();
                }
                return database;
            }
        }
    }
}
