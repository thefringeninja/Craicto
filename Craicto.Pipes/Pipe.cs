namespace Craicto.Pipes
{
    public delegate Handler<T> Pipe<T>(Handler<T> next);
}