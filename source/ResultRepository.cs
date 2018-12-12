using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using CSharpFunctionalExtensions;
using LanguageExt;
using Newtonsoft.Json;
using static LanguageExt.Prelude;

namespace HandlingOperationResult
{
    public class ResultRepository
    {
        private readonly IAmazonSQS _amazonSqs;
        private readonly string _sqsUrl;

        public ResultRepository(IAmazonSQS amazonSqs, string sqsUrl)
        {
            _amazonSqs = amazonSqs;
            _sqsUrl = sqsUrl;
        }

        public async Task<CSharpFunctionalExtensions.Result<IEnumerable<ImageDataPoint>>> GetAllAsyncResultBased(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _amazonSqs.ReceiveMessageAsync(_sqsUrl, cancellationToken);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                    return Result.Fail<IEnumerable<ImageDataPoint>>("something happened");

                var imageDataPoints = response.Messages.Select(x => JsonConvert.DeserializeObject<ImageDataPoint>(x.Body));
                return Result.Ok(imageDataPoints);
            }
            catch (OverLimitException)
            {
                return Result.Fail<IEnumerable<ImageDataPoint>>("something happened");
            }
        }

public async Task<Either<Error, IEnumerable<ImageDataPoint>>> GetAllAsyncEitherBased(CancellationToken cancellationToken)
{
    try
    {
        var response = await _amazonSqs.ReceiveMessageAsync(_sqsUrl, cancellationToken);
        if (response.HttpStatusCode != HttpStatusCode.OK)
            return Left<Error, IEnumerable<ImageDataPoint>>(new Error(ErrorCode.General, "Something happened"));

        var imageDataPoints = response.Messages.Select(x => JsonConvert.DeserializeObject<ImageDataPoint>(x.Body));
        return Right<Error, IEnumerable<ImageDataPoint>>(imageDataPoints);
    }
    catch (OverLimitException e)
    {
        return Left<Error, IEnumerable<ImageDataPoint>>(new Error(ErrorCode.FailedToCommunicateToAws, exception: e));
    }
}
    }
}