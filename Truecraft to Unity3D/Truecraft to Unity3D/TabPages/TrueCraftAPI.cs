using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TrueCraft.NET35.TabPages
{
    public partial class TrueCraftAPI : Form
    {
        public string csproj = "";
        public string fnbtPath;
        public string tuplePath = "";
        public string threadingPath = "";

        Reference reference;

        public TrueCraftAPI()
        {
            InitializeComponent();
        }

        public bool Convert()
        {
            try
            {
                richTextBox1.Text = "API loaded! adding references." + Environment.NewLine;
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

                reference.RemoveOriginal("YamlDotNet");
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

                    if (needsTuple)
                    {
                        File.WriteAllText(path, "using System.Tuples;" + Environment.NewLine + builder.ToString());
                        builder.Clear(); // clear builder to save memeory
                    }

                    bool modifiedAgain = false;
                    /// Pass #2 for fixes.
                    foreach (var line in File.ReadAllLines(path))
                    {
                        builder.AppendLine(line);
                        if (line.Contains(".Item1"))
                        {
                            builder.Replace(".Item1", ".Element1");
                            modifiedAgain = true;
                        }
                        else if (line.Contains(".Item2"))
                        {
                            builder.Replace(".Item2", ".Element2");
                            modifiedAgain = true;
                        }
                        else if (line.Contains(".Item3"))
                        {
                            builder.Replace(".Item3", ".Element3");
                            modifiedAgain = true;
                        }

                        if (line.Contains("Tuple.Create"))
                        {
                            builder.Replace("Tuple.Create", "Tuple.New");
                            modifiedAgain = true;
                        }
                    }

                    if (modifiedAgain)
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