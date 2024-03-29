﻿using NetMQ;
using NetMQ.Sockets;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;

using TSource = System.String;
using TResult = System.String;

namespace Bonsai.BonsaiMQ
{
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Sends the input string to the defined ZeroMQ server, and returns response as a string.")]

    public class SendMessage : Transform<TSource, TResult>
    {
        [Description("The address of the ZeroMQ server.")]
        public string Address { get; set; }

        [Description("The port listened by the ZeroMQ server.")]
        public string Port { get; set; }

        public override IObservable<TResult> Process(IObservable<TSource> source)
        {
            return source.Select(input =>
            {
                using (var client = new RequestSocket(string.Format(">tcp://{0}:{1}", Address, Port)))
                {
                    client.SendFrame(input);
                    var msg = client.ReceiveFrameString();
                    return msg;
                };
            });
        }
    }
}