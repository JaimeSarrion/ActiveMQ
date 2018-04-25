using Apache.NMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Oficina1
{
    class Program
    {
        public static string brokerUri = $"activemq:tcp://localhost:61616";

        public static string topicOfi1Temp = "TempOfi1";
        public static string topicOfi1Lum = "LumOfi1";
        public static string serviceOfi1Lum = "serviceLumOfi1";
        public static string serviceOfi1Temp = "serviceTempOfi1";

        public static int opServieTemp = 0;
        public static int opServieLum = 0;

        public static string temperatura = "";
        public static string luminosidad = "";

        static void Main(string[] args)
        {
            Random tempR = new Random();
            Random lumR = new Random();
            Random foo = new Random();

            NMSConnectionFactory factory = new NMSConnectionFactory(brokerUri);
            IConnection connection = factory.CreateConnection();
            {
                connection.Start();
               
                ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
                //producers
                //temperatura
                IDestination destOfi1Temp = session.GetTopic(topicOfi1Temp);

                IMessageProducer producerOfi1Temp = session.CreateProducer(destOfi1Temp);
                //luminosidad
                IDestination destOfi1Lum = session.GetTopic(topicOfi1Lum);
                IMessageProducer producerOfi1Lum = session.CreateProducer(destOfi1Lum);
                //consumidores
                //temperatura
                IDestination servOfi1Temp = session.GetTopic(serviceOfi1Temp);
                IMessageConsumer consumerOfi1Temp = session.CreateConsumer(servOfi1Temp);
                //luminosidad
                IDestination servOfi1Lum = session.GetTopic(serviceOfi1Lum);
                IMessageConsumer consumerOfi1Lum = session.CreateConsumer(servOfi1Lum);
                
                while (true)
                {
                    if(opServieTemp == 0)
                    {
                        temperatura = tempR.Next(0, 50).ToString();
                    }else if (opServieTemp == -1)
                    {
                        temperatura = (Int32.Parse(temperatura) - foo.Next(1,4)).ToString();
                    }
                    else
                    {
                        temperatura = (Int32.Parse(temperatura) + foo.Next(1, 4)).ToString();
                    }

                    if (opServieLum == 0)
                    {
                        luminosidad = lumR.Next(200, 1000).ToString();
                    }
                    else if (opServieLum == -1)
                    {
                        luminosidad = (Int32.Parse(luminosidad) - foo.Next(10, 50)).ToString();
                    }
                    else
                    {
                        luminosidad = (Int32.Parse(luminosidad) + foo.Next(10, 50)).ToString();
                    }


                    
                    //temperatura
                    producerOfi1Temp.DeliveryMode = MsgDeliveryMode.NonPersistent;

                    producerOfi1Temp.Send(session.CreateTextMessage(temperatura));
                    //luminosidad
                    producerOfi1Lum.DeliveryMode = MsgDeliveryMode.NonPersistent;

                    producerOfi1Lum.Send(session.CreateTextMessage(luminosidad));


                    IMessage msg = consumerOfi1Temp.Receive();
                    IMessage msg2 = consumerOfi1Lum.Receive();
                    if (msg is ITextMessage && msg2 is ITextMessage)
                    {
                        ITextMessage txtMsg = msg as ITextMessage;
                        ITextMessage txtMsg2 = msg2 as ITextMessage;
                        opServieTemp = Int32.Parse(txtMsg.Text);
                        opServieLum = Int32.Parse(txtMsg2.Text);
                    }
                    else
                    {
                        Console.WriteLine("Unexpected message type: " + msg.GetType().Name);
                    }

                    Console.WriteLine("T: " + temperatura + " S: " + opServieTemp + "| L: " + luminosidad + " S: " + opServieLum);
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
