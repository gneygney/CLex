namespace CLex;
 
abstract class ExprTemplate
{
    internal readonly ExprTemplate? Left;
    internal readonly ExprTemplate? Right;
    internal readonly Token? Op;

    internal ExprTemplate(ExprTemplate left, Token op, ExprTemplate right)
    {
        Left = left;
        Op = op;
        Right = right;
    }
}

class Binary : ExprTemplate
{
    Binary(ExprTemplate left, Token op, ExprTemplate right) : base(left, op, right)
    {
    }

}
