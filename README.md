# Consumir un servicio SOAP (XML) desde un API Rest con .Net Core.
Hoy vamos a implementar una solución para poder consumir un servicio SOAP(xml) desde .Net Core y devolver su respuesta en REST.

## Tabla de contenido
1. [Problema](#problema)
2. [Ejercicio](#ejercicio)
3. [Implementación](#implementación)
4. [Ventajas de la solución](#ventajas-de-la-solución)
5. [Desventajas](#desventajas)
6. [Docker](#docker)
7. [Advertencia](#advertencia)
8. [Conclusión](#conclusión)

## Problema
En ocaciones, surge la necesidad de comunicar un sistema de servicios SOAP(xml) con nuevos desarrollos de API's en REST, complicando 
un poco la comunicación entre ellos y llevandonos a pensar que podríamos utilziar para resolver ese inconveniente.

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
seguido de **Aplicación de servicios WCF*
![WCF](https://github.com/Joac89/SoapToRest/blob/master/Blog/CrearWcf.JPG)

Nuestro proyecto en la solución, debe verse asi:

![WCF_View](https://github.com/Joac89/SoapToRest/blob/master/Blog/proyectoWcf.JPG)

**Proyecto API**
Para crear el proyecto, vamos a Visual Studio y seleccionamos en el menú izquierdo la opción .Net Core, seguido de 
**Aplicación web ASP.NET Core**
![REST](https://github.com/Joac89/SoapToRest/blob/master/Blog/crearREST.JPG)

Nuestro proyecto en la solución, debe verse asi:

![REST_View](https://github.com/Joac89/SoapToRest/blob/master/Blog/proyectoREST.JPG)

Luego de tener nuestros proyectos creados, vamos a construir la calculadora para ser expuesta como servicio SOAP(xml). Para ello, 
crearemos la interfaz de servicio con los métodos **Speedup** y **Efficiency**. Ademas, necesitaremos los contratos de datos para las 
operaciones descritas

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

Ahora, vamos por nuestra API Rest intermediario y para ello, crearemos un nuevo controlador llamado **CalculatorController** que 
tendrá los mismos métodos que usa el servicio SOAP.
Para apoyar el desarrollo del controlador, crearemos dos capas (carpetas) en el proyecto que contendrán las clases 
necesarias y los modelos de la implementación.

Los modelos, nos permitirán tomar los datos de la respuesta XML del servicio SOAP y deserializarla en un Objeto local, para luego 
enviarla como respuesta en formato REST desde nuestra API.

¿Como construimos el modelo?
Primero, debemos conocer el **Request** y **Response** del servicio SOAP para entender que recibe y que devuelve y 
así poder transformarlo. Utilizando herramientas como SOAP-UI y el complemento Boomerang de Google, podemos consumir el servicio.


