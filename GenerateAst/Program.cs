using System.Text;

//expression     → literal
//               | unary
//               | binary
//               | grouping ;

//literal        → NUMBER | STRING | "true" | "false" | "nil" ;
//grouping       → "(" expression ")" ;
//unary          → ( "-" | "!" ) expression ;
//binary         → expression operator expression ;
//operator       → "==" | "!=" | "<" | "<=" | ">" | ">="
//               | "+"  | "-"  | "*" | "/" ;

//if (args.Length != 1)
//{
//    Console.WriteLine("Usage: generate_ast <output directory>");
//    Environment.Exit(1);
//}
string outputDir = "D:\\dev\\csharp\\CraftingInterpreters\\CLex\\";

DefineAst(outputDir, "Expr", new List<string>() {
    "Assign : Token name, Expr value",
     "Binary   : Expr left, Token op, Expr right",
     "Grouping : Expr expression",
     "Literal  : Object value",
     "Unary    : Token op, Expr right",
     "Variable : Token name"
});

DefineAst(outputDir, "Stmt", new List<string>() {
"Block : List<Stmt> statements",
"Expression : Expr express",
"Print : Expr express",
"Var  : Token name, Expr initializer"});

static void DefineAst(string outputDir, string baseName, List<string> types)
{
    string path = outputDir + baseName + ".cs";
    StringBuilder? writer = new StringBuilder();

    writer.AppendLine("namespace CLex;");
    writer.AppendLine();
    writer.AppendLine("public abstract class " + baseName + " {");
    
    DefineVisitor(writer, baseName, types);

    foreach (string? type in types)
    {
        string className = type.Split(":")[0].Trim();
        string fields = type.Split(":")[1].Trim();
        DefineType(writer, baseName, className, fields);
    }

    // The base accept() method.
    writer.AppendLine();
    writer.AppendLine("  public abstract T Accept<T>(Visitor<T> visitor);");

    writer.AppendLine("}");
    File.WriteAllText(path, writer.ToString());
}

static void DefineType(
    StringBuilder writer, string baseName,
    string className, string fieldList)
{
    writer.AppendLine("  public class " + className + " : " +
        baseName + " {");

    // Fields.
    string[]? fields = fieldList.Split(", ");

    foreach (string? field in fields)
    {
        string[]? classAndName = field.Split(" ");
        writer.AppendLine($"    public readonly {classAndName[0]} {FirstCharToUpper(classAndName[1])};");
    }

    writer.AppendLine();

    // Constructor.
    writer.AppendLine("    public " + className + "(" + fieldList + ") {");
    // Store parameters in fields.
    foreach (string? field in fields)
    {
        string name = field.Split(" ")[1];
        writer.AppendLine("      " + FirstCharToUpper(name) + " = " + name + ";");
    }

    writer.AppendLine("    }");

    writer.AppendLine();
    writer.AppendLine("    public override T Accept<T>(Visitor<T> visitor) {");
    writer.AppendLine("      return visitor.visit" +
        className + baseName + "(this);");
    writer.AppendLine("    }");

    writer.AppendLine("  }");
}

static void DefineVisitor(StringBuilder writer, string baseName, List<string> types)
{
    writer.AppendLine("  public interface Visitor<T> {");

    foreach (string? type in types)
    {
        string typeName = type.Split(":")[0].Trim();
        writer.AppendLine("    T visit" + typeName + baseName + "(" +
            typeName + " " + baseName.ToLower() + ");");
    }

    writer.AppendLine("  }");
}

static string FirstCharToUpper(string input) =>
    input switch
    {
        null => throw new ArgumentNullException(nameof(input)),
        "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
        _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
    };