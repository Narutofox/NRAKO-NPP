using PostSharp.Aspects;
using PostSharp.Extensibility;
using System;
using Metrics;
using PostSharp.Serialization;

namespace NRAKO_IvanCicek.AOP
{
    [PSerializable]
    [MulticastAttributeUsage(MulticastTargets.Method)]
    public class RequestMetricsAspect : OnMethodBoundaryAspect
    {
         [PNonSerialized] private readonly Timer _timer = Metric.Timer("Requests", Unit.Requests);
         [PNonSerialized] private readonly Counter _counter = Metric.Counter("ConcurrentRequests", Unit.Requests);
         [PNonSerialized] private readonly Meter _errors = Metric.Meter("Errors", Unit.Requests, TimeUnit.Seconds);
         [PNonSerialized]  private TimerContext _context;

        //static RequestMetricsAspect()
        //{
            //_timer = Metric.Timer("Requests", Unit.Requests);
            //_counter = Metric.Counter("ConcurrentRequests", Unit.Requests);
            //_errors Metric.Meter("Errors", Unit.Requests, TimeUnit.Seconds);
        //}

        //public RequestMetricsAspect()
        //{
        //    _timer = Metric.Timer("Requests", Unit.Requests);
        //    _counter = Metric.Counter("ConcurrentRequests", Unit.Requests);
        //    _errors = Metric.Meter("Errors", Unit.Requests, TimeUnit.Seconds);
        //}

        public override void OnEntry(MethodExecutionArgs args)
        {
            _counter.Increment();
            _context = _timer.NewContext(args.Method.Name);
            base.OnEntry(args);
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            _counter.Decrement();
            _context.Dispose();
            base.OnSuccess(args);
        }

        public override void OnException(MethodExecutionArgs args)
        {
            _errors.Mark(args.Method.Name);
            base.OnException(args);
        }
    }
}