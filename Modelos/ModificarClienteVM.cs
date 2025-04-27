using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Diagnosis.Clases;
using Diagnosis.Servicios;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Diagnosis.Modelos
{
    internal class ModificarClienteVM:ObservableObject
    {
        private ServicioClienteAPI servicioClienteAPI;

        private Cliente clienteSeleccionado;  
       
        public Cliente ClienteSeleccionado
        {
            get { return clienteSeleccionado; }
            set { SetProperty(ref clienteSeleccionado, value); }
        }

        private ObservableCollection<Cliente> listaClientes;

        public ObservableCollection<Cliente> ListaClientes
        {
            get { return listaClientes; }
            set
            {
                SetProperty(ref listaClientes, value);
            }
        }

        public RelayCommand ModificarClienteCommand { get; }
        public ModificarClienteVM() {
            servicioClienteAPI = new ServicioClienteAPI();
            ListaClientes = new ObservableCollection<Cliente>();            
            ClienteSeleccionado = WeakReferenceMessenger.Default.Send<ClienteSeleccionadoMessage>();
            ModificarClienteCommand = new RelayCommand(ModificarCliente);

            WeakReferenceMessenger.Default.Register<ModificarClienteVM, ClienteSeleccionadoMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(ClienteSeleccionado);
                }

            });            
            CargarClientes();
        }

        private async Task CargarClientes()
        {
            ListaClientes = await servicioClienteAPI.ObtenerClientes();
        }
        private bool esCorreo(string correo)
        {
            return correo.Contains("@");
        }

        public async void ModificarCliente()
        {
            try
            {
                // Verificar si el correo electrónico existe en otro cliente, excluyendo al cliente seleccionado
                bool emailExistente = ListaClientes.Any(c => c.Email == ClienteSeleccionado.Email && c.Id != ClienteSeleccionado.Id);

                if (emailExistente)
                {
                    MessageBox.Show("Ya existe un cliente con el mismo correo electrónico.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!esCorreo(ClienteSeleccionado.Email))
                {
                    MessageBox.Show("Formato de correo eléctronico inválido.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                // Llamar al método del servicio para modificar el cliente
                bool exito = await servicioClienteAPI.ModificarCliente();

                if (exito)
                {
                    MessageBoxResult result = MessageBox.Show("Cliente modificado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (result == MessageBoxResult.OK)
                    {
                        var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                        window?.Close();
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Ha surgido un problema al modificar el cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción
                Trace.WriteLine("Error: " + ex.Message);
            }
        }
        

    }
    
}
