namespace CLex;

public abstract class Stmt {
  public interface Visitor<T> {
    T visitBlockStmt(Block stmt);
    T visitExpressionStmt(Expression stmt);
    T visitPrintStmt(Print stmt);
    T visitVarStmt(Var stmt);
  }
  public class Block : Stmt {
    public readonly List<Stmt> Statements;

    public Block(List<Stmt> statements) {
      Statements = statements;
    }

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitBlockStmt(this);
    }
  }
  public class Expression : Stmt {
    public readonly Expr Express;

    public Expression(Expr express) {
      Express = express;
    }

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitExpressionStmt(this);
    }
  }
  public class Print : Stmt {
    public readonly Expr Express;

    public Print(Expr express) {
      Express = express;
    }

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitPrintStmt(this);
    }
  }
  public class Var : Stmt {
    public readonly Token Name;
    public readonly Expr Initializer;

    public Var(Token name, Expr initializer) {
      Name = name;
      Initializer = initializer;
    }

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitVarStmt(this);
    }
  }

  public abstract T Accept<T>(Visitor<T> visitor);
}
