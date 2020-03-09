using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Collections.Specialized;
//using MySql.Data.MySqlClient;
using CryptSharp;
using System.Net.Sockets;
using System.Threading;

namespace EgvilagLauncher
{
    //public class MyPerPixelAlphaForm : PerPixelAlphaForm
    //{
    //    public MyPerPixelAlphaForm()
    //    {
    //        TopMost = true;
    //        ShowInTaskbar = false;
    //    }


    //     // Let Windows drag this form for us
    //    protected override void WndProc(ref Message m)
    //    {
    //        if (m.Msg == 0x0084 /*WM_NCHITTEST*/)
    //        {
    //            m.Result = (IntPtr)2;	// HTCLIENT
    //            return;
    //        }
    //        base.WndProc(ref m);
    //    }
    //}



    public partial class FormSplash : Form
    {
        public const string version = "0.2";
        public const string version_date = "2019.12.19.";
        public const string web_location = "http://egvilag.net";
        public string new_version = "";
        public string IP = "";

        public Settings settings = new Settings();

        public FormMain fm;
        TCP tcp;

        public FormSplash()
        {
            //// Label with to display current opacity level
            //Label Label1 = new Label();
            //Label1.AutoSize = true;
            //Label1.Location = new System.Drawing.Point(4, 8);
            //Label1.Text = "1. Drag&&Drop an image file from windows explorer into this window.";
            //Controls.Add(Label1);
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            settings.ReadSettings();
            fm = new FormMain();
            if (false)      //true: splash screen kihagyása
            {
                this.Hide();
                fm.Show();
            }
            else
            {
                Color transp2 = Color.FromArgb(75, Color.White);
                panel4.BackColor = transp2;
                this.CenterToScreen();
                //Color tr = System.Drawing.ColorTranslator.FromHtml("#FCFEFC");
                //Color tr = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                //Color tr = Color.LimeGreen;
                //this.BackColor = tr;
                //this.TransparencyKey = tr;
                this.BackgroundImage = Image.FromFile(@"res/bgsplash.jpg");    //bgsplash.jpg

                //Bitmap newBitmap = Image.FromFile(@"res/Launcher_test01.png") as Bitmap;
                //this.SetBitmap(newBitmap, (byte)255);



                panel1.BackColor = Color.Transparent;
                panel1.BackgroundImage = Image.FromFile(@"res/x.png");
                label4.Text = "Launcher - " + version + " (" + version_date+ ")";
                button1.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
                //to-do: checksum ellenőrzés
                //to-do: launcher frrisítés ellenőrzés a szerverről
            }
            
        }

        // Megtudjuk a külső IP-címet
        private string GetPublicIpAddress()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://ifconfig.me");

            request.UserAgent = "curl"; // this will tell the server to return the information as if the request was made by the linux "curl" command

            string publicIPAddress;

            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    publicIPAddress = reader.ReadToEnd();
                }
            }

            return publicIPAddress.Replace("\n", "");
        }

        //Megnyomom a kilépés gombot
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            panel1.BackgroundImage = Image.FromFile(@"res/x2.png");
        }
        // Elengedem a kilépés gombot
        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            panel1.BackgroundImage = Image.FromFile(@"res/x.png");
            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(1);
            IP = GetPublicIpAddress();
            tcp = new TCP(this, fm);

            backgroundWorker1.ReportProgress(2);
            CheckNewVersion("Updater", "updater_proba.exe");
            CheckNewVersion("Launcher", "");
            CheckNewVersion("Theme", "Release.zip");
            //if (newVer != "0")
            //{
            //    MessageBox.Show("Elérhető a Launcher új verziója!" + "\r\n\r\n" + "v" + newVer.Split(':')[0] + " (" + newVer.Split(':')[1] + ")" + "\r\n\r\n" + "Telepítsük?", "Új frissítés elérhető!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            //    //to-do: Frissítés elvégzése!
            //}
            //else backgroundWorker1.ReportProgress(10);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 1: label6.Text = "Csatlakozás a szerverhez...";
                    textBox1.Enabled = alphaBlendTextBox1.Enabled = false;
                    break;
                case 2: label6.Text = "Frissítések keresése...";
                    break;
                case 3: label6.Text = "A kliens sérült!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Javítás";
                    button2.Visible = true;
                    break;
                case 4: label6.Text = "Van elérhető frissítés (" + new_version + ")!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Kötelező frissítés";
                    button2.Visible = true;
                    button3.Text = "Kilépés";
                    button3.Visible = true;
                    break;
                case 5: label6.Text = "Van elérhető frissítés (" + new_version + ")!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Frissítés";
                    button2.Visible = true;
                    button3.Text = "Elhalasztom";
                    button3.Visible = true;
                    checkBox1.Text = "Jegyezze meg!";
                    checkBox1.Visible = true;
                    break;
                case 6: label6.Text = "Visszaállunk régebbire (" + new_version + ")!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Kötelező rollback";
                    button2.Visible = true;
                    button3.Text = "Kilépés";
                    button3.Visible = true;
                    break;
                case 10: label6.Text = "A kliens naprakész.";
                    //textBox1.Enabled = alphaBlendTextBox1.Enabled = true;
                    break;
                case 13: label6.Text = "Az Updater sérült!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Javítás";
                    button2.Visible = true;
                    break;
                case 14: label6.Text = "Van Updater frissítés (" + new_version + ")!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Kötelező frissítés";
                    button2.Visible = true;
                    button3.Text = "Kilépés";
                    button3.Visible = true;
                    break;
                case 15: label6.Text = "Van Updater frissítés (" + new_version + ")!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Frissítés";
                    button2.Visible = true;
                    button3.Text = "Elhalasztom";
                    button3.Visible = true;
                    checkBox1.Text = "Jegyezze meg!";
                    checkBox1.Visible = true;
                    break;
                case 16: label6.Text = "Visszaállunk régebbi Updater-re (" + new_version + ")!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Kötelező rollback";
                    button2.Visible = true;
                    button3.Text = "Kilépés";
                    button3.Visible = true;
                    break;
                case 20: label6.Text = "Az Updater naprakész.";
                    //textBox1.Enabled = alphaBlendTextBox1.Enabled = true;
                    break;
                case 23: label6.Text = "A téma sérült!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Javítás";
                    button2.Visible = true;
                    break;
                case 24: label6.Text = "Van téma frissítés (" + new_version + ")!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Kötelező frissítés";
                    button2.Visible = true;
                    button3.Text = "Kilépés";
                    button3.Visible = true;
                    break;
                case 25: label6.Text = "Van téma frissítés (" + new_version + ")!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Frissítés";
                    button2.Visible = true;
                    button3.Text = "Elhalasztom";
                    button3.Visible = true;
                    checkBox1.Text = "Jegyezze meg!";
                    checkBox1.Visible = true;
                    break;
                case 26: label6.Text = "Visszaállunk régebbi témára (" + new_version + ")!";
                    label6.ForeColor = Color.Red;
                    button2.Text = "Kötelező rollback";
                    button2.Visible = true;
                    button3.Text = "Kilépés";
                    button3.Visible = true;
                    break;
                case 30: label6.Text = "A téma naprakész.";
                    textBox1.Enabled = alphaBlendTextBox1.Enabled = true;
                    textBox1.Focus();
                    break;
            }
            
        }

        // Kiszámolja a futó exe ellenőrző összegét
        private string GetHash(string path)
        {
            if (path == "") path = System.Reflection.Assembly.GetEntryAssembly().Location;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(System.Reflection.Assembly.GetEntryAssembly().Location))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        // Beolvassa egy weben lévő fájl tartalmát
        private string ReadFileFromWeb(string path)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                byte[] buf = new byte[8192];

                HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(path);
                // execute the request
                HttpWebResponse response = (HttpWebResponse)
                    request.GetResponse();
                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();
                string tempString = null;
                int count;
                do
                {
                    // fill the buffer with data
                    count = resStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                    {
                        // translate from bytes to ASCII text
                        tempString = Encoding.ASCII.GetString(buf, 0, count);
                        sb.Append(tempString);
                    }
                }
                while (count > 0); // any more data to read?
                return sb.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("Hiba a csatlakozáskor!");
            }

            return "";
        }

        //Megnézi, van-e a launcherből újabb, illetve, hogy sérült-e
        private void CheckNewVersion(string packname, string filename)
        {
            int addToProgress = 0;  //BackroundWorker progress választás miatt
            switch (packname)
            {
                case "Updater": addToProgress = 10; break;
                case "Theme": addToProgress = 20; break;
            }


            string currentHash = "";
            currentHash = GetHash(filename);
            currentHash = "proba";    //to-do: próba
            if (currentHash.Length > 0)
            { 
                string resp = "";
                //resp = LaunchPhp("ev_check_launcher.php", new List<string> {"name", "hash", "ver"}, new List<string> {"Launcher", currentHash, version});
                resp = LaunchPhp("ev_check_launcher.php", new List<string> { "name", "hash" }, new List<string> { packname, currentHash });
                if (resp.Length > 0)
                {
                    // lehet "dbver:x.y" is!!
                    if (resp == "0") // Nincs frissítés
                    {
                        backgroundWorker1.ReportProgress(10 + addToProgress);
                        return;
                    }
                    new_version = resp.Split('*')[0];
                    if (resp.Split('*')[2] == "r")  //javítani kell
                    {
                        backgroundWorker1.ReportProgress(3 + addToProgress);
                        //to-do: javítási folyamat
                    }
                    else //van újabb verzió
                        //if (Convert.ToDouble(new_version.Replace('.', ',')) > Convert.ToDouble(version.Replace('.', ',')))   
                    {
                        if (resp.Split('*')[2] == "1")  //kötelező frissíteni
                        {
                            backgroundWorker1.ReportProgress(4 + addToProgress);
                            //to-do: frissítési folyamat
                        }
                        else //nem kötelező frissíteni
                        {
                            backgroundWorker1.ReportProgress(5 + addToProgress);
                            //to-do: ha kéri, akkor frissítési folyamat
                            //to-do: ha nem kéri, ezt rögzíteni kell beállításban, hogy következő indításnál ne ajánlja fel ismét
                        }
                    }
                    //else
                    //        if (Convert.ToDouble(new_version.Replace('.', ',')) < Convert.ToDouble(version.Replace('.', ',')))   //kötelező rollback
                    //        backgroundWorker1.ReportProgress(6);
                    //    else //Legfrissebb verzió van fent
                    //    {
                    //        //to-do: visszaállítási folyamat
                    //        backgroundWorker1.ReportProgress(10);
                    //    }
                }
            }
        }

        //Bejelentkezés
        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text.Length > 0) && (alphaBlendTextBox1.Text.Length > 0))
            {

                string message = "[CONN]" + textBox1.Text + "$" + alphaBlendTextBox1.Text;
                tcp.SendMessage(message);
                // a jelszó még kódolatlanul utazik!
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                string salt = GetSalt(textBox1.Text);
                if (salt == "0")
                {
                    MessageBox.Show("Hiba a bejelentkezés során!", "Hitelesítési hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string hash = Crypter.Phpass.Crypt(alphaBlendTextBox1.Text, "$P$B" + salt);
                string log = Login(textBox1.Text, hash);
                switch (log)
                {
                    case "0" : MessageBox.Show("Rossz felhasználónév vagy jelszó!", "Hitelesítési hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "b" : MessageBox.Show("Ez a felhasználó jelenleg tiltott!", "Hitelesítési hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "d" : MessageBox.Show("Ez a felhasználói fiók törölve van!", "Hitelesítési hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case "1": this.Hide();
                        FormMain fm = new FormMain();
                        fm.Show();
                        break;
                    default : MessageBox.Show("Hiba a bejelentkezés során!", "Hitelesítési hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                //if (log == "0")
                //{
                //    MessageBox.Show("Hiba a bejelentkezés során!", "Hitelesítési hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                //this.Hide();
                //FormMain fm = new FormMain();
                //fm.Show();
            }
        }

        // Lekéri az adatbázisból az userhez tartozó salt-ot
        private string GetSalt(string uid)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(web_location + "/ev_check_user1.php?uid=" + uid);
            WebResponse resp = myRequest.GetResponse();
            using (Stream dataStream = resp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                return responseFromServer;
            }
        }

        // Megpróbálja a bejelentkezést
        private string Login(string uid, string hash)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(web_location + @"/ev_check_user2.php?uid=" + uid + "&pw=" + hash);
            WebResponse resp = myRequest.GetResponse();
            using (Stream dataStream = resp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                return responseFromServer;
            }
            //to-do: kitiltva nincs? illetve kell logolás is!
        }

        // PHP szkript futtatása paraméterekkel
        private string LaunchPhp(string file, List<string> keys, List<string> args)
        {
            if (file == "") return "";
            int count = keys.Count;
            string argstr = "?ip=" + IP;
            if (count > 0)
            {
                argstr += "&";
                for (int i = 0; i < count; i++)
                {
                    argstr += keys[i] + "=" + args[i];
                    if (i < count - 1) argstr += "&";
                }
            }
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(web_location + @"/" +  file + argstr);
            WebResponse resp = myRequest.GetResponse();
            using (Stream dataStream = resp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                return responseFromServer;
            }
        }

        // Végzett az önellenőrzés
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Enabled = true;
        }
        
        // Bezárja a programot
        public void CloseMe()
        {
            this.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                if (alphaBlendTextBox1.Text.Length < 1) alphaBlendTextBox1.Select(); else button1_Click(this, null);
        }

        private void alphaBlendTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                if (alphaBlendTextBox1.Text.Length < 1) alphaBlendTextBox1.Select(); else button1_Click(this, null);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
