using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splendid__
{
    internal class SplendidCompiler
    {
        public class Command
        {

        }
        public string FindBracketing(string s, char bra, char ket)
        {
            string ret = "";
            char inString = ' ';
            int bed = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '\\')
                {
                    i++;
                    continue;
                }
                if (inString != ' ')
                {
                    if (c == '\"' && inString == '\"')
                    {
                        inString = ' ';
                    }
                    else if (c == '\'' && inString == '\'')
                    {
                        inString = ' ';
                    }
                }
                else
                {
                    if (c == bra) ++bed;
                    if (bed > 0)
                    {
                        ret += c;
                    }
                    if (c == ket) --bed;
                    if (bed == 0 && ret != "") return ret;
                }
            }
            return ret;
        }
        public class StringUtilities
        {
            
        }
        public string code = "";
        public string output = "";
        const string ENDL = "\r\n";
        public void ImportCode(string code)
        {
            this.code = code;
        }
        List<string> handles = new List<string>();

        bool CharIsVarName(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '_';
        }
        bool CharIsSign(char c)
        {
            return false
                || c == '.' || c == ',' || c == '=' || c == '-' || c == '+'
                || c == '*' || c == '/' || c == '>' || c == '<' || c == '!'
                || c == '?' || c == '|' || c == '&' || c == '^'
                || c == '(' || c == ')' || c == '[' || c == ']'
                ;
        }
        string GetFirstWord(string s)
        {
            string ret = "";
            for(int i = 0; i < s.Length; i++)
            {
                if (CharIsVarName(s[i])) ret += s[i];
                else break;
            }
            return ret;
        }
        string GetFirstSign(string s)
        {
            string ret = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (CharIsSign(s[i])) ret += s[i];
                else break;
            }
            return ret;
        }
        string PreTrimmed(string s)
        {
            string ret = "";
            for(int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    if (ret == "") continue;
                    //break;
                }
                ret += s[i];
            }
            return ret;
        }
        public void Compile()
        {
            string curHandle = "";
            int inString = 0, inMac = 0;
            output = "";
            code += ";";
            for(int i = 0; i < code.Length-1; i++)
            {
                char c = code[i];
                if(c == '\\')
                {
                    curHandle += "\\" + code[i + 1];
                    i++;
                }
                else
                {
                    if (c == '\"')
                    {
                        inString ^= 1;
                        curHandle += "\"";
                    }
                    else if (c == '\'')
                    {
                        inString ^= 1;
                        curHandle += "\'";
                    }else if(c == '/' && code[i+1] == '*')
                    {
                        inMac++;
                    }else if(c == '*' && code[i+1] == '/')
                    {
                        inMac--;
                    }
                    else
                    {
                        if(inString == 1 || (c != '\n' && c != '\r' && c != '\t'))
                        {
                            if(curHandle.Length != 0 || c != ' ')
                                curHandle += c;

                        }
                    }
                }
                if(inString == 0 && inMac == 0)
                {
                    if(c == ';' || c == '{' || c == '}' || c == ':')
                    {
                        while (curHandle.Length >= 2 && curHandle[curHandle.Length-2] == ' ')
                        {
                            curHandle = curHandle.Substring(0, curHandle.Length - 2) + c;
                        }
                        handles.Add(curHandle);
                        curHandle = "";
                    }
                }
            }


        }

        class VarType
        {
            int typ = 0;
            int a = 0;
            string s = "";
            public VarType(int a)
            {
                typ = 1;
                this.a = a;
            }
            public VarType(string s)
            {
                typ = 2;
                this.s = s;
            }
            public VarType()
            {

            }
            public string AsString()
            {
                if (typ == 2) return s;
                else if (typ == 1) return a.ToString();
                else if (typ == 0) return "undefined";
                else return "";
            }
            public bool AsBool()
            {
                if (typ == 1) return a != 0;
                if (typ == 2) return s != "";
                return false;
            }
            public void Plus(VarType o)
            {
                if(typ == 1)
                {
                    if (o.typ == 1)
                    {
                        a += o.a;
                    }
                    else if (o.typ == 2)
                    {
                        s = a.ToString();
                        typ = 2;
                        s += o.s;
                    }
                }else if(typ == 2)
                {
                    if (o.typ == 1)
                    {
                        s += o.a.ToString();
                    }
                    else if (o.typ == 2)
                    {
                        s += o.s;
                    }
                }
            }
        }
        Dictionary<string, VarType> heapVars = new Dictionary<string, VarType>();
        VarType ParseRH(string rh, Dictionary<string, VarType> vars)
        {
            if (rh[0] == '\"' && rh.Last() == '\"')
            {
                return new VarType(rh.Substring(1, rh.Length - 2));
            }
            try
            {
                int a = Convert.ToInt32(rh);
                return new VarType(a);
            }
            catch
            {

            }
            if(vars.ContainsKey(rh))
            {
                return vars[rh];
            }

            return new VarType();
        }
        void PushOutput(string s)
        {
            output += s;
        }
        int FindEndHandleFromBegin(int start)
        {
            int bed = 0;
            for (; ; )
            {
                if (handles[start].Last() == '{') ++bed;
                if (handles[start].Last() == '}') --bed;
                if (bed == 0) return start;
                ++start;
                if (start >= handles.Count) return -1;
            }
        }
        class Stringwm // string with mark
        {
            public string s = "";
            public int mark = 0;
            public Stringwm(string s)
            {
                this.s = s;
            }
        }
        int RunAt(int hi, Dictionary<string, VarType> vars)
        {
            string ch = handles[hi];
            if(ch == "{")
            {
                int end = FindEndHandleFromBegin(hi);
                ++hi;
                for(; ; )
                {
                    if (hi == end) break;
                    hi = RunAt(hi, vars);
                }
                return end + 1;
            }
            ch = ch.Remove(ch.Length - 1);
            if(ch == "helloworld")
            {
                PushOutput("hello, world!");
                return hi+1;
            }
            string firstWord = GetFirstWord(ch);
            string s2 = PreTrimmed(ch.Substring(firstWord.Length));
            if (firstWord == "print")
            {
                PushOutput(ParseRH(s2, vars).AsString());
                return hi+1;
            }else if(firstWord == "if")
            {
                string param = FindBracketing(s2, '(', ')');
                bool execute = ParseRH(param.Substring(1, param.Length - 2), vars).AsBool();
                if (execute)
                {
                    return hi + 1;
                    //PushOutput("yesdo");
                }
                else
                {
                    return FindEndHandleFromBegin(hi + 1)+1;
                    //PushOutput("nodo");
                }
            }
            string firstSign = GetFirstSign(s2);
            string s3 = PreTrimmed(s2.Substring(firstSign.Length));
            if (firstSign == "=")
            {
                vars[firstWord] = ParseRH(s3, vars);
            }else if(firstSign == "+=")
            {
                if (vars.ContainsKey(firstWord))
                    vars[firstWord].Plus(ParseRH(s3, vars));
                else
                    vars[firstWord] = ParseRH(s3, vars);
            }
            return hi + 1;
        }
        public void Run()
        {
            output = "";
            output += handles.Count.ToString() + ENDL;
            for (int i = 0; i < handles.Count; i++)
            {
                output += handles[i] + ENDL;
            }
            output += "--------\r\n";
            int hi = 0;
            for(; ;)
            {
                hi = RunAt(hi, heapVars);
                if (hi >= handles.Count || hi < 0) break;
            }
            //for (int i = 0; i < handles.Count; i++)
            //{
            //    RunAt(i);
            //    break;
            //}
            //if(handles.Count > 0)
            //    RunAt(0);
            
                //    if (handles[i] == "hello splendid;")
                //    {
                //        output += "Hello, Splendid!\r\n";
                //    }
                //}
                //int hi = 0;
                //for(; ;)
                //{
                //    string ch = handles[hi];
                //    string firstWord = GetFirstWord(ch);
                //    string firstSign = GetFirstSign(ch.Substring(firstWord.Length));
                //    if(firstSign == "=")
                //    {
                //        vars[firstWord] = ParseRH(ch.Substring(firstWord.Length + firstSign.Length));
                //    }
                //}
            }
        }
}
