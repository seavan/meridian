using System;
using System.IO;

namespace meridian.diagram
{
    public class CSharpWriter : StreamWriter
    {
        public CSharpWriter(Stream _stream)
            : base(_stream, System.Text.Encoding.UTF8)
        {

            Indent = 0;
            IndentSymbol = "\t";
            Eol = ";";
            BracketL = "{";
            BracketR = "}";
        }

        public void SetMysqlMode()
        {
            /*FieldSeparatorL = "`";
            FieldSeparatorR = "`";
            AdoNetPrefix = "MySql";
            AdoNetNamespace = "using MySql.Data.MySqlClient;";
            */
        }


        public void WriteIndent(string _format, params object[] _args)
        {
            WriteIndent();
            Write(_format, _args);
        }

        public void WriteIndentEol(string _format, params object[] _args)
        {
            WriteIndent();
            Write(_format, _args);
            WriteEol();
        }

        public void WriteIndentBlank(string _format, params object[] _args)
        {
            WriteIndent();
            Write(_format, _args);
            WriteBlank();
        }

        public void WriteUsing(string _ns)
        {
            WriteIndent();
            Write("using {0}", _ns);
            WriteEol();
        }

        public void WriteNamespace(string _ns)
        {
            WriteIndent();
            Write("namespace {0}", _ns);
            WriteBlank();
        }

        public void WriteFunction(string _modifier, string _returnType, string _name, string[] _args = null, string[] _argNames = null)
        {
            WriteIndent();
            if (!String.IsNullOrEmpty(_modifier))
            {
                Write(_modifier);
                WriteSpace();
            }

            if (!String.IsNullOrEmpty(_returnType))
            {
                Write(_returnType);
                WriteSpace();
            }

            if (!String.IsNullOrEmpty(_name))
            {
                Write(_name);
            }

            Write("(");

            if (_args != null)
            {
                for (int i = 0; i < _args.Length; ++i)
                {
                    if (i > 0)
                    {
                        Write(", ");
                    }
                    if ((_argNames != null) && (_argNames.Length == _args.Length))
                    {
                        Write("{0} {1}", _args[i], _argNames[i]);
                    }
                    else
                    {
                        Write("{0}", _args[i]);
                    }
                }
            }

            Write(")");
            WriteBlank();
        }

        public void WriteClass(string _ns, string _scope = "public", bool _partial = true)
        {
            WriteIndent();

            if (!String.IsNullOrEmpty(_scope))
            {
                Write(_scope + " ");
            }
            if (_partial)
            {
                Write("partial ");
                
            }

            Write("class {0}", _ns);
            WriteBlank();
        }

        public void WriteInterface(string _ns)
        {
            WriteIndent();
            Write("interface {0}", _ns);
            WriteEol();
        }

        public void WriteIndent()
        {
            for (int i = 0; i < Indent; ++i)
            {
                Write(IndentSymbol);
            }
        }


        public void WriteEol()
        {
            WriteLine(Eol);
        }

        public void WriteBlank()
        {
            WriteLine();
        }

        public void WriteSpace()
        {
            Write(" ");
        }

        public void OpenBlock()
        {
            WriteIndent();
            WriteLine(BracketL);
            ++Indent;
        }

        public void CloseBlock()
        {
            --Indent;
            WriteIndent();
            WriteLine(BracketR);
        }

        public BlockHelper Block(string _firstLine = null, params object[] _args)
        {
            return new BlockHelper(this, _firstLine, _args);
        }

        public string BracketL { get; set; }
        public string BracketR { get; set; }
        public string Eol { get; set; }
        public string IndentSymbol { get; set; }
        public int Indent { get; set; }

        public class BlockHelper : IDisposable
        {
            public BlockHelper(CSharpWriter _writer, string _firstLine = null, params object[] _args)
            {
                m_Writer = _writer;
                if (_firstLine != null)
                {
                    m_Writer.WriteIndentBlank(_firstLine, _args);
                }
                m_Writer.OpenBlock();
            }

            private CSharpWriter m_Writer;

            public void Dispose()
            {
                m_Writer.CloseBlock();
            }
        }
    }
}