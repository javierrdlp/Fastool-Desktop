using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Diagnosis.Clases;
using Diagnosis.Servicios;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Text.RegularExpressions;

namespace Diagnosis.Modelos
{
    class NuevoClienteVM : ObservableObject
    {
        private ServicioNavegacion servicioNavegacion; 
        private ServicioClienteAPI servicioClienteAPI;
            
        private ObservableCollection<Cliente> listaClientes;

        public ObservableCollection<Cliente> ListaClientes
        {
            get { return listaClientes; }
            set
            {
                SetProperty(ref listaClientes, value);
            }
        }

        private Cliente nuevoCliente;

        public Cliente NuevoCliente
        {
            get { return nuevoCliente; }
            set { SetProperty(ref nuevoCliente, value); }
        }

        private Cliente clienteSeleccionado;

        public Cliente ClienteSeleccionado
        {
            get { return clienteSeleccionado; }
            set { SetProperty(ref clienteSeleccionado, value); }
        }

        private string cifBusqueda;
        public string CifBusqueda
        {
            get { return cifBusqueda; }
            set { SetProperty(ref cifBusqueda, value); }
        }

        public RelayCommand CrearClienteCommand { get; }
        public RelayCommand EliminarClienteCommand { get; }
        public RelayCommand BuscarClienteCommand { get; }
        public RelayCommand AbrirVentanaModificarClienteCommand { get; }

        public NuevoClienteVM()
        {
            servicioNavegacion = new ServicioNavegacion();
            servicioClienteAPI = new ServicioClienteAPI();
            ListaClientes = new ObservableCollection<Cliente>();
            NuevoCliente = new Cliente();
            CargarClientes();
            CrearClienteCommand = new RelayCommand(async () => await AgregarCliente());
            EliminarClienteCommand = new RelayCommand(async () => await EliminarCliente());
            BuscarClienteCommand = new RelayCommand(async () => await BuscarClientePorCif());
            AbrirVentanaModificarClienteCommand = new RelayCommand(AbrirVentanaModificarCliente);
            try
            {
                WeakReferenceMessenger.Default.Register<NuevoClienteVM, ClienteSeleccionadoMessage>(this, (r, m) =>
                {
                    if (!m.HasReceivedResponse)
                    {
                        m.Reply(ClienteSeleccionado);
                    }
                    
                });
            }catch (Exception ex)
            {
                MessageBox.Show("Vuelva a seleccionar cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            WeakReferenceMessenger.Default.Register<NuevoClienteVM, NuevoClienteMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(NuevoCliente);
                }
            });
            WeakReferenceMessenger.Default.Register<NuevoClienteVM, CifBucarMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(CifBusqueda);
                }
            });

        }

        public void AbrirVentanaModificarCliente()
        {
            servicioNavegacion.AbrirVentanaModificarCliente();
            CargarClientes();

        }

        private async Task CargarClientes()
        {
            ListaClientes = await servicioClienteAPI.ObtenerClientes();
        }

        private async Task AgregarCliente()
        {
            // Verificar si el CIF o el correo electrónico ya existen en la lista actual de clientes
            bool cifExistente = ListaClientes.Any(c => c.Cif == NuevoCliente.Cif);
            bool emailExistente = ListaClientes.Any(c => c.Email == NuevoCliente.Email);

            // Si el CIF o el correo electrónico ya existen en la lista, mostrar un mensaje y salir del método
            if (cifExistente || emailExistente)
            {
                string mensaje = "Ya existe un cliente con ";
                if (cifExistente && emailExistente)
                {
                    mensaje += "el mismo CIF y correo electrónico.";
                }
                else if (cifExistente)
                {
                    mensaje += "el mismo CIF.";
                }
                else
                {
                    mensaje += "el mismo correo electrónico.";
                }

                MessageBox.Show(mensaje, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Verificar si el formato del CIF es válido
            if (!EsCifValido(NuevoCliente.Cif))
            {
                MessageBox.Show("El CIF tiene un formato inválido.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!esCorreo(NuevoCliente.Email))
            {
                MessageBox.Show("El Email tiene un formato inválido.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Si no existe en la lista y el CIF tiene un formato válido, proceder con la inserción del cliente
            bool exito = await servicioClienteAPI.AgregarCliente();
            if (exito)
            {
                MessageBox.Show("Cliente creado exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                await CargarClientes(); // Recargar la lista de clientes después de agregar uno nuevo
                NuevoCliente = new Cliente(); // Limpiar el formulario de nuevo cliente

                // Limpiar los campos en el modelo de vista
                NuevoCliente.Nombre = string.Empty;
                NuevoCliente.Cif = string.Empty;
                NuevoCliente.Email = string.Empty;
            }
            else
            {
                MessageBox.Show("Error al agregar el cliente. Por favor, inténtelo de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool esCorreo(string correo)
        {
            return correo.Contains("@");
        }

        public bool EsCifValido(string cif)
        {
            // Expresión regular para validar el formato del CIF permitiendo que comience con dígitos
            string patron = @"^\d{8}[A-Za-z0-9]{1}$";
            // Verificar si el CIF coincide con el patrón
            return Regex.IsMatch(cif, patron);
        }

        private async Task EliminarCliente()
        {
            if (ClienteSeleccionado != null)
            {
                MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea eliminar este cliente?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bool exito = await servicioClienteAPI.EliminarCliente();
                    if (exito)
                    {
                        ListaClientes.Remove(ClienteSeleccionado);
                        MessageBox.Show("Cliente eliminado exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar el cliente. Por favor, inténtelo de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningún cliente para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task BuscarClientePorCif()
        {
            if (string.IsNullOrEmpty(CifBusqueda))
            {
                await CargarClientes(); // Si el campo de búsqueda está vacío, cargar todos los clientes
            }
            else
            {
                 Cliente clienteEncontrado = await servicioClienteAPI.BuscarClientePorCif();
               
                if (clienteEncontrado != null)
                {
                    ListaClientes.Clear(); // Limpiar la lista actual de clientes
                    ListaClientes.Add(clienteEncontrado);
                }
                else
                {
                    MessageBox.Show("No se encontró ningún cliente con el CIF especificado.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

    }
}
