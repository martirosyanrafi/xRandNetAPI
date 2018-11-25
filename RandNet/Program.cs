﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Windows.Forms;
using Web.Routing;

namespace RandNet
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}