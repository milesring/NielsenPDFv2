using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using NielsenPDFv2.Data_Access;

namespace NielsenPDFv2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string filename = "NielsenPDFSettings";
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

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Restore application-scope property from isolated storage
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, storage))
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Restore each application-scope property individually
                    while (!reader.EndOfStream)
                    {
                        string[] keyValue = reader.ReadLine().Split(new char[] { ',' });
                        this.Properties[keyValue[0]] = keyValue[1];
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                // Handle when file is not found in isolated storage:
                // * When the first application session
                // * When file has been deleted
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            // Persist application-scope property to isolated storage
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForDomain();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, FileMode.Create, storage))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Persist each application-scope property individually
                foreach (string key in this.Properties.Keys)
                {
                    writer.WriteLine("{0},{1}", key, this.Properties[key]);
                }
            }
        }
    }
}
