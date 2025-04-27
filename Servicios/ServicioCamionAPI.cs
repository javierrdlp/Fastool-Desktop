using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Diagnosis.Clases;
using Diagnosis.Modelos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diagnosis.Servicios
{
    public class ServicioCamionAPI: ObservableObject
    {
        private readonly string baseUrl = "http://localhost:8080/fasttool/datos/camiones";
        ServicioClienteAPI servicioClienteAPI = new ServicioClienteAPI();

        private Camion camionServicio;
        public Camion CamionServicio
        {
            get { return camionServicio; }
            set
            {
                SetProperty(ref camionServicio, value);
            }
        }

        private int? idCliente;
        public int? IdCliente
        {
            get { return idCliente; }
            set { SetProperty(ref idCliente, value); }
        }

        private string matriculaBusqueda;
        public string MatriculaBusqueda
        {
            get { return matriculaBusqueda; }
            set { SetProperty(ref matriculaBusqueda, value); }
        }
        public async Task<ObservableCollection<Camion>> ObtenerCamiones()
        {
            ObservableCollection<Camion> listaCamiones = new ObservableCollection<Camion>();

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(baseUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var camionesDTO = JsonConvert.DeserializeObject<List<Camion>>(jsonString);
                        listaCamiones = new ObservableCollection<Camion>(camionesDTO);
                    }
                    else
                    {
                        Trace.WriteLine("Error al obtener los camiones: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error: " + ex.Message);
            }

            return listaCamiones;
        }

        public async Task<bool> AgregarCamion()
        {
            try
            {
                CamionServicio = WeakReferenceMessenger.Default.Send<NuevoCamionMessage>();
                IdCliente = WeakReferenceMessenger.Default.Send<IdClienteBuscarMessage>();
                // Buscar el cliente por su ID
                var cliente = await servicioClienteAPI.BuscarClientePorId(IdCliente);

                
                    using (var httpClient = new HttpClient())
                    {
                        // Crear el objeto camionDTO con los datos del nuevo camión
                        var camionDTO = new
                        {
                            matricula = CamionServicio.Matricula,
                            marca = CamionServicio.Marca,
                            modelo = CamionServicio.Modelo,    
                            clienteId = cliente // Utilizar el ID del cliente obtenido
                        };

                        // Serializar el objeto camionDTO a formato JSON
                        var camionJson = JsonConvert.SerializeObject(camionDTO);

                        // Crear el contenido de la solicitud con el JSON serializado
                        var contenido = new StringContent(camionJson, Encoding.UTF8, "application/json");

                        // Realizar la solicitud POST a la URL
                        var response = await httpClient.PostAsync(baseUrl, contenido);

                        // Verificar si la solicitud fue exitosa
                        if (response.IsSuccessStatusCode)
                        {
                            // El camión se agregó exitosamente
                            return true;
                        }
                        else
                        {
                            // Manejar el caso en el que la solicitud no sea exitosa
                            Trace.WriteLine("Error al agregar el camión: " + response.StatusCode);
                            return false;
                        }
                    }
                
                
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción
                Trace.WriteLine("Error: " + ex.Message);
                return false;
            }
        }


        public async Task<bool> EliminarCamion()
        {
            try
            {
                CamionServicio = WeakReferenceMessenger.Default.Send<CamionSeleccionadoMessage>();
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.DeleteAsync($"{baseUrl}/{CamionServicio.Id}");

                    if (response.IsSuccessStatusCode)
                    {
                        return true; // La eliminación fue exitosa
                    }
                    else
                    {
                        Trace.WriteLine("Error al eliminar el camión: " + response.StatusCode);
                        return false; // La eliminación falló
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error: " + ex.Message);
                return false; // La eliminación falló debido a una excepción
            }
        }
        
        public async Task<Camion> BuscarCamionPorMatricula(string matricula)
        {
            try
            {
                //MatriculaBusqueda = WeakReferenceMessenger.Default.Send<MatriculaBuscarMessage>();
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync($"{baseUrl}/matricula/{matricula}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var camiones = JsonConvert.DeserializeObject<List<Camion>>(jsonString);

                        return camiones[0];
                    }
                    else
                    {
                        Trace.WriteLine("Error al obtener el camión: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error: " + ex.Message);
            }

            return null;
        }

        public async Task<bool> EditarCamion()
        {
            try
            {
                CamionServicio = WeakReferenceMessenger.Default.Send<CamionSeleccionadoMessage>();             
                

                using (var httpClient = new HttpClient())
                {
                    
                    // Serializar el objeto camionDTO a formato JSON
                    var camionJson = JsonConvert.SerializeObject(CamionServicio);

                    // Crear el contenido de la solicitud con el JSON serializado
                    var contenido = new StringContent(camionJson, Encoding.UTF8, "application/json");

                    // Realizar la solicitud POST a la URL
                    var response = await httpClient.PutAsync(baseUrl, contenido);

                    // Verificar si la solicitud fue exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // El camión se agregó exitosamente
                        return true;
                    }
                    else
                    {
                        // Manejar el caso en el que la solicitud no sea exitosa
                        Trace.WriteLine("Error al modificar el camión: " + response.StatusCode);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }



    }  
    
}
