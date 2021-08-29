﻿// Copyright (c) 2020 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using FluentAssertions;
using Silverback.Messaging.Configuration;
using Silverback.Messaging.Serialization;
using Silverback.Tests.Types;
using Silverback.Tests.Types.Domain;
using Xunit;

namespace Silverback.Tests.Integration.Newtonsoft.Messaging.Configuration
{
    public class ProducerEndpointBuilderSerializeAsJsonExtensionsTests
    {
        [Fact]
        public void SerializeAsJsonUsingNewtonsoft_Default_SerializerSet()
        {
            var builder = new TestProducerEndpointBuilder();

            var endpoint = builder.SerializeAsJsonUsingNewtonsoft().Build();

            endpoint.Serializer.Should().BeOfType<NewtonsoftJsonMessageSerializer>();
            endpoint.Serializer.Should().NotBeSameAs(NewtonsoftJsonMessageSerializer.Default);
        }

        [Fact]
        public void SerializeAsJsonUsingNewtonsoft_UseFixedTypeWithGenericArgument_SerializerSet()
        {
            var builder = new TestProducerEndpointBuilder();

            var endpoint = builder
                .SerializeAsJsonUsingNewtonsoft(serializer => serializer.UseFixedType<TestEventOne>())
                .Build();

            endpoint.Serializer.Should().BeOfType<NewtonsoftJsonMessageSerializer<TestEventOne>>();
        }

        [Fact]
        public void SerializeAsJsonUsingNewtonsoft_UseFixedType_SerializerSet()
        {
            var builder = new TestProducerEndpointBuilder();

            var endpoint = builder
                .SerializeAsJsonUsingNewtonsoft(serializer => serializer.UseFixedType(typeof(TestEventOne)))
                .Build();

            endpoint.Serializer.Should().BeOfType<NewtonsoftJsonMessageSerializer<TestEventOne>>();
        }

        [Fact]
        public void SerializeAsJsonUsingNewtonsoft_Configure_SerializerAndOptionsSet()
        {
            var builder = new TestProducerEndpointBuilder();

            var endpoint = builder.SerializeAsJsonUsingNewtonsoft(
                serializer => serializer.Configure(
                    settings =>
                    {
                        settings.MaxDepth = 42;
                    })).Build();

            endpoint.Serializer.Should().BeOfType<NewtonsoftJsonMessageSerializer>();
            endpoint.Serializer.As<NewtonsoftJsonMessageSerializer>().Settings.MaxDepth.Should().Be(42);
        }

        [Fact]
        public void SerializeAsJsonUsingNewtonsoft_WithEncoding_SerializerAndOptionsSet()
        {
            var builder = new TestProducerEndpointBuilder();

            var endpoint = builder.SerializeAsJsonUsingNewtonsoft(
                serializer => serializer.WithEncoding(MessageEncoding.Unicode)).Build();

            endpoint.Serializer.Should().BeOfType<NewtonsoftJsonMessageSerializer>();
            endpoint.Serializer.As<NewtonsoftJsonMessageSerializer>().Encoding.Should()
                .Be(MessageEncoding.Unicode);
        }

        [Fact]
        public void SerializeAsJsonUsingNewtonsoft_UseFixedTypeAndConfigure_SerializerAndOptionsSet()
        {
            var builder = new TestProducerEndpointBuilder();

            var endpoint = builder.SerializeAsJsonUsingNewtonsoft(
                serializer => serializer
                    .UseFixedType<TestEventOne>()
                    .Configure(
                        settings =>
                        {
                            settings.MaxDepth = 42;
                        })).Build();

            endpoint.Serializer.Should().BeOfType<NewtonsoftJsonMessageSerializer<TestEventOne>>();
            endpoint.Serializer.As<NewtonsoftJsonMessageSerializer<TestEventOne>>().Settings.MaxDepth.Should()
                .Be(42);
        }
    }
}
