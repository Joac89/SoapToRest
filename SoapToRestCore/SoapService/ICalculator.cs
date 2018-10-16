using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SoapService
{
	// NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
	[ServiceContract]
	public interface ICalculator
	{
		[OperationContract]
		Result Speedup(Speedup data);

		[OperationContract]
		Result Efficiency(Amdhal data);

	}
	
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
}
