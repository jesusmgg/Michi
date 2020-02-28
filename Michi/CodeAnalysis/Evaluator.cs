using System;

namespace Michi.CodeAnalysis
{
    class Evaluator
    {
        ExpressionSyntax root;
        
        public Evaluator(ExpressionSyntax root)
        {
            this.root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(root);
        }

        int EvaluateExpression(ExpressionSyntax node)
        {
            if (node is NumberExpressionSyntax n) { return (int) n.NumberToken.Value; }

            if (node is BinaryExpressionSyntax b)
            { 
                int left = EvaluateExpression(b.Left);
                int right = EvaluateExpression(b.Right);

                if (b.OperatorToken.Kind == SyntaxKind.PlusToken) { return left + right; }
                else if (b.OperatorToken.Kind == SyntaxKind.MinusToken) { return left - right; }
                else if (b.OperatorToken.Kind == SyntaxKind.StarToken) { return left * right; }
                else if (b.OperatorToken.Kind == SyntaxKind.SlashToken) { return left / right; }
                else
                {
                    throw new Exception($"Unexpected binary operator <{b.OperatorToken.Kind}>");   
                }
            }

            if (node is ParenthesizedExpressionSyntax p)
            {
                return EvaluateExpression(p.Expression);
            }
            
            throw new Exception($"Unexpected node <{node .Kind}>");
        }
    }
}