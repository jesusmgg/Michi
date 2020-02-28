using System;
using System.Collections.Generic;
using Michi.CodeAnalysis.Syntax;

namespace Michi.CodeAnalysis.Binding
{
    sealed class Binder
    {
        List<string> diagnostics = new List<string>();

        public List<string> Diagnostics => diagnostics;

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax) syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax) syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax) syntax);
                default:
                    throw new Exception($"Unexpected syntax: <{syntax.Kind}>");
            }
        }
        
        BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            BoundExpression boundOperand = BindExpression(syntax.Operand);
            BoundUnaryOperatorKind? boundOperationKind =
                BindUnaryOperatorKind(syntax.OperatorToken.Kind, boundOperand.Type);
           
            if (boundOperationKind == null)
            {
                diagnostics.Add(
                    $"ERROR: unary operator '{syntax.OperatorToken.Text}' is not defined for type <{boundOperand.Type}>");
                return boundOperand;
            }
            
            return new BoundUnaryExpression(boundOperationKind.Value, boundOperand);
        }

        BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            BoundExpression boundLeft = BindExpression(syntax.Left);
            BoundExpression boundRight = BindExpression(syntax.Right);
            BoundBinaryOperatorKind? boundOperationKind =
                BindBinaryOperatorKind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
            
            if (boundOperationKind == null)
            {
                diagnostics.Add(
                    $"ERROR: binary operator '{syntax.OperatorToken.Text}' is not defined for types <{boundLeft.Type}> and <{boundRight.Type}>");
                return boundLeft;
            }
            
            return new BoundBinaryExpression(boundLeft, boundOperationKind.Value, boundRight);
        }
        
        BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
        {
            if (operandType != typeof(int)) { return null; }
            
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return BoundUnaryOperatorKind.Identity;
                case SyntaxKind.MinusToken:
                    return BoundUnaryOperatorKind.Negation;
                default:
                    throw new Exception($"Unexpected unary operator: <{kind}>");
            }
        }

        BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        {
            if (leftType != typeof(int) || rightType != typeof(int)) { return null; }
            
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                    return BoundBinaryOperatorKind.Addition;
                case SyntaxKind.MinusToken:
                    return BoundBinaryOperatorKind.Subtraction;
                case SyntaxKind.StarToken:
                    return BoundBinaryOperatorKind.Multiplication;
                case SyntaxKind.SlashToken:
                    return BoundBinaryOperatorKind.Division;
                default:
                    throw new Exception($"Unexpected binary operator: <{kind}>");
            }
        }
    }
}