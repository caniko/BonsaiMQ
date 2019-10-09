using NetMQ;
using NetMQ.Sockets;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Bonsai.BonsaiMQ
{
    [DefaultProperty("Index")]
    [WorkflowElementCategory(ElementCategory.Source)]
    [Description("Receives string from the defined ZeroMQ receiver, and outputs a timestamp string.")]

    public class ReceiveMessage : Source<string>
    {
        [Description("The address of the ZeroMQ client.")]
        public string Address { get; set; }

        [Description("The listening port.")]
        public string Port { get; set; }

        public override IObservable<string> Generate()
        {
            return Observable.Create<string>((observer, cancellationToken) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    using (var receiver = new PullSocket(string.Format("@tcp://{0}:{1}", Address, Port)))
                    {
                        observer.OnNext(receiver.ReceiveFrameString());
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            string msg = receiver.ReceiveFrameString();
                            if (msg != null)
                            {
                                observer.OnNext(msg);
                                Console.WriteLine("From Client: {0}", msg);
                            }
                        }
                    }
                });
            })
            .PublishReconnectable()
            .RefCount();
        }
    }
}