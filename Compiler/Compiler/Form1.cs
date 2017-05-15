using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Compiler
{
    public partial class Form1 : Form
    {
        List<Token> Tokens = new List<Token>();
        string[] choose = new string[] { "Switch"};
        string[] condtion = new string[] { "If", "Else if", "Else" };
        string[] Loops = new string[] { "While","For" };
        string[] types = new string[] { "Int", "String" };
        string[] braces = new string[] { "{", "}" };
        string[] parenthless = new string[] { "(", ")" };
        string[] mathOp = new string[] { "+", "-", "*", "/" };
        string[] relyOp = new string[] { "==", "!=", "<=", "<", ">", ">=" };
        Regex nums = new Regex("^[0-9]+$");
        Regex Strings = new Regex("^\"(.*)\"$");
        Regex identifier = new Regex("^[a-z][a-zA-Z0-9]*$");
        List<KeyValuePair<int, List<int>>> errorPositions = new List<KeyValuePair<int, List<int>>>();

        public Form1()
        {
            InitializeComponent();
        }

        private void tokenization(string txt)
        {
            string input = txt;
            string pattern = "\\s+";
            string replacement = " ";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(input, replacement);
            string[] substrings = Regex.Split(result, replacement);
            int counter = 0;
            foreach (var item in substrings)
            {
                Token t = new Token(item, counter, TokenBelongTo(item), TokenNumber(item));
                Tokens.Add(t);
                counter++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // restart app
            Tokens.Clear();
            errorPositions.Clear();
            richTextBox3.Clear();
            dataGridView1.Rows.Clear();
            ///////////////////////////////////////////////

            tokenization(richTextBox2.Text);
            displayInDataGridView(dataGridView1);
            if (Tokens.Count() > 1)
            {
                CheckGrammar(Tokens);
                MakeTree(Tokens);
            }
            else
            {
                richTextBox3.AppendText("Write Something ");
            }

        }

        private string TokenBelongTo(string text)
        {
            foreach (var item in types)
            {
                if (item.Equals(text))
                {
                    return "type";
                }
            }

            foreach (var item in relyOp)
            {
                if (item.Equals(text))
                {
                    return "relyOp";
                }
            }
            foreach (var item in mathOp)
            {
                if (item.Equals(text))
                {
                    return "mathOp";
                }
            }
            foreach (var item in parenthless)
            {
                if (item.Equals(text))
                {
                    return "parenthless";
                }
            }
            foreach (var item in braces)
            {
                if (item.Equals(text))
                {
                    return "braces";
                }
            }
            foreach (var item in condtion)
            {
                if (item.Equals(text))
                {
                    return "condtion";
                }
            }
            foreach (var item in Loops)
            {
                if (item.Equals(text))
                {
                    return "Loop";
                }
            }
            foreach (var item in choose)
            {
                if (item.Equals(text))
                {
                    return "choose";
                }
            }

            if (nums.IsMatch(text))
            {
                return "numbers";
            }

            if (Strings.IsMatch(text))
            {
                return "strings";
            }
            if (identifier.IsMatch(text))
            {
                return "identifiers";
            }
            if (text == "=")
            {
                return "Equal";
            }
            if (text == ";")
            {
                return "SemiCol";
            }

            return "not Found";
        }
        private int TokenNumber(string text)
        {
            foreach (var item in types)
            {
                if (item.Equals(text))
                {
                    return 7;
                }
            }

            foreach (var item in relyOp)
            {
                if (item.Equals(text))
                {
                    return 3;
                }
            }
            foreach (var item in mathOp)
            {
                if (item.Equals(text))
                {
                    return 4;
                }
            }

            foreach (var item in condtion)
            {
                if (item.Equals(text))
                {
                    return 8;
                }
            }

            if (nums.IsMatch(text))
            {
                return 0;
            }

            if (Strings.IsMatch(text))
            {
                return 1;
            }
            if (identifier.IsMatch(text))
            {
                return 2;
            }
            if (text.Equals(";"))
            {
                return 10;
            }
            if (text.Equals("="))
            {
                return 9;
            }
            if (text.Equals("("))
            {
                return 5;
            }
            if (text.Equals(")"))
            {
                return 11;
            }
            if (text.Equals("{"))
            {
                return 6;
            }
            if (text.Equals("}"))
            {
                return 12;
            }
            return 20;
        }
        private void displayInDataGridView(DataGridView dgv)
        {
            dgv.ColumnCount = 4;
            dgv.Rows.Add("Token", "Position", "Token Type", "Token Num");
            foreach (var item in Tokens)
            {
                dgv.Rows.Add(item.name, item.position, item.type, item.number);
            }

        }
        private void CheckGrammar(List<Token> tokens)
        {
            for (int i = 1; i < tokens.Count(); i++)
            {
                lookAhead(tokens[i - 1], tokens[i]);
            }
            if (errorPositions.Count() == 0)
            {
                richTextBox3.Text += "No Error";
            }
            else
            {
                foreach (var item in errorPositions)
                {
                    richTextBox3.AppendText("\n Error Position = " + item.Key);
                    foreach (var item0 in item.Value)
                    {
                        richTextBox3.AppendText("\n Must be " + turnNumIntoName(item0));
                    }
                    richTextBox3.AppendText("\n");
                }
            }
        }
        private void lookAhead(Token x, Token y)
        {

            if (x.number == 8 && (y.number != 5 || y.number == 20)) errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 5 }));
            if (x.number == 5 && (y.number != 2 || y.number == 20)) errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 2 }));
            if (x.number == 2 && ((y.number != 3 && y.number != 9) || y.number == 20))
            {
                if (lookBefore(x) == 7)
                {
                    errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 9 }));
                }
                if (lookBefore(x) == 5)
                {
                    errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 3 }));
                }
            }
            if (x.number == 2 && ((y.number == 3 || y.number == 9)))
            {
                if (lookBefore(x) == 20 && y.number == 3) // start with number so there is no token before it
                {
                    errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 5 }));
                }
                if (lookBefore(x) == 20 && y.number == 9) // start with number so there is no token before it
                {
                    errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 7 }));
                }
                if (lookBefore(x) == 7 && y.number == 3)
                {
                    errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 9 }));
                }
                if (lookBefore(x) == 5 && y.number == 9)
                {
                    errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 3 }));
                }

            }
            if (x.number == 3 && (y.number != 0 || y.number == 20)) errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 0 }));
            if (x.number == 0 && ((y.number != 11 && y.number != 10) || y.number == 20))
            {
                if (lookBefore(x) == 30) // start with number so there is no token before it
                {
                    errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 11, 10 }));
                }
                if (lookBefore(x) == 3)
                {
                    errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 11 }));
                }
                if (lookBefore(x) == 9)
                {
                    errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 10 }));
                }
            }
            if (x.number == 7 && (y.number != 2 || y.number == 20)) errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 2 }));
            if (x.number == 9 && ((y.number != 0 && y.number != 1 && y.number != 2) || y.number == 20)) errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 0, 1 }));
            if (x.number == 6 && ((y.number != 7 && y.number != 8 && y.number != 12) || y.number == 20)) errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 7, 8, 12 }));
            if (x.number == 10 && ((y.number != 7 && y.number != 8 && y.number != 12) || y.number == 20)) errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 7, 8, 12 }));
            if (x.number == 11 && ((y.number != 3 && y.number != 6 && y.number != 4) || y.number == 20)) errorPositions.Add(new KeyValuePair<int, List<int>>(y.position, new List<int>() { 3, 6, 4 }));


        }
        private int lookBefore(Token x)
        {
            for (int i = 1; i < Tokens.Count(); i++)
            {
                if (Tokens[i] == x)
                {
                    return Tokens[i - 1].number;
                }
            }
            return 30;
        }
        private string turnNumIntoName(int num)
        {
            if (num == 0) return "number";
            if (num == 1) return "string";
            if (num == 2) return "Identifier";
            if (num == 3) return "RelyOp";
            if (num == 4) return "MathOp";
            if (num == 5) return "(";
            if (num == 11) return ")";
            if (num == 6) return "{";
            if (num == 12) return "}";
            if (num == 7) return "Type";
            if (num == 8) return "ReservedWord";
            if (num == 9) return "Equal";
            if (num == 10) return ";";

            return "Not Found";
        }
        private void MakeTree(List<Token> tokens)
        {
            treeView1.Nodes.Clear();
            TreeNode treeNode;

            foreach (var item in tokens)
            {
                    if (item.number == 8)
                    {
                        treeNode = new TreeNode("Condition");
                        foreach (var item0 in tokens.Skip(item.position))
                        {
                            treeNode.Nodes.Add(item0.type);
                            if (item0.number == 12) break;
                        }
                        treeView1.Nodes.Add(treeNode);
                    }
                    if (item.number == 7)
                    {
                        treeNode = new TreeNode("Declaration");
                        foreach (var item0 in tokens.Skip(item.position))
                        {
                            treeNode.Nodes.Add(item0.type);
                            if (item0.number == 10) break;
                        }
                        treeView1.Nodes.Add(treeNode);
                    }
            }
        }
   
    }
}
