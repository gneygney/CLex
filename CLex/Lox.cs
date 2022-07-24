using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLex;

class Lox
{
    public static bool HadError { get; private set; }
    public static bool HadRuntimeError { get; private set; }

    private static Interpreter Interpret { get; set; } = new();

    public static void RunFile(string v)
    {
        var file = File.ReadAllLines(v);

        Run(string.Join(" ", file));

        if (HadError) Environment.Exit(65);
    }

    internal static void RuntimeError(RuntimeException error)
    {
        Console.WriteLine(error.Message +
            $"\n[line] {error.token.Line}]");
        HadRuntimeError = true;
    }

    public static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line != null) Run(line);
            HadError = false;
        }
    }

    static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var statements = parser.Parse();
        var printer = new AstPrinter();
        //Console.WriteLine(printer.Print(expresion));
        if (HadError) Environment.Exit(65);

        Interpret.Interpret(statements);
        
        if (HadRuntimeError) Environment.Exit(70);
        
        //foreach (var token in tokens)
        //{
        //    Console.WriteLine(token);
        //}
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }
    
    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
            
        } else
        {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }

    static void Report(int line, string where, string message)
    {
        Console.WriteLine($"[line {line}] Error {where}: {message}");
        HadError = true;
    }

}
