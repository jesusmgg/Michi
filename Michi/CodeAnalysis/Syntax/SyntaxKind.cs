namespace Michi.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        BadToken,
        WhitespaceToken,
        EndOfFileToken,
        
        NumberToken,
        
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        
        OpenParenthesisToken,
        CloseParenthesisToken,
        
        // Expressions
        LiteralExpression,
        BinaryExpression,
        UnaryExpression,
        ParenthesizedExpression,
    }
}