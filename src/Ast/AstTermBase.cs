namespace Ast;

public interface IAstTerm
{
    void Evaluate(StringBuilder sb);
    void SetNext(IAstTerm next);
    void Eval<T>(out T result);
}

public abstract record Term() : IAstTerm
{
    protected IAstTerm? Next { get; set; }
    public virtual void Evaluate(StringBuilder sb) { }
    public virtual void Eval<T>(out T result) { result = default!; }
    public virtual void Eval()
    {
        var sb = new StringBuilder();
        Evaluate(sb);
    }

    public void SetNext(IAstTerm next) => Next = next;
}

public abstract record AstTermStructure() : Term
{
    protected abstract void TranslateFirst(StringBuilder sb);
    public override void Evaluate(StringBuilder sb)
    {
        TranslateFirst(sb);
        Next?.Evaluate(sb);
    }
}


public record AstNodeRoot
{
    public AstNodeRoot(IAstTerm expression) => Expression = expression;
    [JsonConverter(typeof(AstNodeJsonConverter)), JsonPropertyName("expression")]
    public IAstTerm Expression { get; }
}

[JsonSerializable(typeof(AstNodeRoot), GenerationMode = JsonSourceGenerationMode.Serialization)]
public partial class AstNodeRootContext : JsonSerializerContext { }