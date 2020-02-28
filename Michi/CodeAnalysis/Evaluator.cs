using System;
using Michi.CodeAnalysis.Binding;
using Michi.CodeAnalysis.Syntax;

namespace Michi.CodeAnalysis
{
    sealed class Evaluator
    {
        readonly BoundExpression root;
        
        public Evaluator(BoundExpression root)
        {
            this.root = root;
        }

        public object Evaluate()
        {
            return EvaluateExpression(root);
        }

        object EvaluateExpression(BoundExpression node)
        {
            if (node is BoundLiteralExpression n) { return n.Value; }

            if (node is BoundUnaryExpression u)
            {
                var operand = (int) EvaluateExpression(u.Operand);

                if (u.OperatorKind == BoundUnaryOperatorKind.Identity) { return operand; }
                else if (u.OperatorKind == BoundUnaryOperatorKind.Negation) { return -operand; }
                else { throw new Exception($"Unexpected unary operator <{u.OperatorKind}>"); }
            }

            if (node is BoundBinaryExpression b)
            { 
                var left = (int) EvaluateExpression(b.Left);
                var right = (int) EvaluateExpression(b.Right);

                if (b.OperatorKind == BoundBinaryOperatorKind.Addition) { return left + right; }
                else if (b.OperatorKind == BoundBinaryOperatorKind.Subtraction) { return left - right; }
                else if (b.OperatorKind == BoundBinaryOperatorKind.Multiplication) { return left * right; }
                else if (b.OperatorKind == BoundBinaryOperatorKind.Division) { return left / right; }
                else { throw new Exception($"Unexpected binary operator <{b.OperatorKind}>"); }
            }
            
            throw new Exception($"Unexpected node <{node .Kind}>");
        }
    }
}