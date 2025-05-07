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
    internal class ModificarCamionVM : ObservableObject
    {
        private ServicioCamionAPI servicioCamionAPI;
        private ServicioClienteAPI servicioClienteAPI;

        private Camion? camionSeleccionado;
       
        public Camion? CamionSeleccionado
        {
            get { return camionSeleccionado; }
            set { SetProperty(ref camionSeleccionado, value); }
        }

        private int? idClienteBuscar;
        public int? IdClienteBuscar
        {
            get { return idClienteBuscar; }
            set { SetProperty(ref idClienteBuscar, value); }
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
        public RelayCommand ModificarCamionCommand { get; }

        public ModificarCamionVM()
        {
            CamionSeleccionado = WeakReferenceMessenger.Default.Send<CamionSeleccionadoMessage>();
            ModificarCamionCommand = new RelayCommand(ModificarCamion);
            servicioCamionAPI = new ServicioCamionAPI();
            servicioClienteAPI = new ServicioClienteAPI();
            ListaClientes = new ObservableCollection<Cliente>();
            //Para que aparezca el numero a la hora de modificar, si no, estaría vacío.
            IdClienteBuscar = CamionSeleccionado?.IdCliente?.Id;

            WeakReferenceMessenger.Default.Register<ModificarCamionVM, CamionSeleccionadoMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(CamionSeleccionado);
                }
            });

            WeakReferenceMessenger.Default.Register<ModificarCamionVM, IdClienteBuscarMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(IdClienteBuscar);
                }
            });
            CargarClientes();

        }
        private async Task CargarClientes()
        {
            ListaClientes = await servicioClienteAPI.ObtenerClientes();
        }
        public async void ModificarCamion()
        {
            try
            {
                bool clienteExiste = VerificarExistenciaCliente();               
                
                if (!clienteExiste)
                {
                    MessageBox.Show("El ID del cliente especificado no existe.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

               
                bool exito = await servicioCamionAPI.EditarCamion();

                if (exito)
                {
                    MessageBox.Show("Camión modificado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                   
                    var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                    window?.Close();
                }
                else
                {
                    MessageBox.Show("Error al modificar el camión.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                  
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Se produjo un error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool VerificarExistenciaCliente()
        {
            try
            {
                foreach (var cliente in ListaClientes)
                {
                    if (cliente.Id == IdClienteBuscar)
                    {
                        CamionSeleccionado.IdCliente = cliente;
                        return true;
                    }
                }                
                return false;
                
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error al verificar la existencia del cliente: " + ex.Message);
                return false;
            }
        }
    }
}
