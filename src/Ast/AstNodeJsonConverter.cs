namespace Ast;

public class AstNodeJsonConverter : JsonConverter<IAstTerm>
{
    private const string VALUE = "value";
    private const string NAME = "name";
    private const string TEXT = "text";
    private const string KIND = "kind";
    private const string NEXT = "next";

    public override IAstTerm Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => ExtractKind(JsonElement.ParseValue(ref reader));

    private static IAstTerm ExtractKind(JsonElement? root)
    {
        if (root is null) return new Empty();

        JsonElement document = root.Value;
        JsonElement GetValue() => document.GetProperty(VALUE)!;
        string GetName() => document.GetProperty(NAME).GetProperty(TEXT).GetString()!;

        Binary GetBinary() => new(
            document.GetProperty(nameof(Binary.Op).ToLower()).GetString()!,
             ExtractKind(document.GetProperty(nameof(Binary.Lhs).ToLower())),
             ExtractKind(document.GetProperty(nameof(Binary.Rhs).ToLower())));

        string[] Parameters() => document.TryGetProperty(nameof(Function.Parameters).ToLower(), out var paramsDoc) ?
            paramsDoc.EnumerateArray().Select(x => x.GetProperty(TEXT).GetString()!).ToArray() : Array.Empty<string>();

        Function GetFunction() => new(
            ExtractKind(GetValue()), Parameters());

        If GetIf() => new(
             ExtractKind(document.GetProperty(nameof(If.Condition).ToLower())),
             ExtractKind(document.GetProperty(nameof(If.Then).ToLower())),
             ExtractKind(document.GetProperty(nameof(If.Otherwise).ToLower())));

        string kind = document.GetProperty(KIND).GetString() ?? throw new InvalidOperationException("Kind is null");
        IAstTerm value = kind switch
        {
            nameof(Print) => new Print(ExtractKind(GetValue())),
            nameof(Let) => new Let(GetName(), ExtractKind(GetValue())),
            nameof(Function) => GetFunction(),
            nameof(Binary) => GetBinary(),
            nameof(Var) => new Var(document.GetProperty(nameof(Var.Text).ToLower()).GetString()!),
            nameof(Int) => new Int(GetValue().GetInt32()!),
            nameof(Str) => new Str(GetValue().GetString()!),
            nameof(If) => GetIf(),
            nameof(Call) => new Call(
                document.GetProperty(nameof(Call.Arguments).ToLower()).EnumerateArray()
                .Select((x) => ExtractKind(x)).ToArray(),
                 ExtractKind(document.GetProperty(nameof(Call.Callee).ToLower()))
            ),

            _ => throw new NotSupportedException($"AST node of kind {kind} is not supported."),
        };
        
        if (document.TryGetProperty(NEXT, out var next))
        {
            value.SetNext(ExtractKind(next));
        }
        return value;
    }

    public override void Write(Utf8JsonWriter writer, IAstTerm value, JsonSerializerOptions options)
        => throw new NotImplementedException();
}