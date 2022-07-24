namespace CLex;

using System;
using static CLex.TokenType;

public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
{
    private Env Environ = new();

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach(var stmt in statements)
            {
                Execute(stmt);
            }
        }
        catch (RuntimeException error)
        {
            Lox.RuntimeError(error);
        }
    }

    private void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }

    private string Stringify(object val)
    {
        if (val == null) return "nil";
        
        if (val is double)
        {
            var text = val.ToString();
            if (text.EndsWith(".0"))
                return text.Substring(0, text.Length - 2);
            return text;
        }
        return val.ToString();
    }

    public object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    public object visitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case PLUS:
                if (left is double && right is double)
                    return (double)left + (double)right;
                else if (left is string && right is string)
                    return (string)left + (string)right;
                throw new RuntimeException(expr.Op, "Operands must be two numbers or two strings");

            case MINUS:
                CheckNumberOperand(expr.Op,left, right);
                return (double)left - (double)right;
            case SLASH:
                CheckNumberOperand(expr.Op, left, right);
                return (double)left / (double)right;
            case STAR:
                CheckNumberOperand(expr.Op, left, right);
                return (double)left * (double)right;
            case BANG_EQUAL:
                CheckNumberOperand(expr.Op, left, right);
                return !IsEqual(left, right);
            case EQUAL_EQUAL:
                CheckNumberOperand(expr.Op, left, right);
                return IsEqual(left, right);
            case GREATER:
                CheckNumberOperand(expr.Op, left, right);
                return (double)left > (double)right;
            case GREATER_EQUAL:
                CheckNumberOperand(expr.Op, left, right);
                return (double)left >= (double)right;
            case LESS:
                CheckNumberOperand(expr.Op, left, right);
                return (double)left < (double)right;
            case LESS_EQUAL:
                CheckNumberOperand(expr.Op, left, right);
                return (double)left <= (double)right;
        }
        throw new NotImplementedException();

    }

    private void CheckNumberOperand(Token op, object left, object right)
    {
        if (left is double && right is double) return;
        throw new RuntimeException(op, "Operands must be two number.");

    }

    private void CheckNumberOperand(Token op, object operand)
    {
        if (operand is double) return;
        throw new RuntimeException(op, "Operand must be a number.");
    }

    private bool IsEqual(object left, object right)
    {
        if (left == null && right == null)
            return true;
        if (left == null)
            return false;
        return left.Equals(right);
    }

    public object visitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object visitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value;
    }

    public object visitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);
        
        switch(expr.Op.Type)
        {
            case MINUS:
                CheckNumberOperand(expr.Op, right);
                return -(double)right;
            case BANG:
                return !IsTruthy(right);
        }

        //Unreachable
        return null;
    }

    private bool IsTruthy(object obj)
    {
        if (obj == null) return false;
        if (obj is bool) return (bool)obj;
        return true;
    }

    object Stmt.Visitor<object>.visitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Express);
        return null;
    }

    object Stmt.Visitor<object>.visitPrintStmt(Stmt.Print stmt)
    {
        var val = Evaluate(stmt.Express);
        Console.WriteLine(Stringify(val));
        return null;
    }

    public object visitVariableExpr(Expr.Variable expr)
    {
        return Environ.Get(expr.Name);
    }

    public object visitVarStmt(Stmt.Var stmt)
    {
        object val = null;
        if (stmt.Initializer != null)
        {
            val = Evaluate(stmt.Initializer);
        }
        Environ.Define(stmt.Name.Lexeme, val);
        return null;
    }

    public object visitAssignExpr(Expr.Assign expr)
    {
        var value = Evaluate(expr.Value);
        Environ.Assign(expr.Name, value);
        return value;
    }

    public object visitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Env(Environ));
        return null;
    }

    private void ExecuteBlock(List<Stmt> statements, Env env)
    {
        var previous = Environ;
        try
        {
            Environ = env;

            foreach (var stmt in statements)
            {
                Execute(stmt);
            }
        } finally
        {
            Environ = previous;
        }
    }
}
