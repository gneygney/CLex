using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLex;

public class Env
{
    private readonly Dictionary<string, object> Values = new();
    readonly Env Enclosing;
    
    internal void Define(string name, object value)
    {
        Values[name] = value;
    }

    internal Env() { }

    internal Env(Env enclosing)
    {
        Enclosing = enclosing;
    }

    internal object Get(Token name)
    {
        if (Values.ContainsKey(name.Lexeme))
        {
            return Values[name.Lexeme];
        }
        if (Enclosing != null) return Enclosing.Get(name);
        
        throw new RuntimeException(name, 
            $"Undefined variable '{name.Lexeme}'.");
    }

    internal void Assign(Token name, object value)
    {
        if (Values.ContainsKey(name.Lexeme))
        {
            Values.Add(name.Lexeme, value);
            return;
        }

        if (Enclosing != null)
        {
            Enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeException(name,
            $"Undefined variable '{name.Lexeme}'");
    }
}
