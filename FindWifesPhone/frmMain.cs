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

namespace FindWifesPhone
{
    using System.Net;

    using FindWifesPhone.Library;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public partial class frmMain : Form
    {
        



        public frmMain()
        {
            InitializeComponent();           
        }


        private async void button1_Click(object sender, EventArgs e)
        {

          FindPhoneService findPhone = new FindPhoneService("1");
          await  findPhone.FindPhone(UserName.Text, Password.Text, DeviceName.Text);
        }    
    }
 
}
