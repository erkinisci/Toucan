namespace Toucan.Enums;

/// <summary>
/// Retry Count for fault-async operation
/// </summary>
public enum RetryTimes
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,

    /// <summary>
    /// Once
    /// </summary>
    One = 1,

    /// <summary>
    /// Twist
    /// </summary>
    Two = 2,

    /// <summary>
    /// Third
    /// </summary>
    Three = 3,

    /// <summary>
    /// Fourth
    /// </summary>
    Four = 4,

    /// <summary>
    /// Fifth
    /// </summary>
    Five = 5,

    /// <summary>
    /// Forever
    /// </summary>
    Forever = int.MaxValue
}