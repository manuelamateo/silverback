﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System;
using System.Collections.Generic;
using Silverback.Messaging.Outbound.Routing;
using Silverback.Util;

namespace Silverback.Messaging.Configuration.Mqtt
{
    internal sealed class MqttEndpointsConfigurationBuilder : IMqttEndpointsConfigurationBuilder
    {
        private readonly IEndpointsConfigurationBuilder _endpointsConfigurationBuilder;

        public MqttEndpointsConfigurationBuilder(IEndpointsConfigurationBuilder endpointsConfigurationBuilder)
        {
            _endpointsConfigurationBuilder = endpointsConfigurationBuilder;
        }

        public IServiceProvider ServiceProvider => _endpointsConfigurationBuilder.ServiceProvider;

        internal MqttClientConfig ClientConfig { get; private set; } = new();

        public IMqttEndpointsConfigurationBuilder Configure(Action<MqttClientConfig> configAction)
        {
            Check.NotNull(configAction, nameof(configAction));

            configAction.Invoke(ClientConfig);

            return this;
        }

        public IMqttEndpointsConfigurationBuilder Configure(
            Action<IMqttClientConfigBuilder> configBuilderAction)
        {
            Check.NotNull(configBuilderAction, nameof(configBuilderAction));

            var configBuilder = new MqttClientConfigBuilder(ServiceProvider);
            configBuilderAction.Invoke(configBuilder);

            ClientConfig = configBuilder.Build();

            return this;
        }

        public IMqttEndpointsConfigurationBuilder AddOutbound<TMessage>(
            Action<IMqttProducerEndpointBuilder> endpointBuilderAction,
            bool preloadProducers = true) =>
            AddOutbound(typeof(TMessage), endpointBuilderAction, preloadProducers);

        public IMqttEndpointsConfigurationBuilder AddOutbound(
            Type messageType,
            MqttOutboundEndpointRouter<object>.RouterFunction routerFunction,
            IReadOnlyDictionary<string, Action<IMqttProducerEndpointBuilder>> endpointBuilderActions,
            bool preloadProducers = true)
        {
            var router = new MqttOutboundEndpointRouter<object>(
                routerFunction,
                endpointBuilderActions,
                ClientConfig);
            this.AddOutbound(messageType, router, preloadProducers);

            return this;
        }

        public IMqttEndpointsConfigurationBuilder AddOutbound<TMessage>(
            MqttOutboundEndpointRouter<TMessage>.RouterFunction routerFunction,
            IReadOnlyDictionary<string, Action<IMqttProducerEndpointBuilder>> endpointBuilderActions,
            bool preloadProducers = true)
        {
            var router = new MqttOutboundEndpointRouter<TMessage>(
                routerFunction,
                endpointBuilderActions,
                ClientConfig);
            this.AddOutbound(router, preloadProducers);

            return this;
        }

        public IMqttEndpointsConfigurationBuilder AddOutbound(
            Type messageType,
            MqttOutboundEndpointRouter<object>.SingleEndpointRouterFunction routerFunction,
            IReadOnlyDictionary<string, Action<IMqttProducerEndpointBuilder>> endpointBuilderActions,
            bool preloadProducers = true)
        {
            var router = new MqttOutboundEndpointRouter<object>(
                routerFunction,
                endpointBuilderActions,
                ClientConfig);
            this.AddOutbound(messageType, router, preloadProducers);

            return this;
        }

        public IMqttEndpointsConfigurationBuilder AddOutbound<TMessage>(
            MqttOutboundEndpointRouter<TMessage>.SingleEndpointRouterFunction routerFunction,
            IReadOnlyDictionary<string, Action<IMqttProducerEndpointBuilder>> endpointBuilderActions,
            bool preloadProducers = true)
        {
            var router = new MqttOutboundEndpointRouter<TMessage>(
                routerFunction,
                endpointBuilderActions,
                ClientConfig);
            this.AddOutbound(router, preloadProducers);

            return this;
        }

        public IMqttEndpointsConfigurationBuilder AddOutbound(
            Type messageType,
            Action<IMqttProducerEndpointBuilder> endpointBuilderAction,
            bool preloadProducers = true)
        {
            Check.NotNull(messageType, nameof(messageType));
            Check.NotNull(endpointBuilderAction, nameof(endpointBuilderAction));

            var builder = new MqttProducerEndpointBuilder(ClientConfig, this);
            endpointBuilderAction.Invoke(builder);

            _endpointsConfigurationBuilder.AddOutbound(messageType, builder.Build(), preloadProducers);

            return this;
        }

        public IMqttEndpointsConfigurationBuilder AddInbound(
            Action<IMqttConsumerEndpointBuilder> endpointBuilderAction) =>
            AddInbound(null, endpointBuilderAction);

        public IMqttEndpointsConfigurationBuilder AddInbound<TMessage>(
            Action<IMqttConsumerEndpointBuilder> endpointBuilderAction) =>
            AddInbound(typeof(TMessage), endpointBuilderAction);

        public IMqttEndpointsConfigurationBuilder AddInbound(
            Type? messageType,
            Action<IMqttConsumerEndpointBuilder> endpointBuilderAction)
        {
            Check.NotNull(endpointBuilderAction, nameof(endpointBuilderAction));

            var builder = new MqttConsumerEndpointBuilder(ClientConfig, messageType, this);
            builder.DeserializeJson();

            endpointBuilderAction.Invoke(builder);

            _endpointsConfigurationBuilder.AddInbound(builder.Build());

            return this;
        }
    }
}
