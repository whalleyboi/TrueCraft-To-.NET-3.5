using System;
using System.IO;
using System.Windows.Forms;
using TrueCraft.NET35.TabPages;

namespace TrueCraft.NET35
{
    public partial class Form1 : Form
    {
        TrueCraftServer server;
        TrueCraftAPI API;
        TrueCraftCore Core;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(@"References.txt"))
            {
                File.Create(@"References.txt").Close();
            }

            foreach (string lines in System.IO.File.ReadAllLines(@"References.txt"))
            {
                if (lines.StartsWith("Project:"))
                {
                    textBox1.Text = lines.Replace("Project:", "");
                    if (textBox1.Text != "")
                    {
                        LoadTabs();
                    }
                }
                else if (lines.StartsWith("fNBT:"))
                {
                    textBox2.Text = lines.Replace("fNBT:", "");
                }
                else if (lines.StartsWith("Threading:"))
                {
                    textBox3.Text = lines.Replace("Threading:", "");
                }
                else if (lines.StartsWith("Tuples:"))
                {
                    textBox4.Text = lines.Replace("Tuples:", "");
                }
                else if (lines.StartsWith("Newtonsoft:"))
                {
                    textBox5.Text = lines.Replace("Newtonsoft:", "");
                }
                else if (lines.StartsWith("Yaml:"))
                {
                    textBox6.Text = lines.Replace("Yaml:", "");
                }
            }

            if (textBox2.Text == null || textBox2.Text == "")
            {
                textBox2.Text = Path.GetDirectoryName(Application.ExecutablePath) + "\\References\\fNBT.dll";
            }

            if (textBox3.Text == null || textBox3.Text == "")
            {
                textBox3.Text = Path.GetDirectoryName(Application.ExecutablePath) + "\\References\\System.Threading.dll";
            }

            if (textBox4.Text == null || textBox4.Text == "")
            {
                textBox4.Text = Path.GetDirectoryName(Application.ExecutablePath) + "\\References\\System.Tuples.dll";
            }

            if (textBox5.Text == null || textBox5.Text == "")
            {
                textBox5.Text = Path.GetDirectoryName(Application.ExecutablePath) + "\\References\\Newtonsoft.Json.dll";
            }

            if (textBox6.Text == null || textBox6.Text == "")
            {
                textBox6.Text = Path.GetDirectoryName(Application.ExecutablePath) + "\\References\\YamlDotNet.dll";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                LoadTabs();
            }
        }

        public void LoadTabs()
        {
            foreach (string path in Directory.GetDirectories(textBox1.Text))
            {
                string[] files = System.IO.Directory.GetFiles(path, "*.csproj");
                if (Path.GetFileName(path) == "TrueCraft.Core")
                {
                    tabControl1.TabPages.Add("Core", "TrueCraft Core");
                    Core = new TrueCraftCore();
                    Core.TopLevel = false; // This is most Important
                    Core.FormBorderStyle = FormBorderStyle.None;
                    Core.Width = tabControl1.TabPages["Core"].Width;
                    Core.Height = tabControl1.TabPages["Core"].Height;
                    tabControl1.TabPages["Core"].Visible = true;
                    tabControl1.TabPages["Core"].Select();
                    tabControl1.TabPages["Core"].Controls.Add(Core);
                    Core.csproj = files[0];
                    Core.Show();
                }
                if (Path.GetFileName(path) == "TrueCraft")
                {
                    tabControl1.TabPages.Add("Server", "TrueCraft Server");
                    server = new TrueCraftServer();
                    server.TopLevel = false; // This is most Important
                    server.FormBorderStyle = FormBorderStyle.None;
                    server.Width = tabControl1.TabPages["Server"].Width;
                    server.Height = tabControl1.TabPages["Server"].Height;
                    tabControl1.TabPages["Server"].Visible = true;
                    tabControl1.TabPages["Server"].Select();
                    tabControl1.TabPages["Server"].Controls.Add(server);
                    server.csproj = files[0];
                    server.Show();
                }
                if (Path.GetFileName(path) == "TrueCraft.API")
                {
                    tabControl1.TabPages.Add("API", "TrueCraft API");
                    API = new TrueCraftAPI();
                    API.TopLevel = false; // This is most Important
                    API.FormBorderStyle = FormBorderStyle.None;
                    API.Width = tabControl1.TabPages["API"].Width;
                    API.Height = tabControl1.TabPages["API"].Height;
                    tabControl1.TabPages["API"].Visible = true;
                    tabControl1.TabPages["API"].Select();
                    tabControl1.TabPages["API"].Controls.Add(API);
                    API.csproj = files[0];
                    API.Show();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string[] lines = { "Project:" + textBox1.Text, "fNBT:" + textBox2.Text, "Threading:" + textBox3.Text, "Tuples:" + textBox4.Text, "Newtonsoft:" + textBox5.Text, "Yaml:" + textBox6.Text };
            File.WriteAllLines(@"References.txt", lines);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ReferencePaths.FnbtPath = textBox2.Text;
            ReferencePaths.ThreadingPath = textBox3.Text;
            ReferencePaths.TuplePath = textBox4.Text;
            ReferencePaths.NewtonsoftPath = textBox5.Text;
            ReferencePaths.YamlPath = textBox6.Text;

            DialogResult result = MessageBox.Show("IMPORTANT!" + Environment.NewLine + "This tool can corrupt ~the project files~ if not used correctly." + Environment.NewLine + "Only convert a FRESH copy of TrueCraft 1.7.3!" + Environment.NewLine + "Do NOT open any of the TrueCraft files until I say you can." + Environment.NewLine + "By clicking YES you are agreeing to following these simples rules and not holding me liable.", "Confirmation", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                if (server.Convert())
                {
                    if (API.Convert())
                    {
                        if (Core.Convert())
                        {
                            string filename = textBox1.Text + "\\TrueCraft.sln";
                            string[] lines2 = File.ReadAllLines(filename);
                            lines2[8] = "";
                            lines2[9] = "";
                            lines2[10] = "";
                            lines2[11] = "";
                            lines2[12] = "";
                            lines2[13] = "";
                            lines2[14] = "";
                            lines2[15] = "";
                            lines2[16] = "";
                            lines2[17] = "";
                            File.WriteAllLines(filename, lines2);
                            MessageBox.Show("Everything was successful! If there are ANY errors it should only be a small managable amount." + Environment.NewLine + "If there is a lots more than a file is corrupt and you have to try again..");
                        }
                    }
                }
            }
        }
    }
}
