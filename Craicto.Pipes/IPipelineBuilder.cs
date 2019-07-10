namespace Craicto.Pipes
{
    public interface IPipelineBuilder<T>
    {
        IPipelineBuilder<T> Pipe(Pipe<T> pipe);
        Handler<T> Handle(Handler<T> handler);
    }
}