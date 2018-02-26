# Bot hierarchy

## Goal
The goal is to design a bot, that can paralelly asynchronously find the
best move for the next situation and can cancel the evaluation on users
demand at any time and return the best move.

## Approach
<img src="bot.jpg" alt="Bot design" style="width: 60%; float: right"/>

1. Can stop evaluation - the bot implements method *StopEvaluation()*
2. Can return best move at any given time - bot is online-bot, it will periodically
update its best move and property *CurrentBestMove* will return this move.
3. Can asynchronously and paralelly find best move - *FindBestMoveAsync()*

This design is flexible enough to fill up requirements. However, the problem is
to implement those features without hindering the evaluation speed.

## Details
### Stop evaluation
Bot has to know if the stop condition has been fulfilled. Therefore there must be
stored this information. But how?

1. Boolean field *IsStopped* reporting whether the evaluation should stop
(and then reset it to continue evaluation again)
    - *problems*: situation when user wants to stop evaluation and soon after
    continue again, could end up ignoring the stop condition.
2. Field of type *CancellationTokenSource* and passing tokens from this source of type
*CancellationToken* in the method parameters - this solution would solve
fast stop/continue commands, because tokens relate to the source and if the source
is cancelled, it cannot be "uncancelled". Thus for continuing the evaluation
bot would create new *CancellationTokenSource* instance and get new tokens from this source.,
    - *problems*: passing tokens in parameter will slow the evaluation

Solution that was chosen was to have enum with 3 states:
```csharp
    /// <summary>
    /// Evaluation state of the bot.
    /// </summary>
    public enum BotEvaluationState
    {
        /// <summary>
        /// Bot is not running at the moment.
        /// </summary>
        NotRunning,
        /// <summary>
        /// Bot should stop.
        /// </summary>
        ShouldStop,
        /// <summary>
        /// Bot is running at the moment.
        /// </summary>
        Running
    }
```

This will ensure that new evaluation won't be started unless
the bot is in *NotRunning* state. When evaluation starts,
bot will check for *NotRunning* state, otherwise throw an exception, then
it will set *Running* on.

We don't want to throw exception and catch it every time someone
tries to start a new evaluation process when the cancellation
of the previous one was not handled.
Therefore *IBot* interface will have property *CanStartEvaluation*.
