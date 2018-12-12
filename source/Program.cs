using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using CSharpFunctionalExtensions;
using LanguageExt.ClassInstances.Const;

namespace HandlingOperationResult
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        async Task ResultRepositoryUsage()
        {
            var resultRepository = new ResultRepository(new AmazonSQSClient(), "http://sqsurl.test");
            var result = await resultRepository.GetAllAsyncResultBased(CancellationToken.None);
            result
                .OnSuccess(values => Console.WriteLine(values.Count()))
                .OnFailure(message => Console.WriteLine(message));

            var either = await resultRepository.GetAllAsyncEitherBased(CancellationToken.None);
            either
                .Right(values => Console.WriteLine(values.Count()))
                .Left(error => Console.WriteLine(error.Message));
        }
    }
}