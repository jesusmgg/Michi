namespace Michi.CodeAnalysis
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