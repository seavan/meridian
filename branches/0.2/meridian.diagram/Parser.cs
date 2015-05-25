using System;
using System.Collections.Generic;
using System.Text;
using meridian.core;

namespace meridian.diagram
{
    public class Parser
    {
/*        public const string CMD_QUIT = "QUIT";
        public const string CMD_CREATE = "CREATE";
        public const string CMD_DROP = "DROP";
        public const string CMD_RENAME = "RENAME";
        public const string CMD_ALTER = "ALTER"; */
        public string ExpectedList(SyntaxElement _element)
        {
            var msg = new List<string>();
            var msgList = new SortedList<SyntaxElement, string>();
            msgList[SyntaxElement.seEol] = "end of line";
            msgList[SyntaxElement.seOperandType] = "operand type";
            msgList[SyntaxElement.seCommand] = "command";
            msgList[SyntaxElement.seIdentifier] = "identifier";
            msgList[SyntaxElement.seComplexIdentifier] = "complex identifier";

            foreach (var i in msgList.Keys)
            {
                if (_element.HasFlag(i))
                {
                    msg.Add(msgList[i]);
                }
            }

            return String.Join(", ", msg.ToArray());
        }

        public void ParseLine()
        {
            var cmd = GetCommand();
            if (string.IsNullOrEmpty(cmd))
            {
                Tracer.I.Debug("Command is empty. Continue...");
                return;
            }
            Tracer.I.Debug("Parser:ParseLine:: Got command {0}", cmd);

            if (!Executor.CommandExists(cmd))
            {
                Tracer.I.Error("Command {0} is not recognized. Continue...", cmd);
                return;
            }

            Executor.StartBuilding(cmd);
            while (true)
            {
                var expected = Executor.Expected();
                if (Eol)
                {
                    if (expected.HasFlag(SyntaxElement.seEol))
                    {
                        Tracer.I.Debug("End of line accepted");
                        break;
                    }
                    else
                    {
                        Tracer.I.Error("End of line. Expected: {0}", ExpectedList(expected));
                        Executor.AbortBuilding();
                        break;
                    }
                }
                else
                {
                    // optional
                    if (expected.HasFlag(SyntaxElement.seOptions))
                    {
                        var options = GetOptions();
                        if (options.Length > 0)
                        {
                            //
                            Tracer.I.Debug("Got options: {0}", options);
                            var parsedOptions = new SortedList<string, string>();
                            Executor.FeedOptions(parsedOptions);
                        }
                    }

                    if(expected.HasFlag(SyntaxElement.seOperandType))
                    {
                        var operandType = GetKeyword();
                        //
                        if (operandType.Length > 0)
                        {
                            Tracer.I.Debug("Got operand type: {0}", operandType);
                            if (!ElementTypeMapper.IsType(operandType))
                            {
                                Tracer.I.Error("No such type exists {0}", operandType);
                                return;
                            }

                            Executor.FeedOperandType(ElementTypeMapper.Map(operandType));
                        }
                    }
                    else
                        if (expected.HasFlag(SyntaxElement.seComplexIdentifier))
                        {
                            var complexIdent = GetComplexIdentifier();
                            //
                            if (complexIdent.Length > 0)
                            {
                                Tracer.I.Debug("Got complex identifier: {0}", complexIdent);
                                Executor.FeedOperand(complexIdent);
                            }
                        }
                        else
                            if (expected.HasFlag(SyntaxElement.seIdentifier))
                            {
                                var ident = GetIdentifier();
                                //
                                if (ident.Length > 0)
                                {
                                    Tracer.I.Debug("Got identifier: {0}", ident);
                                    Executor.FeedOperand(ident);
                                }
                            }



                }
            }

            // command complete, execute
            Executor.CompleteCommand();
        }

        public void Launch()
        {
            Tracer.I.Notice("Parser started");
            while (!Feeder.Eof())
            {
                // command format
                // COMMAND[(OPTIONS)] [OPERAND_TYPE_1 OPERAND_1 [SUBCOMMAND[(OPTIONS)] OPERAND_TYPE OPERAND]
                // COMMAND -> QUIT(0), CREATE(1), DROP(1), RENAME(1)
                // OPERAND_TYPE -> OPERAND_TYPE_KEYWORD [(parameter1, ...)]
                // OPERAND_TYPE -> 
                //                  PROTO(0), VIEW (enumerable1, enumerable2, enumerabl3 ...), UNION(enumerable1, enumerable2, enumerable3 ...)
                //                  ELEMENT(proto1, proto2, proto3 ...), COMPOSITION(proto1, proto2, proto3 ...), AGGREGATION (proto1, proto2, proto3 ...)
                //                  ASSOCIATION(proto1, proto2, proto3 ...)
                // OPERAND -> IDENTIFIER
                // OPTIONS - if value is omitted, option is considered boolean true
                // OPTIONS -> IDENTIFIER[=identifier][, ...]
                //
                Current = 0;
                Line = Feeder.Next().Trim();
                if (Line.Trim().ToUpper().Equals("QUIT"))
                {
                    Tracer.I.Notice("BYE");
                    return;
                }

  

                if (Line.Trim().ToUpper().Equals("GEN"))
                {
                    Tracer.I.Notice("Generator start");
                    Generator.Generate();
                    Tracer.I.Notice("Generator end");
                    return;
                }

                Tracer.I.Debug("Got line: {0}", Line);
                ParseLine();
                
            }
        }

        private string GetOptions()
        {
            return GetNextTo(ParenthesisOpen, NotParenthesisClose);
        }

        private string GetKeyword()
        {
            return GetCommand();
        }

        private string GetCommand()
        {
            return GetNextTo(IsIdentifierStartSymbol, IsIdentifierSymbol).ToUpper();
        }

        private string GetComplexIdentifier()
        {
            return GetNextTo(IsComplexIdentifierStartSymbol, IsComplexIdentifierSymbol);
        }

        private string GetIdentifier()
        {
            return GetNextTo(IsIdentifierStartSymbol, IsIdentifierSymbol);
        }

        private string GetNextTo(Func<char, bool> _first, Func<char, bool> _next)
        {
            var builder = new StringBuilder();

            while (!Eol && IsWhitespace(CurrentChar))
            {
                ++Current;
            }

            while (!Eol && _first(CurrentChar))
            {
                builder.Append(CurrentChar);
                _first = _next;
                ++Current;
            }

            return builder.ToString();
        }

        private bool IsWhitespace(char _c)
        {
            return Char.IsWhiteSpace(_c);
        }

        private bool IsIdentifierStartSymbol(char _c)
        {
            return M_IDENT_START.IndexOf(_c) >= 0;
        }

        private bool IsIdentifierSymbol(char _c)
        {
            return M_IDENT_CONT.IndexOf(_c) >= 0;
        }

        private bool IsComplexIdentifierStartSymbol(char _c)
        {
            return M_COMPLEX_IDENT_START.IndexOf(_c) >= 0;
        }

        private bool IsComplexIdentifierSymbol(char _c)
        {
            return M_COMPLEX_IDENT_CONT.IndexOf(_c) >= 0;
        }

        private bool ParenthesisOpen(char _c)
        {
            return _c.Equals('(');
        }

        private bool NotParenthesisClose(char _c)
        {
            return !_c.Equals(')');
        }


        private string M_IDENT_START = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        private string M_IDENT_CONT = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_1234567890";

        private string M_COMPLEX_IDENT_START = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        private string M_COMPLEX_IDENT_CONT = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_1234567890.";


        private char CurrentChar { get { return Line[Current]; } }
        private bool Eol { get { return Current >= Line.Length; } }
        private int Current { get; set; }
        private string Line { get; set; }
        public IFeeder Feeder { get; set; }
        public IExecutor Executor { get; set; }
        public IGenerator Generator { get; set; }
    }
}