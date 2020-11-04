using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pp_lab_2
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        internal static string remoteAddress = "127.0.0.1"; // хост для отправки данных
        internal static int remotePort = 8002; // порт для отправки данных
        internal static int localPort = 8001; // локальный порт для прослушивания входящих подключений
    }
}
