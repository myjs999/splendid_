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
        public string FindBracketing(string s, char bra, char ket, bool considerInString=true)
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
                    if (c == '\"' && considerInString)
                    {
                        inString = c;
                    }
                    else if (c == '\'' && considerInString)
                    {
                        inString = c;
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
            public VarType(bool b)
            {
                typ = 1;
                this.a = b ? 1 : 0;
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
            public bool EqualsTo(VarType o)
            {
                if (typ != o.typ) return false;
                if (typ == 0) return true;
                if (typ == 1) return a == o.a;
                if (typ == 2) return s == o.s;
                return false;
            }
            public bool SmallerThan(VarType o)
            {
                //if (typ != o.typ) return false;
                if (typ == 1 && o.typ == 1) return a < o.a;
                if (typ == 2 && o.typ == 2) return s.CompareTo(o.s) == -1;
                return false;
            }
            public bool Logic(VarType o, string sign)
            {
                if (sign == "==") return EqualsTo(o);
                if (sign == "!=") return !EqualsTo(o);
                if (sign == "<") return SmallerThan(o);
                if (sign == ">") return !SmallerThan(o) && !EqualsTo(o);
                if (sign == "<=") return SmallerThan(o) || EqualsTo(o);
                if (sign == ">=") return !SmallerThan(o);
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
            public VarType NegativeOne()
            {
                if (typ == 1) return new VarType(-a);
                if (typ == 2) return new VarType(new String(s.ToCharArray().Reverse().ToArray()));
                return this;
            }
            public void Minus(VarType o)
            {
                if(typ == 1 && o.typ == 1)
                {
                    a -= o.a;
                }
            }
        }
        Dictionary<string, VarType> heapVars = new Dictionary<string, VarType>();
        void ParseThingsAndSigns(string s, List<string> things, List<string> signs)
        {
            string curThing = "", curSign = "";
            for(int i = 0; i < s.Length;)
            {
                if (s[i] == '(')
                {
                    if(curSign != "")
                    {
                        signs.Add(curSign);
                        curSign = "";
                    }
                    string t = FindBracketing(s.Substring(i), '(', ')');
                    i += t.Length;
                    curThing += t;
                }
                if (i >= s.Length) break;
                if (s[i] == '\"')
                {
                    if (curSign != "")
                    {
                        signs.Add(curSign);
                        curSign = "";
                    }
                    if (curThing != "")
                    {
                        things.Add(curThing);
                        curThing = "";
                    }
                    int j = i + 1;
                    while (s[j] != '\"')
                    {
                        //MessageBox.Show(j.ToString());
                        ++j;
                    }
                    string t = s.Substring(i, j - i+1);
                    i += t.Length;
                    curThing += t;
                }
                if (i >= s.Length) break;
                if (CharIsVarName(s[i]))
                {
                    if (curSign != "")
                    {
                        signs.Add(curSign);
                        curSign = "";
                    }
                    string t = GetFirstWord(s.Substring(i));
                    i += t.Length;
                    curThing += t;
                }
                if (i >= s.Length) break;
                if (s[i] == ' ' || s[i] == '\t')
                {
                    i++; continue;
                }
                if (CharIsSign(s[i]))
                {
                    if (curThing != "")
                    {
                        things.Add(curThing);
                        curThing = "";
                    }
                    string t = GetFirstSign(s.Substring(i));
                    i += t.Length;
                    curSign += t;
                }
            }
            //MessageBox.Show("ok");
            if (curThing != "") things.Add(curThing);
            if (curSign != "") signs.Add(curSign);
        }
        bool StringIsLogicSign(string s)
        {
            return s == "==" || s == "<" || s == ">" || s == "<=" || s == ">=" || s == "!=";
        }
        VarType ParseRH(string rh, Dictionary<string, VarType> vars)
        {
            if (rh == "") return new VarType();
            while (rh[0] == '(' && rh.Last() == ')') rh = rh.Substring(1, rh.Length - 2);
            if (rh[0] == '\"' && rh.Last() == '\"')
            {
                bool isString = true;
                for(int i = 1; i < rh.Length-1; i++)
                {
                    if(rh[i] == '\\')
                    {
                        i++;
                        continue;
                    }
                    if (rh[i] == '\"') isString = false;
                }
                if(isString) return new VarType(rh.Substring(1, rh.Length - 2));
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
            //if (rh[0] == '(' && rh.Last() == ')') return ParseRH
            //string firstWord = GetFirstWord(rh);
            //string s2 = PreTrimmed(rh.Substring(firstWord.Length));
            List<string> things = new List<string>();
            List<string> signs = new List<string>();
            ParseThingsAndSigns(rh, things, signs);
            //MessageBox.Show(things[0]);
            if (things.Count == 0) return new VarType();
            if (things.Count == 1 && signs.Count == 0) return ParseRH(things[0], vars);
            VarType ret = ParseRH(things[0], vars);
            for(int i = 0; i < signs.Count; i++)
            {
                if (things.Count <= i + 1) break;
                VarType o = ParseRH(things[i + 1], vars);
                if (signs[i] == "+")
                {
                    ret.Plus(o);
                }
                else if (signs[i] == "-")
                {
                    ret.Minus(o);
                }
                else if (StringIsLogicSign(signs[i]))
                {
                    ret = new VarType(ret.Logic(o, signs[i]));
                }
            }
            return ret;
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
                if (handles[start].Last() == ':')
                {
                    ++start;
                    continue;
                }
                if (handles[start].Last() == '{') ++bed;
                if (handles[start].Last() == '}') --bed;
                if (bed == 0) return start;
                ++start;
                if (start >= handles.Count) return -1;
            }
        }
        string GetHandleByIndex(int ind)
        {
            if (ind < 0 || ind >= handles.Count) return "";
            return handles[ind];
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
                int end = FindEndHandleFromBegin(hi + 1);
                if (execute)
                {
                    //while(end+1 <= handles.Count && handles[end+1] == "else:")
                    //{
                    //    end = FindEndHandleFromBegin(end+2);
                    //}
                    return hi + 1;
                    //PushOutput("yesdo");
                }
                else
                {
                    return end+1;
                    //PushOutput("nodo");
                }
            }else if(firstWord == "sw")
            {
                string param = FindBracketing(s2, '(', ')');
                VarType varparam = ParseRH(param.Substring(1, param.Length-2), vars);
                int end;
                for (int i = hi+1; ;)
                {
                    string cch = GetHandleByIndex(i);
                    if (cch == "end;") {
                        end = i;
                        break;
                    }
                    i = FindEndHandleFromBegin(i) + 1;
                }
                for (int i = hi + 1; ;)
                {
                    string cch = GetHandleByIndex(i);
                    if (cch == "end;")
                    {
                        return end + 1;
                    }
                    if(cch == "else:")
                    {
                        RunAt(i + 1, vars);
                        return end + 1;
                    }
                    if(varparam.EqualsTo(ParseRH(cch.Substring(0, cch.Length - 1),vars) )){
                        RunAt(i + 1, vars);
                        return end + 1;
                    }
                    i = FindEndHandleFromBegin(i) + 1;
                }
                //return end + 1;

            }else if(firstWord == "while")
            {
                int end = FindEndHandleFromBegin(hi + 1);
                string param = FindBracketing(s2, '(', ')');
                param = param.Substring(1, param.Length - 2);
                while(ParseRH(param, vars).AsBool())
                {
                    RunAt(hi + 1, vars);
                }
                return end + 1;
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
            }else if(firstSign == "-=")
            {
                if (vars.ContainsKey(firstWord))
                    vars[firstWord].Minus(ParseRH(s3, vars));
                else
                    vars[firstWord] = ParseRH(s3, vars).NegativeOne();
                
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
                try
                {
                    hi = RunAt(hi, heapVars);
                }
                catch
                {
                    PushOutput("--------\r\n[ERROR]");
                }
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
