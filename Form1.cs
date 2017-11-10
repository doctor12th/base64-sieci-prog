using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Convert_ot_Base64
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stream streamRead = null;
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = Environment.CurrentDirectory;
            openFile.Filter = "All files (*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((streamRead = openFile.OpenFile()) != null)
                    {
                        using (streamRead)
                        {
                            ConvertToBase64 converter = new ConvertToBase64();
                            textBox4.Text=converter.Encode(File.ReadAllBytes(openFile.FileName));
                            streamRead.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
                

            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ConvertToBase64 converter = new ConvertToBase64();
            textBox2.Text = converter.EncodeString(textBox1.Text);
            textBox3.Text = converter.Decode(textBox2.Text).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            ConvertToBase64 converter = new ConvertToBase64();
            saveFile.Filter = "base64 files (*.b64)|*.b64";
            saveFile.Title = "Save encoded file";
            saveFile.DefaultExt = ".b64";
            saveFile.ShowDialog();
            if (saveFile.FileName != "")
            {
                using (FileStream fstream = new FileStream(saveFile.FileName, FileMode.OpenOrCreate))      //@"C:\SomeDir\noname\note.txt"
                {
                   
                    fstream.Write(Encoding.ASCII.GetBytes(textBox4.Text), 0, Encoding.ASCII.GetBytes(textBox4.Text).Length);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)  //save source file
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.Filter = "All files (*.*)|*.*";
            saveFile.RestoreDirectory = true;
            saveFile.ShowDialog();
            if (saveFile.FileName != "")
            {
                using (FileStream fstream = new FileStream(saveFile.FileName, FileMode.OpenOrCreate))      //@"C:\SomeDir\noname\note.txt"
                {
                    ConvertToBase64 converter = new ConvertToBase64();
                    fstream.Write(converter.Decode(textBox4.Text), 0, converter.Decode(textBox4.Text).Length);
                }
                
            }
        }

        private void button5_Click(object sender, EventArgs e) //open b64 file
        {
            Stream streamRead = null;
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = Environment.CurrentDirectory;
            openFile.Filter = "Base64 files (*.b64)|*.b64";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((streamRead = openFile.OpenFile()) != null)
                    {
                        using (streamRead)
                        {
                            byte[] arr;
                            ConvertToBase64 converter = new ConvertToBase64();
                            arr = File.ReadAllBytes(openFile.FileName);
                            Encoding enc8 = Encoding.UTF8;
                            textBox4.Text = enc8.GetString(arr);
                            streamRead.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                };
            }
        }
    }

    public class ConvertToBase64
    {
        private string base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        public ConvertToBase64() { }

        public string Encode(byte[] content)
        {
            Encoding enc8 = Encoding.UTF8;
            string arr = enc8.GetString(content);
            int c = arr.Length % 3;
            string r = "", p = "";
            if (c > 0)
            {
                for (; c < 3; c++)
                {
                    p += "=";
                    arr += "\0";
                }
            }
            for ( c = 0; c < arr.Length; c += 3)
            {
                int n = (arr[c] << 16) + (arr[c + 1] << 8) + (arr[c+2]);
                int n1 = (n >> 18) & 63, n2 = (n >> 12) & 63, n3 = (n >> 6) & 63, n4 = n & 63;
                r += "" + base64[n1] + base64[n2] + base64[n3] + base64[n4];
            }
            return r.Substring(0, r.Length - p.Length) + p;
        }

        public string EncodeString(string s)
        {
            string r = "", p = "";
            int c = s.Length % 3;
            if (c>0)
            {
                for (; c < 3; c++)
                {

                    p += "=";
                    s += "\0";
                }
            }
            for (c = 0; c < s.Length; c+=3)
            {
                
                int n = (s[c] << 16) + (s[c + 1]<<8) + (s[c + 2]);
                int n1 = (n >> 18) & 63, n2 = (n >> 12) & 63, n3 = (n >> 6) & 63, n4 = n & 63;
                r += "" + base64[n1] + base64[n2] + base64[n3] + base64[n4];
            }
            return r.Substring(0,r.Length-p.Length)+p;
        }

        public byte[] Decode(string s)
        {
            byte[] array;
            s = s.Replace("[^" + base64 + "=]", "");
            
            String p = (s[s.Length - 1] == '=' ?
                (s[s.Length - 2] == '=' ? "AA" : "A") : "");
            String r = "";
            s = s.Substring(0, s.Length - p.Length) + p;
            
            for (int c = 0; c < s.Length; c += 4)
            {
                int n = (base64.IndexOf(s[c]) << 18)
                    + (base64.IndexOf(s[c+1]) << 12)
                    + (base64.IndexOf(s[c+2]) << 6)
                    + base64.IndexOf(s[c+3]);
                
                r += "" + (char)((n >> 16) & 0xFF) + (char)((n >> 8) & 0xFF)
                    + (char)(n & 0xFF);
            }
            
            return array = Encoding.ASCII.GetBytes(r);
        }
    }
    
}
