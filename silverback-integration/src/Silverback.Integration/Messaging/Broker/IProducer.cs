﻿// Copyright (c) 2018-2019 Sergio Aquilini
// This code is licensed under MIT license (see LICENSE file for details)

using System.Threading.Tasks;

namespace Silverback.Messaging.Broker
{
    public interface IProducer
    {
        void Produce(object message);

        Task ProduceAsync(object message);
    }
}