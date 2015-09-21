using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MsQueueFirstLook
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"kim-msi\private$\kimqueue2";
            MessageQueue queue = null;
            if (!MessageQueue.Exists(path))
            {
                queue = MessageQueue.Create(path, true);
            }
            else
                queue = new MessageQueue(path);
            //queue.Purge();
            //return;
            //   queue.ReceiveCompleted += Queue_ReceiveCompleted;//非同步寫法BeginReceive接收Send事件
            // queue.BeginReceive();
            var messages = queue.GetAllMessages();
            for (int i = 0; i < 10; i++)
            {
                if (messages.Count() == 0)
                {
                    Message message = new Message(i.ToString());
                    queue.Send(message, MessageQueueTransactionType.Single);
                }
                else
                {
                    MessageQueueTransaction tran = new MessageQueueTransaction();
                    tran.Begin();

                    var msg = queue.Receive(tran);
                    if (msg == null)
                        break;
                    System.Messaging.XmlMessageFormatter stringFormatter;
                    stringFormatter = new System.Messaging.XmlMessageFormatter(
                       new string[] { "System.String" });
                    msg.Formatter = stringFormatter;
                    Console.WriteLine(" e.Message:" + msg.Body);
                    if (i % 2 == 0)
                        tran.Dispose();
                    else
                        tran.Commit();
                }

            }
            Console.ReadLine();
        }

        private static void Queue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            MessageQueue mq = (MessageQueue)sender;
            var message = mq.EndReceive(e.AsyncResult);
            System.Messaging.XmlMessageFormatter stringFormatter;
            stringFormatter = new System.Messaging.XmlMessageFormatter(
               new string[] { "System.String" });
            message.Formatter = stringFormatter;
            Console.WriteLine(" e.Message:" + message.Body);
            mq.BeginReceive();
        }
    }
}
