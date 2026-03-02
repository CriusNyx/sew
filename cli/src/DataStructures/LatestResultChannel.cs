using System.Threading.Channels;

public class LatestResultChannel<Args, Result>
{
  object asyncLock;
  DateTime lastCallTime;
  Func<Args, Task<Result>> func;
  Channel<Result> channel;
  public ChannelReader<Result> Reader => channel.Reader;

  public LatestResultChannel(Func<Args, Task<Result>> func)
  {
    asyncLock = new object();
    lastCallTime = DateTime.Now;
    this.func = func;
    channel = Channel.CreateUnbounded<Result>();
  }

  public async Task<Result> Queue(Args args)
  {
    var callTime = DateTime.Now;
    var result = await func(args);
    ReportResult(callTime, result);
    return result;
  }

  private void ReportResult(DateTime callTime, Result result)
  {
    lock (asyncLock)
    {
      if (callTime > lastCallTime)
      {
        lastCallTime = callTime;
        channel.Writer.WriteAsync(result);
      }
    }
  }
}
