using System;
using Syringe.Core.Tests.Results;

namespace Syringe.Service.Parallel
{
    public interface ITaskPublisher
    {
        void Start(int taskId, IObservable<TestResult> resultSource);
    }
}