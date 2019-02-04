using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IniFormatter
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private string instructionsFileName = "";
        private string directoryTPath = "";
        private string directoryCPath = "";
        private string directoryResultPath = "";
        private void btnInstruction_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".json";
            dlg.Filter = "JSON Files (*.json)|*.json";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                instructionsFileName = dlg.FileName;
                textBoxInstruction.Text = instructionsFileName;
            }

        }
        private string getDirectory()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Open document 
                    return dialog.SelectedPath;
                }
                return "";
            }
        }
        private void btnDirectoryT_Click(object sender, RoutedEventArgs e)
        {
            directoryTPath = getDirectory();
            textBoxT.Text = directoryTPath;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            List<RootObject> rootObjects = JsonConvert.DeserializeObject<List<RootObject>>(readFile(instructionsFileName));
            foreach (var item in rootObjects)
            {
                if (File.Exists(directoryTPath + "\\T_" + item.file) && File.Exists(directoryCPath + "\\C_" + item.file))
                {
                    string tContent = readFile(directoryTPath + "\\T_" + item.file);
                    string cContent = readFile(directoryCPath + "\\C_" + item.file);
                    string resultContent = getResult(item.numberOfDigits, tContent, cContent, item.actions);
                    File.WriteAllText(directoryResultPath + "\\" + item.file, resultContent, Encoding.GetEncoding("big5"));
                }
               
            }
        }
        private List<Block> devideBlocks(List<int> numberOfDigitsInId, string[] splitString)
        {
            List<Block> blocks = new List<Block>();
            foreach (var item in splitString)
            {
                int id = 0;
                if ((from i in numberOfDigitsInId select Int32.TryParse(item.Length >= i ? item.Substring(0, i) : item, out id)).Any(x => x == true))
                {
                    blocks.Add(new Block(id, item));
                }
                else
                {
                    Block b;
                    if (blocks.Count() > 0)
                    {
                        b = blocks.Last();
                        b.block.AppendLine(item);
                    }
                    else
                    {
                        blocks.Add(new Block(0, item));
                    }
                }
            }
            return blocks;
        }
        private string getResult(List<int> numberOfDigitsInId,string tContent, string cContent,List<Action> actions)
        {
            StringBuilder sb = new StringBuilder();
            List<string> c = Regex.Split(cContent, Environment.NewLine).ToList();
            sb.AppendLine(c[0]);
            c.Remove(c[0]);
            string[] tl = Regex.Split(tContent, Environment.NewLine);
            List<Block> cBlocks = devideBlocks(numberOfDigitsInId, c.ToArray());
            List<Block> tBlocks = devideBlocks(numberOfDigitsInId, tl);
            foreach (var cBlock in cBlocks)
            {
                Block t = tBlocks.Find(x => x.id == cBlock.id);

                string[] cBits = cBlock.block.ToString().Split('|');
                if (t != null && cBlock.id!=0)
                {
                    string[] tBits = t.block.ToString().Split('|');
                    foreach (var action in actions)
                    {
                        if(cBits.Length>action.firstSymbolIndexInC && tBits.Length>action.firstSymbolIndexInT)
                        cBits[action.firstSymbolIndexInC] = tBits[action.firstSymbolIndexInT];
                    }
                }
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < cBits.Length-1; i++)
                {
                    builder.Append(cBits[i]);
                        builder.Append("|");
                }
                sb.AppendLine(builder.ToString());
            }
            return sb.ToString();
        }
        private string readFile(string filename)
        {
            string text = File.ReadAllText(filename, Encoding.GetEncoding("big5"));
            return text;
        }
        private void btnCDirectory_Click(object sender, RoutedEventArgs e)
        {
            directoryCPath = getDirectory();
            textBoxC.Text = directoryCPath;
        }

        private void btnResultDirectory_Click(object sender, RoutedEventArgs e)
        {
            directoryResultPath = getDirectory();
            textBoxResult.Text = directoryResultPath;
        }
    }
}
