namespace CLex;

public abstract class Expr {
  public interface Visitor<T> {
    T visitAssignExpr(Assign expr);
    T visitBinaryExpr(Binary expr);
    T visitGroupingExpr(Grouping expr);
    T visitLiteralExpr(Literal expr);
    T visitUnaryExpr(Unary expr);
    T visitVariableExpr(Variable expr);
  }
  public class Assign : Expr {
    public readonly Token Name;
    public readonly Expr Value;

    public Assign(Token name, Expr value) {
      Name = name;
      Value = value;
    }

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitAssignExpr(this);
    }
  }
  public class Binary : Expr {
    public readonly Expr Left;
    public readonly Token Op;
    public readonly Expr Right;

    public Binary(Expr left, Token op, Expr right) {
      Left = left;
      Op = op;
      Right = right;
    }

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitBinaryExpr(this);
    }
  }
  public class Grouping : Expr {
    public readonly Expr Expression;

    public Grouping(Expr expression) {
      Expression = expression;
    }

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitGroupingExpr(this);
    }
  }
  public class Literal : Expr {
    public readonly Object Value;

    public Literal(Object value) {
      Value = value;
    }

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitLiteralExpr(this);
    }
  }
  public class Unary : Expr {
    public readonly Token Op;
    public readonly Expr Right;

    public Unary(Token op, Expr right) {
      Op = op;
      Right = right;
    }

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitUnaryExpr(this);
    }
  }
  public class Variable : Expr {
    public readonly Token Name;

    public Variable(Token name) {
      Name = name;
    }

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitVariableExpr(this);
    }
  }

  public abstract T Accept<T>(Visitor<T> visitor);
}
