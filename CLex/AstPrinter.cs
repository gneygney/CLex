using System.Text;

namespace CLex;

public class AstPrinter : Expr.Visitor<string>
{
    string Expr.Visitor<string>.visitBinaryExpr(Expr.Binary expr)
    {
        return Parenthesize(expr.Op.Lexeme,
                        expr.Left, expr.Right);
    }

    string Expr.Visitor<string>.visitGroupingExpr(Expr.Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
    }

    string Expr.Visitor<string>.visitLiteralExpr(Expr.Literal expr)
    {
        if (expr.Value == null) return "nil";
        return expr.Value.ToString();
    }

    string Expr.Visitor<string>.visitUnaryExpr(Expr.Unary expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Right);
    }

    private string Parenthesize(string name, params Expr[] exprs)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("(").Append(name);
        foreach (var expr in exprs)
        {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }
        builder.Append(")");

        return builder.ToString();
    }

    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string visitAssignExpr(Expr.Assign expr)
    {
        return null;
        //throw new NotImplementedException();
    }

    public string visitVariableExpr(Expr.Variable expr)
    {
        return null;
        //throw new NotImplementedException();
    }
}
