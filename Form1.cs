using System;
using System.IO;
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
            OpenFileDialog myFileDialog = new OpenFileDialog();
            myFileDialog.InitialDirectory = Environment.CurrentDirectory;
            myFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (myFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((streamRead = myFileDialog.OpenFile()) != null)
                    {
                        using (streamRead)
                        {
                            ConvertToBase64 converter = new ConvertToBase64();
                            converter.Encode(File.ReadAllBytes(myFileDialog.FileName));
                            
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
            ConvertToBase64 converter2 = new ConvertToBase64();
            textBox2.Text = converter2.EncodeString(textBox1.Text);
            textBox3.Text = converter2.Decode(textBox2.Text);
        }
    }

    public class ConvertToBase64
    {
        private string base64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        public ConvertToBase64() { }

        public string Encode(byte[] content)
        {
            int temp = content.Length % 3;
            string r = "", p = "";
            if (temp > 0)
            {
                for (; temp < 3; temp++)
                {
                    p += "=";
                    
                }
            }
            for (int i = 0; i < content.Length; i+=3)
            {
                int n = (content[i] << 16) + (content[i + 1] << 8) + (content[i+2]);
                int n1 = (n >> 18) & 63, n2 = (n >> 12) & 63, n3 = (n >> 6) & 63, n4 = n & 63;
                r += "" + base64[n1] + base64[n2] + base64[n3] + base64[n4];
            }
            return r;
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

        public string Decode(string s)
        {
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
            
            return r.Substring(0, r.Length - p.Length);
        }
    }
    
}
