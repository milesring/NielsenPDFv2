using System;
using System.Collections.Generic;
using System.Text;

namespace NielsenPDFv2.Tools
{
    public class Utility
    {
        public Utility()
        {

        }
        public string TrimPath(string path)
        {
            string returnPath = "";
            string[] folders = path.Split('\\');

            for (int i = 0; i < folders.Length - 1; i++)
            {
                if (i == folders.Length - 2)
                {
                    returnPath += folders[i];
                }
                else
                {
                    returnPath += folders[i] + "\\";
                }
            }
            return returnPath;
        }

        public string TrimFileName(string path)
        {
            string[] folders = path.Split('\\');
            path = folders[folders.Length-1];
            return path;
        }

    }
}
