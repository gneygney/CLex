﻿namespace CLex;

internal partial class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: Clex.exe [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            Lox.RunFile(args[0]);
        }
        else
        {
            Lox.RunPrompt();
        }
    }


}