using System;

public readonly struct MessageData
{
    public readonly string Sender;
    public readonly string Content;
    public readonly DateTime Timestamp;

    public MessageData(string sender, string content, DateTime timestamp)
    {
        Sender = sender;
        Content = content;
        Timestamp = timestamp;
    }
}