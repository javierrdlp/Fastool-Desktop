using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Diagnosis.Clases;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diagnosis.Servicios
{
    public class ServicioClienteAPI : ObservableObject
    {
        private readonly string baseUrl = "http://localhost:8080/fasttool/datos/clientes";

        private Cliente clienteServicio;
        public Cliente ClienteServicio
        {
            get { return clienteServicio; }
            set
            {
                SetProperty(ref clienteServicio, value);
            }
        }
        private string cifBusqueda;
        public string CifBusqueda
        {
            get { return cifBusqueda; }
            set { SetProperty(ref cifBusqueda, value); }
        }

        private int? idCliente;
        public int? IdCliente
        {
            get { return idCliente; }
            set { SetProperty(ref idCliente, value); }
        }

        public async Task<ObservableCollection<Cliente>> ObtenerClientes()
        {
            ObservableCollection<Cliente> listaClientes = new ObservableCollection<Cliente>();

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(baseUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var clientes = JsonConvert.DeserializeObject<List<Cliente>>(jsonString);
                        foreach (var cliente in clientes)
                        {
                            listaClientes.Add(cliente);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error al obtener los clientes: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return listaClientes;
        }

        public async Task<bool> AgregarCliente()
        {
            try
            {
                ClienteServicio = WeakReferenceMessenger.Default.Send<NuevoClienteMessage>();
                using (var httpClient = new HttpClient())
                {
                    var clienteJson = JsonConvert.SerializeObject(ClienteServicio);
                    var contenido = new StringContent(clienteJson, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(baseUrl, contenido);
                    if (response.IsSuccessStatusCode)
                    {
                        return true; 
                    }
                    else
                    {
                        Console.WriteLine("Error al agregar cliente: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return false;
        }

        public async Task<bool> EliminarCliente()
        {
            try
            {
                ClienteServicio = WeakReferenceMessenger.Default.Send<ClienteSeleccionadoMessage>();
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.DeleteAsync($"{baseUrl}/{ClienteServicio.Id}");
                    if (response.IsSuccessStatusCode)
                    {
                        return true; 
                    }
                    else
                    {
                        Console.WriteLine("Error al eliminar cliente: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return false;
        }

        public async Task<Cliente> BuscarClientePorCif()
        {
            try
            {
                CifBusqueda = WeakReferenceMessenger.Default.Send<CifBucarMessage>();
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync($"{baseUrl}/cif/{CifBusqueda}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var cliente = JsonConvert.DeserializeObject<Cliente>(jsonString);
                        return cliente;
                    }
                    else
                    {
                        Console.WriteLine("Error al obtener el cliente: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null;
        }
        public async Task<Cliente> BuscarClientePorId(int? id)
        {
            try
            {
                IdCliente = WeakReferenceMessenger.Default.Send<IdClienteBuscarMessage>();
                if(IdCliente == null)
                {
                    IdCliente = id;
                }
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync($"{baseUrl}/{IdCliente}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var cliente = JsonConvert.DeserializeObject<Cliente>(jsonString);
                        return cliente;
                    }
                    else
                    {
                        Console.WriteLine("Error al obtener el cliente: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null;
        }


        public async Task<bool> ModificarCliente()
        {
            try
            {
                ClienteServicio = WeakReferenceMessenger.Default.Send<ClienteSeleccionadoMessage>();
                using (var httpClient = new HttpClient())
                {
                    var clienteJson = JsonConvert.SerializeObject(ClienteServicio);
                    var contenido = new StringContent(clienteJson, Encoding.UTF8, "application/json");
                    var response = await httpClient.PutAsync("http://localhost:8080/fasttool/datos/clientes", contenido);

                    if (response.IsSuccessStatusCode)
                    {
                        return true; 
                    }
                    else
                    {
                        Console.WriteLine("Error al actualizar cliente: " + response.StatusCode);
                        return false; 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false; 
            }
        }
    }
}
