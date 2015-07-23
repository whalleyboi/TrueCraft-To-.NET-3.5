using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TrueCraft.NET35.TabPages
{
    public partial class TrueCraftServer : Form
    {
        public string csproj = "";
        Reference reference;

        public TrueCraftServer()
        {
            InitializeComponent();
        }

        public bool Convert()
        {
            try
            {
                richTextBox1.Text = "Server loaded! adding references." + Environment.NewLine;
                reference = new Reference(csproj);
                reference.RemoveOriginal("fNbt");
                reference.RemoveReference("fNbt");
                reference.CreateReference("fNbt", ReferencePaths.FnbtPath);

                reference.RemoveOriginal("System.Threading");
                reference.RemoveReference("System.Threading");
                reference.CreateReference("System.Threading", ReferencePaths.ThreadingPath);

                reference.RemoveOriginal("System.Tuples");
                reference.RemoveReference("System.Tuples");
                reference.CreateReference("System.Tuples", ReferencePaths.TuplePath);

                reference.RemoveReference("YamlDotNet");
                reference.CreateReference("YamlDotNet", ReferencePaths.YamlPath);
                reference.ChangeFramework();

                richTextBox1.Text += reference.Status;
                var list = new List<string>();
                DirSearch(list, Path.GetDirectoryName(csproj));

                foreach (string path in list)
                {
                    if (path.Contains(".csproj"))
                    {
                        continue;
                    }

                    StringBuilder builder = new StringBuilder();
                    bool needsTuple = false;
                    foreach (var line in File.ReadAllLines(path))
                    {
                        builder.AppendLine(line);

                        if (line.Contains("using System.Tuples;"))
                        {
                            break;
                        }
                        else if (line.Contains("Tuple"))
                        {
                            needsTuple = true;
                        }
                    }

                    builder.Replace(".Item1", ".Element1");
                    builder.Replace(".Item2", ".Element2");
                    builder.Replace(".Item3", ".Element3");
                    builder.Replace("Tuple.Create", "Tuple.New");
                    builder.Replace("Directory.GetCurrentDirectory(), \"players\"", "(Directory.GetCurrentDirectory() + \"/players\")");
                    builder.Replace("new NbtList(\"inventory\", Inventory.GetSlots().Select(s => s.ToNbt())),", "new NbtList(\"inventory\", Inventory.GetSlots().Select(s => (NbtTag)s.ToNbt())),");
                    if (needsTuple)
                    {
                        File.WriteAllText(path, "using System.Tuples;" + Environment.NewLine + builder.ToString());
                        builder.Clear(); // clear builder to save memeory
                    }
                    else
                    {
                        File.WriteAllText(path, builder.ToString());
                        builder.Clear(); // clear builder to save memeory
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                richTextBox1.Text += "Converting API failed!" + Environment.NewLine + e.Message;
                return false;
            }
        }
        public static void DirSearch(List<string> files, string startDirectory)
        {
            try
            {
                foreach (string file in Directory.GetFiles(startDirectory, "*.cs*"))
                {
                    string extension = Path.GetExtension(file);

                    if (extension != null)
                    {
                        files.Add(file);
                    }
                }

                foreach (string directory in Directory.GetDirectories(startDirectory))
                {
                    DirSearch(files, directory);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
