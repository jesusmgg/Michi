using System.Collections.Generic;

namespace Michi.CodeAnalysis.Syntax
{
    sealed class Parser
    {
        int position;
        List<SyntaxToken> tokens = new List<SyntaxToken>();
        List<string> diagnostics = new List<string>();

        public List<string> Diagnostics => diagnostics;
        
        public Parser(string text)
        {
            Lexer lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();

                if (token.Kind != SyntaxKind.BadToken &&
                    token.Kind != SyntaxKind.WhitespaceToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);
            
            diagnostics.AddRange(lexer.Diagnostics);
        }

        SyntaxToken Peek(int offset = 0)
        {
            int index = position + offset;
            if (index >= tokens.Count) { return tokens[^1]; }

            return tokens[index];
        }

        SyntaxToken Current => Peek();

        SyntaxToken NextToken()
        {
            SyntaxToken current = Current;
            position++;
            return current;
        }
        
        SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind) { return NextToken(); }
            
            diagnostics.Add($"ERROR: unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }
        
        public SyntaxTree Parse()
        {
            ExpressionSyntax expression = ParseExpression();
            SyntaxToken endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(diagnostics, expression, endOfFileToken);
        }

        ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;

            int unaryOperatorPrecedence = Current.Kind.GetUnaryOperationPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                int precedence = Current.Kind.GetBinaryOperationPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence) { break; }

                SyntaxToken operatorToken = NextToken();
                ExpressionSyntax right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                SyntaxToken left = NextToken();
                ExpressionSyntax expression = ParseExpression();
                SyntaxToken right = MatchToken(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }
            
            SyntaxToken numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }
    }
}