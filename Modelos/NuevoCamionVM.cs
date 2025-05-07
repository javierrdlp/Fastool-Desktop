using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Diagnosis.Clases;
using Diagnosis.Servicios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Diagnosis.Modelos
{
    class NuevoCamionVM : ObservableObject
    {
        private ServicioNavegacion servicioNavegacion;
        private ServicioCamionAPI servicioCamionAPI;
        private ServicioClienteAPI servicioClienteAPI;

        private ObservableCollection<Camion> listaCamiones;

        public ObservableCollection<Camion> ListaCamiones
        {
            get { return listaCamiones; }
            set
            {
                SetProperty(ref listaCamiones, value);
            }
        }
        private int? idClienteBuscar;
        public int? IdClienteBuscar
        {
            get { return idClienteBuscar; }
            set { SetProperty(ref idClienteBuscar, value); }
        }
        private Camion nuevoCamion;

        public Camion NuevoCamion
        {
            get { return nuevoCamion; }
            set { SetProperty(ref nuevoCamion, value); }
        }

        private Camion camionSeleccionado;

        public Camion CamionSeleccionado
        {
            get { return camionSeleccionado; }
            set { SetProperty(ref camionSeleccionado, value); }
        }

        private string matriculaBusqueda;
        public string MatriculaBusqueda
        {
            get { return matriculaBusqueda; }
            set { SetProperty(ref matriculaBusqueda, value); }
        }

        public RelayCommand CrearCamionCommand { get; }
        public RelayCommand EliminarCamionCommand { get; }
        public RelayCommand BuscarCamionCommand { get; }
        public RelayCommand AbrirVentanaModificarCamionCommand { get; }

        public NuevoCamionVM()
        {
            servicioNavegacion = new ServicioNavegacion();
            servicioCamionAPI = new ServicioCamionAPI();
            servicioClienteAPI = new ServicioClienteAPI();
            ListaCamiones = new ObservableCollection<Camion>();
            NuevoCamion = new Camion();

            CargarCamiones();
            CrearCamionCommand = new RelayCommand(async () => await AgregarCamion());
            EliminarCamionCommand = new RelayCommand(async () => await EliminarCamion());
            BuscarCamionCommand = new RelayCommand(async () => await BuscarCamionPorMatricula());
            AbrirVentanaModificarCamionCommand = new RelayCommand(AbrirVentanaModificarCamion);


            WeakReferenceMessenger.Default.Register<NuevoCamionVM, CamionSeleccionadoMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(CamionSeleccionado);
                }
            });
            WeakReferenceMessenger.Default.Register<NuevoCamionVM, NuevoCamionMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(NuevoCamion);
                }
            });
            WeakReferenceMessenger.Default.Register<NuevoCamionVM, IdClienteBuscarMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(IdClienteBuscar);
                }
            });
            WeakReferenceMessenger.Default.Register<NuevoCamionVM, MatriculaBuscarMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(MatriculaBusqueda);
                }
            });

        }

        public void AbrirVentanaModificarCamion()
        {
            servicioNavegacion.AbrirVentanaModificarCamion();
            CargarCamiones();

        }

        private async Task CargarCamiones()
        {
            ListaCamiones = await servicioCamionAPI.ObtenerCamiones();
        }

        private async Task AgregarCamion()
        {
            // Verificar si la matrícula ya existe en la lista actual de camiones
            bool matriculaExistente = ListaCamiones.Any(c => c.Matricula == NuevoCamion.Matricula);

            Cliente idClienteExiste = await servicioClienteAPI.BuscarClientePorId();

            // Si la matrícula ya existe en la lista, mostrar un mensaje y salir del método
            if (matriculaExistente)
            {
                MessageBox.Show("Ya existe un camión con la misma matrícula.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (idClienteBuscar != null && idClienteExiste == null)
            {
                MessageBox.Show("No existe un cliente con ese ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (idClienteBuscar == null)
            {
                MessageBox.Show("Formato de ID incorrecto.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Si no existe en la lista, proceder con la inserción del camión
            bool exito = await servicioCamionAPI.AgregarCamion();
            if (exito)
            {
                MessageBox.Show("Camión creado exitosamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                await CargarCamiones(); // Recargar la lista de camiones después de agregar uno nuevo
                NuevoCamion = new Camion(); // Limpiar el formulario de nuevo camión

                // Limpiar los campos en el modelo de vista
                NuevoCamion.Marca = string.Empty;
                NuevoCamion.Matricula = string.Empty;
                NuevoCamion.Modelo = string.Empty;
                NuevoCamion.IdCliente = null; // Limpiar el cliente asociado al nuevo camión
                IdClienteBuscar = null;
            }
            else
            {
                MessageBox.Show("Error al agregar el camión. Por favor, inténtelo de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task EliminarCamion()
        {
            try
            {
                if (CamionSeleccionado != null)
                {
                    MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea eliminar este camión?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Llamar al método del servicio para eliminar el camión seleccionado
                        bool exito = await servicioCamionAPI.EliminarCamion();

                        if (exito)
                        {
                            // Mostrar un mensaje de éxito
                            MessageBox.Show("Camión eliminado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Actualizar la lista de camiones
                            await CargarCamiones();
                        }
                        else
                        {
                            // Manejar el caso en el que la eliminación no sea exitosa
                            MessageBox.Show("Error al eliminar el camión.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    // Manejar el caso en el que no se haya seleccionado ningún camión para eliminar
                    MessageBox.Show("Por favor, seleccione un camión para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task BuscarCamionPorMatricula()
        {
            if (string.IsNullOrEmpty(MatriculaBusqueda))
            {
                await CargarCamiones(); // Si el campo de búsqueda está vacío, cargar todos los camiones
            }
            else
            {
                // Realizar la solicitud para buscar el camión por matrícula
                Camion camionEncontrado = await servicioCamionAPI.BuscarCamionPorMatricula();

                // Verificar si se encontró un camión
                if (camionEncontrado != null)
                {
                    // Limpiar la lista actual de camiones y agregar el camión encontrado
                    ListaCamiones.Clear();
                    ListaCamiones.Add(camionEncontrado);
                }
                else
                {
                    MessageBox.Show("No se encontró ningún camión con la matrícula especificada.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
        }
    }
}

