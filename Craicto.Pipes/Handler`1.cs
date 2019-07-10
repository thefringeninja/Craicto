using System;
using System.Threading;
using System.Threading.Tasks;

namespace Craicto.Pipes
{
    public delegate Task Handler<in T>(T message, CancellationToken ct);
}