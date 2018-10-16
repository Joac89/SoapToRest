# Consumir un servicio SOAP (XML) desde un API Rest con .Net Core y responder con JSON
Hoy vamos a implementar una solución para poder consumir un servicio SOAP(xml) desde .Net Core (versión 2.0) y devolver su respuesta en REST.

## Tabla de contenido
1. [Problema](#problema)
2. [Ejercicio](#ejercicio)
3. [Implementación](#implementación)
4. [Modelos](#modelos)
5. [Transformación](#transformación)
6. [Conclusión](#conclusión)
7. [Nota](#nota)

## Problema
En ocasiones, surge la necesidad de comunicar un sistema de servicios SOAP(xml) con nuevos desarrollos de API's en REST, complicando 
un poco la comunicación entre ellos y llevandonos a pensar que podríamos utilizar para resolver ese inconveniente.  

En el siguiente taller, utilizaremos .Net Core para construir un intermediario que nos permita consumir el servicio SOAP y dar 
su respuesta en REST, con el fin de que futuras API's REST creadas puedan utilizar la funcionalidad del servicio sin pasar de nuevo por 
el mismo problema.

## Ejercicio
Crearemos un proyecto que permita calcular el **Speedup** y la **Eficiencia** para soluciones de cómputo intensivo que sería 
simplemente una calculadora con dos métodos.

## Implementación
Crearemos un proyecto WCF y un API Rest con Visual Studio para el ejercicio propuesto. Antes de crear los proyectos, se recomienda
crear una solución en blanco y luego dentro de esa solución, agregar los nuevos proyectos.

**Proyecto WCF**  
Para crear el proyecto, vamos a Visual Studio, y seleccionamos en el menú izquierdo la opción WCF, 
seguido de **Aplicación de servicios WCF**  
![WCF](https://github.com/Joac89/SoapToRest/blob/master/Blog/CrearWcf.JPG)

Nuestro proyecto en la solución, debe verse asi:  
![WCF_View](https://github.com/Joac89/SoapToRest/blob/master/Blog/proyectoWcf.JPG)

**Proyecto API**  
Para crear el proyecto, vamos a Visual Studio y seleccionamos en el menú izquierdo la opción .Net Core, seguido de 
**Aplicación web ASP.NET Core**  
![REST](https://github.com/Joac89/SoapToRest/blob/master/Blog/crearREST.JPG)

Nuestro proyecto en la solución, debe verse asi  
![REST_View](https://github.com/Joac89/SoapToRest/blob/master/Blog/proyectoREST.JPG)

Luego de tener nuestros proyectos creados, vamos a construir la calculadora para ser expuesta como servicio SOAP(xml). Para ello, 
crearemos la interfaz de servicio con los métodos **Speedup** y **Efficiency**. Ademas, necesitaremos los contratos de datos para las 
operaciones descritas.  

**Interfaz:**  
```
  	[ServiceContract]
	public interface ICalculator
	{
		[OperationContract]
		Result Speedup(Speedup data);

		[OperationContract]
		Result Efficiency(Amdhal data);
	}
```

**Contratos:**  
```
  	[DataContract]
	public class Speedup
	{
		[DataMember]
		public decimal Core { get; set; }
		[DataMember]
		public decimal Time { get; set; }
		[DataMember]
		public decimal SerialTime { get; set; }
	}

	[DataContract]
	public class Amdhal
	{
		[DataMember]
		public decimal Core { get; set; }
		[DataMember]
		public decimal Speedup { get; set; }
	}

	[DataContract]
	public class Result
	{
		[DataMember]
		public decimal Calculated { get; set; }
	}
```

La clase Result, la utilizaremos para devolver el resultado de los métodos de la interfaz.
Teniendo ya nuestra interfaz, vamos a realizar su implementación. Para ello, crearemos una clase llamada Calculator que tendrá la 
implementación de la Interfaz:  

```
  	public class Calculator : ICalculator
	{
		public Result Efficiency(Amdhal data)
		{
			// = (SPEEDUP / CORES ) * 100
			var result = (data.Speedup / data.Core) * 100;

			return new Result()
			{
				Calculated = Math.Round(result, 4)
			};
		}

		public Result Speedup(Speedup data)
		{
			// = SERIAL+TIME / (TIME/CORES+SERIAL)
			var result = (data.SerialTime + data.Time) / ((data.Time / data.Core) + data.SerialTime);

			return new Result()
			{
				Calculated = Math.Round(result, 4)
			};
		}
	}
  ```
Con ésto, tenemos ya nuestro servicio WCF y continuaremos con la construcción de una API Rest que servirá de transformador entre 
las respuestas XML del servicio y los resultados REST.

**Probemos el WCF!!**  
![SpeedSoap](https://github.com/Joac89/SoapToRest/blob/master/Blog/pruebaSpeed.JPG)
![EficiencySoap](https://github.com/Joac89/SoapToRest/blob/master/Blog/pruebaEficiencia.JPG)


Ahora, vamos por nuestra API Rest intermediario y para ello, crearemos un nuevo controlador llamado **CalculatorController** que 
tendrá los mismos métodos que usa el servicio SOAP.  
Para apoyar el desarrollo del controlador, crearemos dos capas (carpetas) en el proyecto que contendrán las clases 
necesarias y los modelos de la implementación.  

Los modelos, nos permitirán tomar los datos de la respuesta XML del servicio SOAP y deserializarla en un Objeto local, para luego 
enviarla como respuesta en formato REST desde nuestra API.

## Modelos
¿Cómo construimos los modelos? Primero, debemos conocer el **Request** y **Response** del servicio SOAP para entender que recibe y que devuelve y 
así poder transformarlo. Utilizando herramientas como SOAP-UI o el complemento Boomerang de Google, podemos consumir el servicio.

**Requests y Responses**  
Para el método **Speedup**:  
*Request*  
![SpeedReq](https://github.com/Joac89/SoapToRest/blob/master/Blog/requestSpeedup.JPG)

*Response*  
![SpeedRes](https://github.com/Joac89/SoapToRest/blob/master/Blog/responseSpeedup.JPG)

Para el método **Efficiency**:  
*Request*  
![SpeedReq](https://github.com/Joac89/SoapToRest/blob/master/Blog/requestEfficiency.JPG)

*Response*  
![SpeedRes](https://github.com/Joac89/SoapToRest/blob/master/Blog/responseEfficiency.JPG)

Ahora que conocemos lo que recibe y responde el servicio SOAP para los métodos a implementar, procedemos a crear los modelos 
en nuestra API Res.

**Modelos en C#**
Speedup
  ```
	namespace ApiRest.Model
  	{
		public class Speedup
		{
			public decimal Calculated { get; set; }
		}
	
    		public class SpeedupRoot
    		{
			public SpeedupEnvelope Envelope { get; set; }
    		}

		public class SpeedupResult
		{
			public decimal Calculated { get; set; }
		}

		public class SpeedupResponse
		{
			public SpeedupResult SpeedupResult { get; set; }
		}

		public class SpeedupBody
		{
			public SpeedupResponse SpeedupResponse { get; set; }
		}

		public class SpeedupEnvelope
		{
			public SpeedupBody Body { get; set; }
		}
	}
  ```
Efficiency
  ```
    	namespace ApiRest.Model
    	{
		public class Efficiency
		{
			public decimal Calculated { get; set; }
		}
	
    		public class EfficiencyRoot
		{
			public EfficiencyEnvelope Envelope { get; set; }
		}

		public class EfficiencyResult
		{
			public decimal Calculated { get; set; }
		}

		public class EfficiencyResponse
		{
			public EfficiencyResult EfficiencyResult { get; set; }
		}

		public class EfficiencyBody
		{
			public EfficiencyResponse EfficiencyResponse { get; set; }
		}

		public class EfficiencyEnvelope
		{
			public EfficiencyBody Body { get; set; }
		}
	}
  ```

Los modelos pueden generarse automáticamente a partir del XML con herramientas online como la siguiente: https://xmltocsharp.azurewebsites.net/ aquí, pegamos nuestro XML sin los Namespaces (un xml limpio) y nos devolverá como resultado las
clases en c#.

## Transformación
Teniendo ya nuestros modelos, continuamos implementando nuestro transformados. Para ello, construiremos una clase llamada **Transform** que nos permitirá: Quitar namespaces del XML, limpiar el XML y convertirlo a JSON para ser deserializado.

  ```
 	public static class Transform
    	{
		public static string Exec(string soapResponse)
		{
			var xm = XElement.Parse(soapResponse);
			var response = RemoveAllNamespacesXml(xm).CleanXml().ToJson();

			return response;
		}

		private static string RemoveAllNamespacesXml(XElement xmlDocument)
		{
			try
			{
				if (!xmlDocument.HasElements)
				{
					var xElement = new XElement(xmlDocument.Name.LocalName);
					xElement.Value = xmlDocument.Value;

					foreach (XAttribute attribute in xmlDocument.Attributes())
						xElement.Add(attribute);

					return xElement.ToString();
				}
				return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespacesXml(el))).ToString();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception in T Remove Namespaces XML: " + ex.Message);
				return "";
			}
		}

		private static string ToJson(this string soapResponse)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(soapResponse);

			return JsonConvert.SerializeXmlNode(xmlDoc);
		}

		private static string CleanXml(this string soapResponse)
		{
			soapResponse = soapResponse.Replace("amp;", "").Replace("&lt;", "<").Replace("&gt;", ">");
			return soapResponse;
		}
	}
  ```

La clase se crea de manera estática y se implementan dos métodos de extensión con el fin de simplificar el código a implementar. Puede implementarse de otras maneras, y seguramente más simples. 

¿Recuerdan los Request y Response que estuvimos analizando anteriormente? bueno, con ellos (Request), crearemos la clase **CalculatorService** que simplemente nos permitirá construir (de una manera sencilla y manual) los XML de envío al servicio SOAP.
  ```
       public class CalculatorService
	{
		public static string Url = "http://localhost:59984/Calculator.svc";

		public static string GetRequestSpeedup(string core, string serialTime, string time)
		{
			var request = "<x:Envelope xmlns:x='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/' xmlns:soa='http://schemas.datacontract.org/2004/07/SoapService'>" +
			"<x:Header/>" +
				"<x:Body>" +
				"<tem:Speedup>" +
				"<tem:data>" +
				"<soa:Core>" + core + "</soa:Core>" +
				"<soa:SerialTime>" + serialTime + "</soa:SerialTime>" +
				"<soa:Time>" + time + "</soa:Time>" +
				"</tem:data>" +
				"</tem:Speedup>" +
				"</x:Body>" +
				"</x:Envelope>";

			return request;
		}

		public static string GetRequestEfficiency(string core, string speedup)
		{
			var request = "<x:Envelope xmlns:x='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/' xmlns:soa='http://schemas.datacontract.org/2004/07/SoapService'>" +
			"<x:Header/>" +
				"<x:Body>" +
				"<tem:Efficiency>" +
				"<tem:data>" +
				"<soa:Core>" + core + "</soa:Core>" +
				"<soa:Speedup>" + speedup + "</soa:Speedup>" +
				"</tem:data>" +
				"</tem:Efficiency>" +
				"</x:Body>" +
				"</x:Envelope>";

			return request;
		}
	}
  ```
  
Seguramente puede implementarse de una mejor manera, pero para efectos del taller es suficiente lo propuesto.
Teniendo ya nuestra clase de transformación, solo nos queda implementar el controlador para transformar los datos:
 ```
 	[HttpGet("speedup/{core}/{serialtime}/{time}")]
	public async Task<IActionResult> Speedup(string core, string serialTime, string time)
	{
		var result = new Speedup();

		using (var client = new HttpClient())
		{
			var request = CalculatorService.GetRequestSpeedup(core, serialTime, time);
			var content = new StringContent(request, Encoding.UTF8, "text/xml");
			var action = "http://tempuri.org/ICalculator/Speedup";

			client.DefaultRequestHeaders.Add("SOAPAction", action);

			using (var response = await client.PostAsync(CalculatorService.Url, content))
			{
				var asyncstring = await response.Content.ReadAsStringAsync();
				var soapResponse = Transform.Exec(asyncstring);
				var serialize = JsonConvert.DeserializeObject<SpeedupRoot>(soapResponse);

				result.Calculated = serialize.Envelope.Body.SpeedupResponse.SpeedupResult.Calculated;
			}
		}

		return Ok(result);
	}

	[HttpGet("efficiency/{core}/{speedup}")]
	public async Task<IActionResult> Efficiency(string core, string speedUp)
	{
		var result = new Efficiency();

		using (var client = new HttpClient())
		{
			var request = CalculatorService.GetRequestEfficiency(core, speedUp);
			var content = new StringContent(request, Encoding.UTF8, "text/xml");
			var action = "http://tempuri.org/ICalculator/Efficiency";

			client.DefaultRequestHeaders.Add("SOAPAction", action);

			using (var response = await client.PostAsync(CalculatorService.Url, content))
			{
				var asyncstring = await response.Content.ReadAsStringAsync();
				var soapResponse = Transform.Exec(asyncstring);
				var serialize = JsonConvert.DeserializeObject<EfficiencyRoot>(soapResponse);

				result.Calculated = serialize.Envelope.Body.EfficiencyResponse.EfficiencyResult.Calculated;
			}
		}

		return Ok(result);
	}
 ```
 
¿qué hacen los métodos?: Utilizan la clase HttpClient para hacer el llamado al servicio SOAP. Pero antes de ello, toman los datos enviados al API Rest, construyen el Request que será enviado, envían la respuesta, toman el resultado de la respuesta (con el xml ya transformado a JSON) y deserializan con los modelos construidos anteriormente para así entregar una respuesta JSON en el API Rest.

**Speedup**  
![SpeedEnd](https://github.com/Joac89/SoapToRest/blob/master/Blog/speedupRest.JPG)

**Efficiency**  
![EfficiencyEnd](https://github.com/Joac89/SoapToRest/blob/master/Blog/efficiencyRest.JPG)

¿Notaron que tanto las pruebas de SOAP y REST devolvieron los mismos resultados?. Con ésto, ya tenemos nuestro transformador sencillo de SOAP a REST con .Net Core. Recuerden que .Net Core es multiplataforma y modular.

## Conclusión
De ésta manera, podremos incluir en nuestras arquitecturas, un transformador de SOAP a REST, algo convencional pero que puede ser de gran ayuda en algún momento. Podríamos incluirlo en un patrón dispatcher que necesite realizar transformaciones, y si lo queremos mas complejo, incluiríamos plantillas XSLT o JSON Schemma para realizar transformaciones dinámicas, una tabla de proveedores para implementar Publisher And Subscriber + Dispatcher + Transformación, etc..etc..etc.

## Nota
El ejercicio seguramente puede ser sustituido por herramientas o componentes que realizan la transformación nativa o automática como por ejemplo un Bus como JBoss u otros similares. El fin del taller es entender un poco más y aprender a combinar los detalles que seguramente en nuestro día a día pueden surgir y necesitan ser aborados temporalmente o de manera inmediata mientras se plantean mejores soluciones.
