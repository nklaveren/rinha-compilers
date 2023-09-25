var fileName = args.Any() ? $"./files/{args.First()}" : "/var/rinha/source.rinha.json";

var file = await File.ReadAllBytesAsync(fileName);

var ast = JsonSerializer.Deserialize<AstNodeRoot>(file);
Console.WriteLine(ast);
var sb = new StringBuilder();

ast!.Expression.Evaluate(sb);
var str = sb.ToString();
var r = await CSharpScript.EvaluateAsync(str);