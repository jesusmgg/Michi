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
        
        IdentifierToken,
        
        // Keywords
        FalseKeyword,
        TrueKeyword,
        
        // Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}