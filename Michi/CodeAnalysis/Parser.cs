using System.Collections.Generic;

namespace Michi.CodeAnalysis
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

        ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }

        ExpressionSyntax ParseTerm()
        {
            ExpressionSyntax left = ParseFactor();

            while (Current.Kind == SyntaxKind.PlusToken ||
                   Current.Kind == SyntaxKind.MinusToken)
            {
                SyntaxToken operatorToken = NextToken();
                ExpressionSyntax right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }
        
        ExpressionSyntax ParseFactor()
        {
            ExpressionSyntax left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxKind.StarToken ||
                   Current.Kind == SyntaxKind.SlashToken)
            {
                SyntaxToken operatorToken = NextToken();
                ExpressionSyntax right = ParsePrimaryExpression();
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