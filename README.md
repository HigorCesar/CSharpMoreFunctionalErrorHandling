# CSharpMoreFunctionalErrorHandling
Presentation given at [LeasePlan digital](https://www.leaseplan.com/corporate/careers/leaseplan-digital) about alternatives to error handling in C# following a more functional way.


### Problem
There is no built-in mechanism to represent the possibility  of failures or errors in a typed manner in C#.
Usually C# Developers rely on exceptions and NULL to represent non successful scenarios, the problem with this approach is
that error handling is not clear.
This problem can be addressed by the usage of code docs but I would rather have it strongly typed so developers can't just skip the error check when it is required.


### Common approaches to handle error

#### Naive Approach
There is no indication that operation can fail neither how to handle any error.

Problems
* It can throw NullExceptions
* There is no indication it can fails so the client doesn't know if exceptions need to be catched
* It is not clear if null checks are required

```CSharp
public class NaiveRepository
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly string _sqsUrl;

    public NaiveRepository(IAmazonSQS amazonSqs, string sqsUrl)
    {
        _amazonSqs = amazonSqs;
        _sqsUrl = sqsUrl;
    }

    public async Task<IEnumerable<ImageDataPoint>> GetAllAsync(CancellationToken cancellationToken)
    {
        var response = await _amazonSqs.ReceiveMessageAsync(_sqsUrl, cancellationToken);
        return response.Messages.Select(x => JsonConvert.DeserializeObject<ImageDataPoint>(x.Body));
    }
}
```
### The Null approach or the "it never happened approach""
This approach makes use of null as return to failure scenarios but there is no details of the failure and sometimes null is a valid result so the failure is mixed with the absence of data

Problems
* There is no indication it can fails so the client doesn't know if exceptions need to be catched
* No details about the error/exception

```Csharp
public class ItNeverHappenedRepository
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly string _sqsUrl;

    public ItNeverHappenedRepository(IAmazonSQS amazonSqs, string sqsUrl)
    {
        _amazonSqs = amazonSqs;
        _sqsUrl = sqsUrl;
    }

    public async Task<IEnumerable<ImageDataPoint>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _amazonSqs.ReceiveMessageAsync(_sqsUrl, cancellationToken);
            if (response.HttpStatusCode != HttpStatusCode.OK)
                return null;

            return response.Messages.Select(x => JsonConvert.DeserializeObject<ImageDataPoint>(x.Body));
        }
        catch (OverLimitException e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}
```

### One More functional solution
Several packages provide typed solutions inspired in the functional approach to type operations results, below you can find my favorite solution using the [Language-ext](https://github.com/louthy/language-ext)

```Csharp
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
```
#### How it works
The operation result is represented(wrapped) in the Either type.
The Left part represents the type which wrappes the error scenario, it could be just a simple error message or as advanced as you may need, in this example I've created an Error class that can store the exception details, error code and more details.
The Right part contains the actual result.

### How to consume this function
The client of this function now must handle the possibility of error.
```Csharp
var either = await resultRepository.GetAllAsyncEitherBased(CancellationToken.None);
either
    .Right(values => Console.WriteLine(values.Count()))
    .Left(error => Console.WriteLine(error.Message));
```

### Benefits of this approach
* Strongly typed operations
* Clearly identify operations that can fail
* No need to inject Loggers everywhere just to catch error messages


