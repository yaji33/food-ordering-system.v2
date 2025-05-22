using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using food_ordering_system.v2.UI.Common;
using food_ordering_system.v2.Data.Seeders;

namespace food_ordering_system.v2
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
            AdminSeeder.SeedAdmin();
        }
        
    }
}
