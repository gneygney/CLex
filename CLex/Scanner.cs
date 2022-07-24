using static CLex.TokenType;

namespace CLex;

internal class Scanner
{
    private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        { "and", AND },
        { "class", CLASS},
        { "else", ELSE},
        { "false", FALSE},
        { "for", FOR},
        { "fun", FUN},
        { "if", IF},
        { "nil", NIL},
        { "or", OR},
        { "print", PRINT},
        { "return", RETURN},
        { "super", SUPER},
        { "this", THIS},
        { "true", TRUE},
        { "var", VAR},
        { "while", WHILE},
    };

    private string source;
    private List<Token> tokens = new List<Token>();
    int start = 0;
    int current = 0;
    int line = 1;

    public Scanner(string source)
    {
        this.source = source;
    }

    bool IsAtEnd() => current >= source.Length;

    internal List<Token> ScanTokens()
    {
        while(!IsAtEnd())
        {
            start = current;
            scanToken();
        }

        tokens.Add(new Token(EOF, "", null, line));
        return tokens;
    }

    private void scanToken()
    {
        char c = Advance();
        switch(c)
        {
            case '(': AddToken(LEFT_PAREN); break;
            case ')': AddToken(RIGHT_PAREN); break;
            case '{': AddToken(LEFT_BRACE); break;
            case '}': AddToken(RIGHT_BRACE); break;
            case ',': AddToken(COMMA); break;
            case '.': AddToken(DOT); break;
            case '-': AddToken(MINUS); break;
            case '+': AddToken(PLUS); break;
            case ';': AddToken(SEMICOLON); break;
            case '*': AddToken(STAR); break;

            case '!': AddToken(Match('=') ? BANG_EQUAL : BANG); break;
            case '=': AddToken(Match('=') ? EQUAL_EQUAL : EQUAL); break;
            case '<': AddToken(Match('=') ? LESS_EQUAL : LESS); break;
            case '>': AddToken(Match('=') ? GREATER_EQUAL : GREATER); break;
            


            case '/':
                if(Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                } 
                else
                {
                    AddToken(SLASH);
                }
                break;

            case ' ':
            case '\r':
            case '\t':
                break;
            
            case '\n':
                line++;
                break;

            case '"': StringLiteral(); break;

            default: 
                if(IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    Lox.Error(line, "Unexpected character."); 
                }
                break;
        }
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        var text = source.SubStartEnd(start, current);
        var isKeyword = keywords.TryGetValue(text, out TokenType type);
        if ( !isKeyword)
        {
            type = IDENTIFIER;
        }

        AddToken(type);
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
                c == '_';
    }

    private void Number()
    {
        while (IsDigit(Peek())) Advance();

        // Look for a fractional part.
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // Consume the "."
            Advance();

            while (IsDigit(Peek())) Advance();
        }

        AddToken(NUMBER,
            double.Parse(source.SubStartEnd(start, current)));
    }

    private char PeekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }

    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private void StringLiteral()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(line, "Unterminated string.");
            return;
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        string value = source.SubStartEnd(start + 1, current - 1);
        AddToken(STRING, value);
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        return true;
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object literal)
    {
        var text = source.SubStartEnd(start, current);
        tokens.Add(new Token(type, text, literal, line));
    }

    private char Advance()
    {
        return source[current++];
    }
}