﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using Silverback.Messaging.Encryption;
using Silverback.Messaging.Inbound.ErrorHandling;
using Silverback.Messaging.Inbound.ExactlyOnce;
using Silverback.Messaging.Messages;
using Silverback.Messaging.Sequences.Batch;
using Silverback.Messaging.Serialization;

namespace Silverback.Messaging.Configuration
{
    /// <summary>
    ///     Builds the <see cref="ConsumerEndpoint" />.
    /// </summary>
    /// <typeparam name="TBuilder">
    ///     The actual builder type.
    /// </typeparam>
    public interface IConsumerEndpointBuilder<out TBuilder> : IEndpointBuilder<TBuilder>
        where TBuilder : IConsumerEndpointBuilder<TBuilder>
    {
        /// <summary>
        ///     Gets the type of the message being consumed.
        /// </summary>
        /// <remarks>
        ///     This value might be used during the configuration to automatically determine some configurations (e.g. the
        ///     correct deserializer to be used) without having to specify the message type once again.
        /// </remarks>
        Type? MessageType { get; }

        /// <summary>
        ///     Specifies the <see cref="IMessageSerializer" /> to be used to deserialize the messages.
        /// </summary>
        /// <param name="serializer">
        ///     The <see cref="IMessageSerializer" />.
        /// </param>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder DeserializeUsing(IMessageSerializer serializer);

        /// <summary>
        ///     Specifies the <see cref="EncryptionSettings" /> to be used to decrypt the messages.
        /// </summary>
        /// <param name="encryptionSettings">
        ///     The <see cref="EncryptionSettings" />.
        /// </param>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder Decrypt(EncryptionSettings encryptionSettings);

        /// <summary>
        ///     Specifies the error policy to be applied when an exception occurs during the processing of the
        ///     consumed messages.
        /// </summary>
        /// <param name="errorPolicy">
        ///     The <see cref="IErrorPolicy" />.
        /// </param>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder OnError(IErrorPolicy errorPolicy);

        /// <summary>
        ///     Specifies the error policy to be applied when an exception occurs during the processing of the
        ///     consumed messages.
        /// </summary>
        /// <param name="errorPolicyBuilderAction">
        ///     An <see cref="Action{T}" /> that takes the <see cref="IErrorPolicyBuilder" /> and configures it.
        /// </param>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder OnError(Action<IErrorPolicyBuilder> errorPolicyBuilderAction);

        /// <summary>
        ///     Specifies the strategy to be used to ensure that each message is processed exactly once.
        /// </summary>
        /// <param name="strategy">
        ///     The <see cref="IExactlyOnceStrategy" />.
        /// </param>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder EnsureExactlyOnce(IExactlyOnceStrategy strategy);

        /// <summary>
        ///     Specifies the strategy to be used to ensure that each message is processed exactly once.
        /// </summary>
        /// <param name="strategyBuilderAction">
        ///     An <see cref="Action{T}" /> that takes the <see cref="IExactlyOnceStrategyBuilder" /> and configures
        ///     it.
        /// </param>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder EnsureExactlyOnce(Action<IExactlyOnceStrategyBuilder> strategyBuilderAction);

        /// <summary>
        ///     Enables batch processing.
        /// </summary>
        /// <param name="batchSize">
        ///     The number of messages to be processed in batch.
        /// </param>
        /// <param name="maxWaitTime">
        ///     The maximum amount of time to wait for the batch to be filled. After this time the batch will be
        ///     completed even if the specified <c>batchSize</c> is not reached.
        /// </param>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder EnableBatchProcessing(int batchSize, TimeSpan? maxWaitTime = null);

        /// <summary>
        ///     Sets the timeout after which an incomplete sequence that isn't pushed with new messages will
        ///     be aborted and discarded. The default is a conservative 30 minutes.
        /// </summary>
        /// <remarks>
        ///     This setting is ignored for batches (<see cref="BatchSequence" />), use the
        ///     <c>maxWaitTime</c> parameter of
        ///     <see cref="IConsumerEndpointBuilder{TBuilder}.EnableBatchProcessing" /> instead.
        /// </remarks>
        /// <param name="timeout">
        ///     The timeout.
        /// </param>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder WithSequenceTimeout(TimeSpan timeout);

        /// <summary>
        ///     Specifies that an exception must be thrown if no subscriber is handling the received message. This
        ///     option is enabled by default. Use the
        ///     <see cref="IConsumerEndpointBuilder{TBuilder}.IgnoreUnhandledMessages" /> to disable it.
        /// </summary>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder ThrowIfUnhandled();

        /// <summary>
        ///     Specifies that the message has to be silently ignored if no subscriber is handling it.
        /// </summary>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder IgnoreUnhandledMessages();

        /// <summary>
        ///     Specifies that the null messages have to be mapped to a <see cref="Tombstone{TMessage}" /> (
        ///     <see cref="NullMessageHandlingStrategy.Tombstone" />). This is the default behavior, use the
        ///     <see cref="UseLegacyNullMessageHandling" /> or <see cref="SkipNullMessages" /> methods to change it.
        /// </summary>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder HandleTombstoneMessages();

        /// <summary>
        ///     Specifies that the null messages have to be silently skipped (
        ///     <see cref="NullMessageHandlingStrategy.Skip" />).
        /// </summary>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder SkipNullMessages();

        /// <summary>
        ///     Specifies that the null messages have to be forwarded as <c>null</c> (
        ///     <see cref="NullMessageHandlingStrategy.Legacy" />).
        /// </summary>
        /// <returns>
        ///     The endpoint builder so that additional calls can be chained.
        /// </returns>
        TBuilder UseLegacyNullMessageHandling();
    }
}
