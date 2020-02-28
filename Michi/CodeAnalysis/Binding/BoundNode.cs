namespace Michi.CodeAnalysis.Binding
{
    abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }
    }
}