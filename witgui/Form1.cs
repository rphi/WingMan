using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rug.Osc;

namespace witgui
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            sendvalue("/eos/chan/1/value", trackBar1.Value);
        }

        private void sendvalue(string s, int i)
        {
            using (OscSender sender = new OscSender(IPAddress.Parse("192.168.1.75"), 3032))
            {
                sender.Connect();
                sender.Send(new OscMessage(s, i));
            }
        }

        private void sendcommand(string s)
        {
            using (OscSender sender = new OscSender(IPAddress.Parse("192.168.1.75"), 3032))
            {
                sender.Connect();
                sender.Send(new OscMessage(s));
                sender.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendcommand("/eos/chan/2/full");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sendcommand("/eos/chan/2/out");
        }
    }
}
