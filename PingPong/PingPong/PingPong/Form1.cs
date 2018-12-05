using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingPong
{
    public partial class frmPingPong : Form
    {
        public frmPingPong()
        {
            InitializeComponent();
        }

        private void frmPingPong_Load(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            txtApiLink.Enabled = false;
            btnStop.Enabled = true;
            btnStart.Enabled = false;
            InitTimer();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            txtApiLink.Enabled = true;
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            //timerTick = new Timer();
            timerTick.Stop();
            lblStatus.Text = "Ping => Stop";
        }
        public void InitTimer()
        {
            timerTick = new Timer();
            timerTick.Tick += new EventHandler(timer1_Tick);
            timerTick.Interval = 1000;
            timerTick.Start();
        }
        public string CallAPI(string baseAddress)
        {

            try
            {
                string rs = "";
                HttpClient _client = new HttpClient();
                _client.BaseAddress = new Uri(baseAddress);
                HttpResponseMessage result = _client.GetAsync("mobile/ping").Result;
                if (result.IsSuccessStatusCode)
                {
                    rs = result.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    rs = "404 not found !";
                }
                return rs;
            }
            catch (Exception e)
            {
                return e.ToString();
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblStatus.Text = "Ping => " + CallAPI(txtApiLink.Text);
        }

        
    }
}
