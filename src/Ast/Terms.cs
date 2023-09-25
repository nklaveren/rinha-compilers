namespace Ast;

public record Empty() : Term;

public record Int(int Value) : Term
{
    public override void Evaluate(StringBuilder sb) => sb.Append(Value);
}

public record Call(IAstTerm[] Arguments, IAstTerm Callee) : Term
{
    public override void Evaluate(StringBuilder sb)
    {
        Callee.Evaluate(sb);
        sb.Append('(');
        foreach (var arg in Arguments)
        {
            arg.Evaluate(sb);
            if (arg != Arguments.Last()) sb.Append(',');
        }
        sb.Append(')');
    }
}

public record Str(string Value) : Term
{
    public override void Evaluate(StringBuilder sb) => sb.Append($"\"{Value}\"");
}

public record Var(string Text) : Term
{
    public override void Evaluate(StringBuilder sb) => sb.Append(Text);
}

public record Binary(string Op, IAstTerm Lhs, IAstTerm Rhs) : Term
{
    private static readonly Dictionary<string, string> _operatorMap = new()
    {
        ["Add"] = "+",
        ["Sub"] = "-",
        ["Mul"] = "*",
        ["Div"] = "/",
        ["Rem"] = "%",
        ["Eq"] = "==",
        ["Neq"] = "!=",
        ["Lt"] = "<",
        ["Gt"] = ">",
        ["Lte"] = "<=",
        ["Gte"] = ">=",
        ["And"] = "&&",
        ["Or"] = "||",
    };

    public override void Evaluate(StringBuilder sb)
    {
        Lhs.Evaluate(sb);
        sb.Append($" {_operatorMap[Op]} ");
        Rhs.Evaluate(sb);
    }
}

public record If(IAstTerm Condition, IAstTerm Then, IAstTerm Otherwise) : AstTermStructure
{
    protected override void TranslateFirst(StringBuilder sb)
    {
        void AppendReturn(IAstTerm astNode)
        {
            sb.Append($"\r\r return ");
            astNode.Evaluate(sb);
            sb.Append(';').AppendLine().Append('}');
        };

        sb.Append($"if (");
        Condition.Evaluate(sb);
        sb.Append(')').AppendLine()
        .Append('{').AppendLine();
        AppendReturn(Then);
        if (Otherwise is not null)
        {
            sb.Append("\nelse { \n\r\r");
            AppendReturn(Otherwise);
        }
    }
}

public record Function(IAstTerm Value, string[] Parameters) : AstTermStructure()
{
    protected override void TranslateFirst(StringBuilder sb)
    {
        sb.Append('(');
        StringBuilder subSb = new();
        Value.Evaluate(subSb);
        foreach (var parameter in Parameters)
        {
            sb.Append("int ");
            sb.Append(parameter);
            if (parameter != Parameters.Last()) _ = sb.Append(", ");
        }

        sb.Append(") { \n");


        sb.AppendLine().Append(subSb).AppendLine("}");
    }
}

public record Let(string Name, IAstTerm Value) : AstTermStructure
{
    protected override void TranslateFirst(StringBuilder sb)
    {
        StringBuilder subSb = new();
        Value.Evaluate(subSb);
        var isFunction = Value is Function;
        if (isFunction)
        {
            sb.Append("int ").Append(Name).Append(subSb);
            return;
        }

        MakeVar(sb, subSb);
    }

    private void MakeVar(StringBuilder sb, StringBuilder subSb)
    {
        sb.Append("var ").Append(Name);
        sb.Append(" = ")
         .Append(subSb)
         .Append(';')
         .AppendLine();
    }
}

public record Print(IAstTerm Value) : Term
{
    public override void Evaluate(StringBuilder sb)
    {
        StringBuilder subSb = new();
        Value.Evaluate(subSb);
        if (Value is Tuple<int, int> tuple)
        {
            sb.AppendLine($"System.Console.WriteLine({subSb}.Item1);");
            sb.AppendLine($"System.Console.WriteLine({subSb}.Item2);");
        }
        else
        {

            sb.AppendLine($"System.Console.WriteLine({subSb});");
        }
    }
}
public record First(IAstTerm Value) : Term
{
    public override void Eval()
    {
        StringBuilder subSb = new();
        Value.Evaluate(subSb);
    }
}

public record Second(IAstTerm Value) : Term
{
    public override void Evaluate(StringBuilder sb)
    {
        StringBuilder subSb = new();
        Value.Evaluate(subSb);
        sb.AppendLine($"System.Console.WriteLine({subSb});");
    }
}

public record Tuple(IAstTerm First, IAstTerm Second) : AstTermStructure
{
    protected override void TranslateFirst(StringBuilder sb)
    {
        StringBuilder subSb = new();
        First.Evaluate(subSb);
        sb.Append("(int ");
        sb.Append(subSb);
        sb.Append(", int ");
        Second.Evaluate(subSb);
        sb.Append(");");
    }
}