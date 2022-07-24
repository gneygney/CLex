
using static CLex.TokenType;

namespace CLex;

public class Parser
{
    private readonly List<Token> Tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        Tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        List<Stmt> statements = new();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }
        return statements;
    }

    private Stmt Declaration()
    {
        try
        {
            if (Match(VAR)) return VarDeclaration();
            return Statement();
        } 
        catch ( ParserException e)
        {
            Synchronize();
            return null;
        }
    }

    private Stmt VarDeclaration()
    {
        var name = Consume(IDENTIFIER, "Expect variable name.");
        Expr initializer = null;
        if (Match(EQUAL))
        {
            initializer = Expression();
        }
        Consume(SEMICOLON, "Expect ';' after variable declaration.");
        return new Stmt.Var(name, initializer);
    }

    private Stmt Statement()
    {
        if (Match(PRINT)) return PrintStatement();
        if (Match(LEFT_BRACE)) return new Stmt.Block(Block());

        return ExpressionStatement();
    }

    private List<Stmt> Block()
    {
        var statements = new List<Stmt>();

        while(!Check(RIGHT_BRACE) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(RIGHT_BRACE, "Expect '}' after block.");

        return statements;
    }

    private Stmt ExpressionStatement()
    {
        var expr = Expression();
        Consume(SEMICOLON, "Expect ';' after value");
        return new Stmt.Expression(expr);
    }

    private Stmt PrintStatement()
    {
        var val = Expression();
        Consume(SEMICOLON, "Expect ';' after value");
        return new Stmt.Print(val);
    }

    private Expr Expression()
    {
        return Assignment();
    }

    private Expr Assignment()
    {
        var expr = Equality();
        if (Match(EQUAL))
        {
            var equals = Previous();
            var value = Assignment();
            if (expr is Expr.Variable exprVar)
            {
                var name = exprVar.Name;
                return new Expr.Assign(name, value);
            }
            throw new ParserException(equals, "Invalid assignment target.");
        }
        return expr;
    }

    private Expr Equality()
    {
        var expr = Compare();

        while (Match(BANG_EQUAL, EQUAL_EQUAL))
        {
            Token? op = Previous();
            Expr? right = Compare();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    private Token Previous()
    {
        return Tokens[current - 1];
    }

    private Expr Compare()
    {
        Expr? expr = Term();

        while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
        {
            Token? op = Previous();
            Expr? right = Term();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    private Expr Term()
    {
        Expr? expr = Factor();
        while (Match(MINUS, PLUS))
        {
            Token? op = Previous();
            Expr? right = Factor();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    private Expr Factor()
    {
        Expr? expr = Unary();
        while (Match(SLASH, STAR))
        {
            Token? op = Previous();
            Expr? right = Unary();
            expr = new Expr.Binary(expr, op, right);
        }
        return expr;
    }

    private Expr Unary()
    {
        if (Match(BANG, MINUS))
        {
            Token? op = Previous();
            Expr? right = Unary();
            return new Expr.Unary(op, right);
        }
        return Primary();
    }

    private Expr Primary()
    {
        if (Match(FALSE))
        {
            return new Expr.Literal(false);
        }

        if (Match(TRUE))
        {
            return new Expr.Literal(true);
        }

        if (Match(NIL))
        {
            return new Expr.Literal(null);
        }

        if (Match(NUMBER, STRING))
        {
            return new Expr.Literal(Previous().Literal);
        }

        if(Match(IDENTIFIER))
        {
            return new Expr.Variable(Previous());
        }

        if (Match(LEFT_PAREN))
        {
            Expr? expr = Expression();
            _ = Consume(RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek(), "Expected expression.");
    }

    private Token Consume(TokenType type, string message)
    {
        return Check(type) ? Advance() : throw Error(Peek(), message);
    }

    private Exception Error(Token token, string message)
    {
        Lox.Error(token, message);
        return new ParserException();
    }

    private bool Match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                _ = Advance();
                return true;
            }
        }
        return false;
    }

    private void Synchronize()
    {
        _ = Advance();
        while (!IsAtEnd())
        {
            if (Previous().Type == SEMICOLON)
            {
                return;
            }

            switch (Peek().Type)
            {
                case CLASS:
                case FUN:
                case VAR:
                case FOR:
                case IF:
                case WHILE:
                case PRINT:
                case RETURN:
                    return;
            }
            _ = Advance();
        }
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            current++;
        }

        return Previous();
    }

    private bool Check(TokenType type)
    {
        return !IsAtEnd() && Peek().Type == type;
    }

    private Token Peek()
    {
        return Tokens[current];
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }
}
