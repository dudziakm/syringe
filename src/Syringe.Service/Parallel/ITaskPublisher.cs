using System;
using Syringe.Core.Results;

namespace Syringe.Service.Parallel
{
    public interface ITaskPublisher
    {
        void Start(int taskId, IObservable<TestCaseResult> resultSource);
    }
}