using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Token
    {
       public string name { get; set; }
       public int position { get; set; }
       public string type { get; set; }
       public int number { get; set; }
        public Token(string name,int position,string type,int number)
        {
            this.name = name;
            this.position = position;
            this.type = type;
            this.number = number;
        }
    }
}
