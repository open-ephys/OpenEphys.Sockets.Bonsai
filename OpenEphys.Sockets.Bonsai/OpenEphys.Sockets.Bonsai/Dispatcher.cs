using System;
using System.Reactive.Concurrency;

namespace OpenEphys.Sockets.Bonsai
{
    class Dispatcher
    {
        readonly IObserver<Message> observer;
        readonly IScheduler scheduler;

        public Dispatcher(IObserver<Message> observer, IScheduler scheduler)
        {
            this.observer = observer??throw new ArgumentNullException(nameof(observer));
            this.scheduler = scheduler??throw new ArgumentNullException(nameof(scheduler));
        }

        public void Process(byte[] data)
        {
            try { ProcessPacket(data); }
            catch (Exception e) { observer.OnError(e); }
        }
        private void ProcessPacket(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var message = new Message(data);
            observer.OnNext(message);
        }
    }
}
