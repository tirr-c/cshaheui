using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using CShAheui.Core;

namespace CShAheui.App
{
    class AheuiCompiler
    {
        class CursorReserver
        {
            public Cursor Cursor { get; private set; }
            private Instruction instr;

            public CursorReserver(Cursor cursor, Instruction instr)
            {
                Cursor = cursor;
                this.instr = instr;
            }

            public Action<int> MakeJumpSetter()
            {
                if (instr == null) return null;
                return (id) => instr.ReverseJumpTo = id;
            }
        }

        class Instruction
        {
            public char Command { get; private set; }
            public int Argument { get; private set; }
            public int ReverseJumpTo { get; set; }
            public bool IsReferenced { get; set; }

            public Instruction(char command, int argument)
            {
                Command = command;
                Argument = argument;
                ReverseJumpTo = -1;
                IsReferenced = false;
            }

            public override string ToString()
            {
                return $"{Command}, {Argument}, {ReverseJumpTo}";
            }
        }
        
        private Dictionary<string, int> ExecuteCache;
        private List<Instruction> Instructions;
        private Stack<CursorReserver> CursorStack;

        string[] code;
        Cursor cursor;
        Action<int> jumpAction;

        public AheuiCompiler()
        {
        }

        public void Load(string aheui)
        {
            ExecuteCache = new Dictionary<string, int>();
            Instructions = new List<Instruction>();
            CursorStack = new Stack<CursorReserver>();
            code = aheui.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            CursorStack.Push(new CursorReserver(new Cursor(), null));
            while (CursorStack.Count > 0)
            {
                var p = CursorStack.Pop();
                cursor = p.Cursor;
                jumpAction = p.MakeJumpSetter();
                while (VirtualStep()) ;
            }
        }

        public Func<int> Compile(bool bigint = false, bool binary = false)
        {
            CodeCompileUnit unit = new CodeCompileUnit();
            CodeNamespace ns = new CodeNamespace("Aheui");
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Numerics"));
            ns.Imports.Add(new CodeNamespaceImport("CShAheui.Core"));
            unit.Namespaces.Add(ns);

            CodeTypeDeclaration aheui = new CodeTypeDeclaration("AheuiExecutor");
            Type baseType = bigint ? typeof(BigIntAheuiBase) : typeof(IntAheuiBase);
            aheui.BaseTypes.Add(baseType);

            CodeMemberMethod execute = new CodeMemberMethod();
            execute.ReturnType = new CodeTypeReference(typeof(int));
            execute.Name = "ExecuteInternal";
            execute.Attributes = MemberAttributes.Family | MemberAttributes.Override;

            CodeStatementCollection collection = new CodeStatementCollection();
            for (int i = 0; i < Instructions.Count; i++)
            {
                var instr = Instructions[i];
                CodeLabeledStatement label = null;
                if (instr.IsReferenced)
                {
                    label = new CodeLabeledStatement($"lineno_{i}");
                }
                if (instr.Command == 'J')
                {
                    collection.Add(new CodeGotoStatement($"lineno_{instr.Argument}"));
                }
                else if (instr.Command == 'ㅎ')
                {
                    CodeFieldReferenceExpression storage = new CodeFieldReferenceExpression(
                        new CodeThisReferenceExpression(), "storage");
                    CodeMethodInvokeExpression pop = new CodeMethodInvokeExpression(storage, "MakeReturnValue");
                    collection.Add(new CodeMethodReturnStatement(pop));
                }
                else if (instr.Command != 'ㅇ')
                {
                    CodeMethodInvokeExpression inv = new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(), "Step",
                        new CodePrimitiveExpression(instr.Command), new CodePrimitiveExpression(instr.Argument));
                    if (instr.ReverseJumpTo != -1)
                    {
                        CodeConditionStatement reverse = new CodeConditionStatement(
                            inv, new CodeGotoStatement($"lineno_{instr.ReverseJumpTo}"));
                        collection.Add(reverse);
                    }
                    else
                    {
                        collection.Add(new CodeExpressionStatement(inv));
                    }
                }
                if (label != null)
                {
                    if (collection.Count > 0) label.Statement = collection[0];
                    else collection.Add(label);
                    collection[0] = label;
                }
                execute.Statements.AddRange(collection);
                collection.Clear();
            }
            aheui.Members.Add(execute);
            ns.Types.Add(aheui);

            var provider = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            if (binary)
            {
                CodeEntryPointMethod entry = new CodeEntryPointMethod();
                var returnStmt = new CodeMethodReturnStatement(new CodeMethodInvokeExpression(
                    new CodeObjectCreateExpression("Aheui.AheuiExecutor"), "Execute"));
                entry.Statements.Add(returnStmt);
                entry.ReturnType = new CodeTypeReference(typeof(int));
                aheui.Members.Add(entry);
                cp.OutputAssembly = "out.exe";
            }
            cp.GenerateExecutable = binary;
            cp.GenerateInMemory = !binary;
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Numerics.dll");
            cp.ReferencedAssemblies.Add("CShAheui.Core.dll");
            cp.CompilerOptions = "/optimize";
#if DEBUG
            cp.TempFiles = new TempFileCollection(".", true);
#endif

            var cpass = provider.CompileAssemblyFromDom(cp, unit);
            if (!binary)
            {
                var assembly = cpass?.CompiledAssembly;
                object aheuiExecutor = assembly.CreateInstance("Aheui.AheuiExecutor");
                var realExecute = aheuiExecutor.GetType().GetMethod("Execute");

                return () => (int)realExecute.Invoke(aheuiExecutor, null);
            }
            else return null;
        }

        private bool VirtualStep()
        {
            // 1. Poll
            Hangul instruction = new Hangul();
            if (code[cursor.Y].Length > cursor.X)
            {
                instruction = new Hangul(code[cursor.Y][cursor.X]);
            }
            // 2. Execute
            string id = $"{cursor.X}_{cursor.Y}";
            if (instruction.Direction == '\0' ||
                instruction.Direction == 'ㅣ' ||
                instruction.Direction == 'ㅡ' ||
                instruction.Direction == 'ㅢ')
            {
                id += $"_{cursor.V.Direction}_{cursor.V.Speed}";
            }

            bool reversible = false;
            Instruction instr = null;
            if (ExecuteCache.ContainsKey(id))
            {
                Instructions[ExecuteCache[id]].IsReferenced = true;
                if (jumpAction == null)
                {
                    instr = new Instruction('J', ExecuteCache[id]);
                    Instructions.Add(instr);
                }
                else jumpAction(ExecuteCache[id]);
                jumpAction = null;
                return false;
            }
            else
            {
                if (!instruction.IsNop)
                {
                    switch (instruction.Command)
                    {
                        case 'ㄷ':
                        case 'ㄸ':
                        case 'ㅌ':
                        case 'ㄴ':
                        case 'ㄹ':
                        case 'ㅁ':
                        case 'ㅃ':
                        case 'ㅍ':
                        case 'ㅆ':
                        case 'ㅈ':
                        case 'ㅊ':
                            reversible = true;
                            break;
                    }
                    instr = new Instruction(instruction.Command, instruction.Argument);
                    ExecuteCache[id] = Instructions.Count;
                    if (jumpAction != null)
                    {
                        instr.IsReferenced = true;
                        jumpAction(Instructions.Count);
                    }
                    Instructions.Add(instr);
                    jumpAction = null;
                }
                if (instruction.Command == 'ㅎ') return false;
            }
            // 3. Move
            if (reversible)
            {
                Cursor nextCursor = cursor.Clone() as Cursor;
                nextCursor.Move(code, instruction.Direction, true);
                CursorStack.Push(new CursorReserver(nextCursor, instr));
            }
            cursor.Move(code, instruction.Direction, false);

            return true;
        }
    }
}
