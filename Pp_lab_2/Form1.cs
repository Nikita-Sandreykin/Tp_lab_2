using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
namespace Pp_lab_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string hello = "hello world";

        public string Hello { get => hello; set => hello = value; }

        private void button1_Click(object sender, EventArgs e)
        {
            var test = new UdpClient();
            try
            {
                while (true)
                {
                    string message = Hello; // сообщение для отправки
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    test.Send(data, data.Length, Program.remoteAddress, Program.remotePort); // отправка
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                test.Close();
            }
        }
    }
}
