# Práctica de la asignatua Metodologías y Técnicas de la Integración de Sistemas (MTIS)
En esta práctica veremos características de ActiveMQ, un intermediario de mensajes entre diferentes aplicaciones o sistemas, veremos cómo se instala, configura y comprobaremos su funcionamiento mediante la implementación de un caso de uso.

# Enunciado
Se pretende construir un sistema para control de temperatura e iluminación en un
edificio inteligente, mediante el empleo de Arduino y tecnología MOM. Para ello haremos
uso de Arduinos (en nuestro caso los simularemos mediante software), para ajustar los
parámetros necesarios para controlar la temperatura y los valores de iluminación dentro
del edificio inteligente.

El lenguaje utilizado en esta práctica es C#, he creado 3 sistemas que se comunican entre ellos, gracias a ActiveMQ. Estos sistemas son: oficina1, oficina2 y consola.

# Funcionamiento 
El funcionamiento es muy sencillo, las oficinas van generando números aleatorios comprendidos entre un rango (0-50 para la temperatura. 200-1000 para la luminosidad). Estos valores son posteados y leídos por la consola, que en base a los valores que de, posteará una acción a realizar para las oficinas. Las oficinas por otro lado, leerán la acción a realizar:
* 0 -> Mantener la temperatura/luminosidad
* 1 -> Aumentar la temperatura/luminosidad
* -1 -> Disminuir la temperatura/luminosidad


# Puesta en marcha
Para la puesta en marcha hay que tener instalado Visual Studio. Ejecutamos la solución, se nos abrirá el proyecto y solamente tenemos que darle a Iniciar.

Una vez en marcha:
* Si queremos modificar algún valor, hemos de pulsar 1, y si queremos dejarlo tal cual está, pulsaremos 0. Una vez le demos a intro se ejecutará todo y cada 5 segundos las oficinas enviarán los datos de temperatura y luminosidad que tienen y leerán las acciones que han de tomar en base a las últimas mediciones.
