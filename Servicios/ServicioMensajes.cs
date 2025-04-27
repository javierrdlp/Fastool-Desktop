using CommunityToolkit.Mvvm.Messaging.Messages;
using Diagnosis.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnosis.Servicios
{    
    //Solicitud
    public class ClienteSeleccionadoMessage : RequestMessage<Cliente> { }

    public class NuevoClienteMessage : RequestMessage<Cliente> { }  

    public class CifBucarMessage : RequestMessage<String> { }

    public class IdClienteBuscarMessage : RequestMessage <int?> { }
    
    public class CamionSeleccionadoMessage : RequestMessage<Camion> { }

    public class NuevoCamionMessage : RequestMessage<Camion> { }

    public class MatriculaBuscarMessage : RequestMessage<String> { }

    public class ReparacionSeleccionadaMessage : RequestMessage<Reparacion> { }  
    
    public class CamionEncontradoMessage : RequestMessage<Camion> { }   
    
    public class DatosPDFMessage : RequestMessage<DatosPDF> { }

}
