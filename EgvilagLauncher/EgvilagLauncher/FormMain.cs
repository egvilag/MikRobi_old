using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EgvilagLauncher
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            this.BackgroundImage = Image.FromFile(@"res/background.jpg");
            Color transp = Color.FromArgb(75, Color.Red);
            Color transp2 = Color.FromArgb(75, Color.White);
            button2.BackColor = transp;
            panel2.BackColor = transp;
            panel2.BorderStyle = BorderStyle.Fixed3D;
            
            panel1.BackgroundImage = Image.FromFile(@"res/x.png");
            button1.BackgroundImage = Image.FromFile(@"res/but_options.png");
            button1.BackColor = Color.Transparent;
            button1.FlatAppearance.BorderSize = 0;
        }


        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            panel1.BackgroundImage = Image.FromFile(@"res/x2.png");
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            panel1.BackgroundImage = Image.FromFile(@"res/x.png");
            this.Close();
        }

        private void panel1_MouseDown_1(object sender, MouseEventArgs e)
        {
            panel1.BackgroundImage = Image.FromFile(@"res/x2.png");
        }

        private void panel1_MouseUp_1(object sender, MouseEventArgs e)
        {
            panel1.BackgroundImage = Image.FromFile(@"res/x.png");
            FormSplash obj = (FormSplash)Application.OpenForms["FormSplash"];
            obj.Close();
        }
    }
}
