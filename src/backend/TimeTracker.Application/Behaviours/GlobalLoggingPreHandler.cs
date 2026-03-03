using LiteBus.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTracker.Application.Behaviours;

public sealed class GlobalLoggingPreHandler<T> : IAsyncMessagePreHandler<T> where T : class
{
    private readonly ILogger<GlobalLoggingPreHandler<T>> _logger;

    public GlobalLoggingPreHandler(ILogger<GlobalLoggingPreHandler<T>> logger)
    {
        _logger = logger;
    }

    public Task PreHandleAsync(T message, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Processing message of type {MessageType}", message.GetType().FullName);
        return Task.CompletedTask;
    }
}