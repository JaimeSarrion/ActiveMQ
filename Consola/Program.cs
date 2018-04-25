using Apache.NMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consola
{
    class Program
    {

        public static string brokerUri = $"activemq:tcp://localhost:61616";

        public static string temperaturaMinOficina1 = "10";
        public static string temperaturaMinOficina2 = "10";
        public static string temperaturaMaxOficina1 = "30";
        public static string temperaturaMaxOficina2 = "30";

        public static string luminosidadMinOficina1 = "400";
        public static string luminosidadMinOficina2 = "400";
        public static string luminosidadMaxOficina1 = "800";
        public static string luminosidadMaxOficina2 = "800";


        public static string tempOfi1 = "";
        public static string lumOfi1 = "";
        public static string tempOfi2 = "";
        public static string lumOfi2 = "";

        public static string topicOfi1Temp = "TempOfi1";
        public static string topicOfi1Lum = "LumOfi1";
        public static string serviceOfi1Lum = "serviceLumOfi1";
        public static string serviceOfi1Temp = "serviceTempOfi1";

        public static string topicOfi2Temp = "TempOfi2";
        public static string topicOfi2Lum = "LumOfi2";
        public static string serviceOfi2Lum = "serviceLumOfi2";
        public static string serviceOfi2Temp = "serviceTempOfi2";

        static void Main(string[] args)
        {
            Console.WriteLine("Bienvenido a la consola central");
            Console.WriteLine("[0] para valores predeterminados");
            Console.WriteLine("[1] para modificar valores");
            int opcion = Int32.Parse(Console.ReadLine());

            if (opcion == 1)
            {
                ajustes();
            }

            NMSConnectionFactory factory = new NMSConnectionFactory(brokerUri);

            IConnection connection = factory.CreateConnection();
            {
                connection.Start();
                ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
                //OFICINA 1
                //CONSUMIDORES
                IDestination destOfi1Temp = session.GetTopic(topicOfi1Temp);
                IMessageConsumer consumerOfi1Temp = session.CreateConsumer(destOfi1Temp);

                IDestination destOfi1Lum = session.GetTopic(topicOfi1Lum);
                IMessageConsumer consumerOfi1Lum = session.CreateConsumer(destOfi1Lum);

                //PRODUCTORES
                IDestination servOfi1Temp = session.GetTopic(serviceOfi1Temp);
                IMessageProducer producerOfi1Temp = session.CreateProducer(servOfi1Temp);

                IDestination servOfi1Lum = session.GetTopic(serviceOfi1Lum);
                IMessageProducer producerOfi1Lum = session.CreateProducer(servOfi1Lum);
                //OFICINA 2
                //CONSUMIDORES
                IDestination destOfi2Temp = session.GetTopic(topicOfi2Temp);
                IMessageConsumer consumerOfi2Temp = session.CreateConsumer(destOfi2Temp);

                IDestination destOfi2Lum = session.GetTopic(topicOfi2Lum);
                IMessageConsumer consumerOfi2Lum = session.CreateConsumer(destOfi2Lum);

                //PRODUCTORES
                IDestination servOfi2Temp = session.GetTopic(serviceOfi2Temp);
                IMessageProducer producerOfi2Temp = session.CreateProducer(servOfi2Temp);

                IDestination servOfi2Lum = session.GetTopic(serviceOfi2Lum);
                IMessageProducer producerOfi2Lum = session.CreateProducer(servOfi2Lum);

                //INICIALIZACION
                //OFICINA 1
                producerOfi1Temp.DeliveryMode = MsgDeliveryMode.NonPersistent;
                producerOfi1Temp.Send(session.CreateTextMessage("0"));
                producerOfi1Lum.DeliveryMode = MsgDeliveryMode.NonPersistent;
                producerOfi1Lum.Send(session.CreateTextMessage("0"));
                //OFICINA 2
                producerOfi2Temp.DeliveryMode = MsgDeliveryMode.NonPersistent;
                producerOfi2Temp.Send(session.CreateTextMessage("0"));
                producerOfi2Lum.DeliveryMode = MsgDeliveryMode.NonPersistent;
                producerOfi2Lum.Send(session.CreateTextMessage("0"));


                while (true){
                    IMessage msg = consumerOfi1Temp.Receive();
                    IMessage msg2 = consumerOfi1Lum.Receive();
                    IMessage msg3 = consumerOfi2Temp.Receive();
                    IMessage msg4 = consumerOfi2Lum.Receive();
                    if (msg is ITextMessage && msg2 is ITextMessage && msg3 is ITextMessage && msg4 is ITextMessage)
                    {
                        ITextMessage txtMsg = msg as ITextMessage;
                        ITextMessage txtMsg2 = msg2 as ITextMessage;
                        ITextMessage txtMsg3 = msg3 as ITextMessage;
                        ITextMessage txtMsg4 = msg4 as ITextMessage;
                        tempOfi1 = txtMsg.Text;
                        lumOfi1 = txtMsg2.Text;
                        tempOfi2 = txtMsg3.Text;
                        lumOfi2 = txtMsg4.Text;

                    }
                    else
                    {
                        Console.WriteLine("Unexpected message type: " + msg.GetType().Name);
                    }
                    //OFICINA 1
                    producerOfi1Temp.DeliveryMode = MsgDeliveryMode.NonPersistent;
                    string vt = validaTemperatura1();
                    producerOfi1Temp.Send(session.CreateTextMessage(vt));


                    producerOfi1Lum.DeliveryMode = MsgDeliveryMode.NonPersistent;
                    string vl = validaLuminosidad1();
                    producerOfi1Lum.Send(session.CreateTextMessage(vl));
                    //OFICINA 2
                    producerOfi2Temp.DeliveryMode = MsgDeliveryMode.NonPersistent;
                    string vt2 = validaTemperatura2();
                    producerOfi2Temp.Send(session.CreateTextMessage(vt2));


                    producerOfi2Lum.DeliveryMode = MsgDeliveryMode.NonPersistent;
                    string vl2 = validaLuminosidad2();
                    producerOfi2Lum.Send(session.CreateTextMessage(vl2));

                    Console.WriteLine(" Oficina 1-> T: " + tempOfi1+" S: "+vt + "| L: " + lumOfi1+ " S: " + vl);
                    Console.WriteLine(" Oficina 2-> T: " + tempOfi2 + " S: " + vt2 + "| L: " + lumOfi2 + " S: " + vl2);
                    Console.WriteLine("------------------------------------------------------------");
                }
            }
        }

        private static string validaTemperatura1()
        {
            if (Int32.Parse(tempOfi1) > Int32.Parse(temperaturaMaxOficina1))//temperatura por encima del limite
            {
                return "-1";
            }else if(Int32.Parse(tempOfi1) < Int32.Parse(temperaturaMinOficina1))
            {
                return "1";
            }
            return "0";
        }
        private static string validaTemperatura2()
        {
            if (Int32.Parse(tempOfi2) > Int32.Parse(temperaturaMaxOficina2))//temperatura por encima del limite
            {
                return "-1";
            }
            else if (Int32.Parse(tempOfi2) < Int32.Parse(temperaturaMinOficina2))
            {
                return "1";
            }
            return "0";
        }
        private static string validaLuminosidad1()
        {
            if (Int32.Parse(lumOfi1) > Int32.Parse(luminosidadMaxOficina1))//temperatura por encima del limite
            {
                return "-1";
            }
            else if (Int32.Parse(lumOfi1) < Int32.Parse(luminosidadMinOficina1))
            {
                return "1";
            }
            return "0";
        }
        private static string validaLuminosidad2()
        {
            if (Int32.Parse(lumOfi2) > Int32.Parse(luminosidadMaxOficina2))//temperatura por encima del limite
            {
                return "-1";
            }
            else if (Int32.Parse(lumOfi2) < Int32.Parse(luminosidadMinOficina2))
            {
                return "1";
            }
            return "0";
        }
        private static void ajustes()
        {
            Console.WriteLine("Temperatura máxima para la oficina 1 [0-50]:");
            string temperaturaMaxOficina1 = Console.ReadLine();
            Console.WriteLine("Temperatura mínima para la oficina 1 [0-50]:");
            string temperaturaMinOficina1 = Console.ReadLine();
            Console.WriteLine("Temperatura máxima para la oficina 2 [0-50]:");
            string temperaturaMaxOficina2 = Console.ReadLine();
            Console.WriteLine("Temperatura mínima para la oficina 2 [0-50]:");
            string temperaturaMinOficina2 = Console.ReadLine();

            Console.WriteLine("Luminosidad máxima para la oficina 1 [200-1000]:");
            string luminosidadMaxOficina1 = Console.ReadLine();
            Console.WriteLine("Luminosidad mínima para la oficina 1 [200-1000]:");
            string luminosidadMinOficina1 = Console.ReadLine();
            Console.WriteLine("Luminosidad máxima para la oficina 2 [200-1000]:");
            string luminosidadMaxOficina2 = Console.ReadLine();
            Console.WriteLine("Luminosidad mínima para la oficina 2 [200-1000]:");
            string luminosidadMinOficina2 = Console.ReadLine();
        }
    }
}
