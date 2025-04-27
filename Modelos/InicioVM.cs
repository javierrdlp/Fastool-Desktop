using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Diagnosis.Clases;
using Diagnosis.Servicios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Diagnosis.Modelos
{
    class InicioVM : ObservableObject
    {
        private ServicioNavegacion servicioNavegacion;
        private ServicioCamionAPI servicioCamionAPI;
        private ServicioReparacionAPI servicioReparacionAPI;


        private ObservableCollection<Reparacion> listaReparaciones;
        public ObservableCollection<Reparacion> ListaReparaciones
        {
            get { return listaReparaciones; }
            set
            {
                SetProperty(ref listaReparaciones, value);
            }
        }

        private Reparacion reparacionSeleccionada;

        public Reparacion ReparacionSeleccionada
        {
            get { return reparacionSeleccionada; }
            set
            {
                SetProperty(ref reparacionSeleccionada, value);
                CalcularDuracion();
            }
        }

        private string matriculaBusqueda;
        public string MatriculaBusqueda
        {
            get { return matriculaBusqueda; }
            set { SetProperty(ref matriculaBusqueda, value); }
        }

        private float? duracionEnHoras;

        public float? DuracionEnHoras
        {
            get { return duracionEnHoras; }
            set
            {
                if (duracionEnHoras != value)
                {
                    SetProperty(ref duracionEnHoras, value);
                }
            }
        }
        private Camion? camionEncontrado;

        public Camion? CamionEncontrado
        {
            get { return camionEncontrado; }
            set { SetProperty(ref camionEncontrado, value); }
        }

        private DateTime? horaFin;
        public DateTime? HoraFin
        {
            get { return horaFin; }
            set { SetProperty(ref horaFin, value); }
        }

        public RelayCommand EliminarReparacionCommand { get; }
        public RelayCommand BuscarPorMatriculaCommand { get; }
        public RelayCommand ModificarReparacionCommand { get; }
        public RelayCommand AbrirVentanaNuevoClienteCommand { get; }
        public RelayCommand AbrirVentanaNuevoCamionCommand { get; }
        public RelayCommand AbrirVentanaNuevaReparacionCommand { get; }
        public RelayCommand AbrirVentanaResumenCommand { get; }



        public InicioVM()
        {
            ListaReparaciones = new ObservableCollection<Reparacion>();
            servicioReparacionAPI = new ServicioReparacionAPI();
            servicioCamionAPI = new ServicioCamionAPI();
            CargarReparaciones();
            servicioNavegacion = new ServicioNavegacion();
            AbrirVentanaNuevoClienteCommand = new RelayCommand(AbrirVentanaNuevoCliente);
            AbrirVentanaNuevoCamionCommand = new RelayCommand(AbrirVentanaNuevoCamion);
            AbrirVentanaNuevaReparacionCommand = new RelayCommand(AbrirVentanaNuevaReparacion);
            AbrirVentanaResumenCommand = new RelayCommand(AbrirVentanaResumen);
            EliminarReparacionCommand = new RelayCommand(async () => await EliminarReparacion());
            BuscarPorMatriculaCommand = new RelayCommand(async () => await BuscarCamionReparacionPorMatricula());
            ModificarReparacionCommand = new RelayCommand(async () => await ModificarReparacion());
            AbrirMain();
            WeakReferenceMessenger.Default.Register<InicioVM, CamionEncontradoMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(CamionEncontrado);
                }
            });


            WeakReferenceMessenger.Default.Register<InicioVM, ReparacionSeleccionadaMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(ReparacionSeleccionada);
                }

            });

            DuracionEnHoras = null;

        }

        private void AbrirMain()
        {
            servicioNavegacion.AbrirMain();

        }

        private async Task CargarReparaciones()
        {
            // Obtener la lista de reparaciones
            var reparaciones = await servicioReparacionAPI.ObtenerReparaciones();

            // Ordenar la lista por fecha de inicio descendente
            ListaReparaciones = new ObservableCollection<Reparacion>(
                reparaciones.OrderByDescending(r => r.HoraInicio)
            );
        }


        private async Task BuscarCamionReparacionPorMatricula()
        {

            if (string.IsNullOrEmpty(MatriculaBusqueda))
            {
                await CargarReparaciones(); ; // Si el campo de búsqueda está vacío, cargar todos los camiones
                CamionEncontrado = null;
            }
            else
            {
                try
                {
                    // Realizar la solicitud para buscar el camión por matrícula
                    CamionEncontrado = await servicioCamionAPI.BuscarCamionPorMatricula(MatriculaBusqueda);


                    // Verificar si se encontró un camión
                    if (camionEncontrado != null)
                    {
                        // Obtener la lista de reparaciones
                        var reparaciones = await servicioReparacionAPI.ReparacionesPorMatricula(MatriculaBusqueda);

                        // Ordenar la lista por fecha de inicio descendente
                        ListaReparaciones = new ObservableCollection<Reparacion>(reparaciones.OrderByDescending(r => r.HoraInicio));

                    }
                    else
                    {
                        MessageBox.Show("No se encontró ningún camión con la matrícula especificada.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        await CargarReparaciones();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al buscar camión: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CalcularDuracion()
        {
            if (ReparacionSeleccionada?.HoraFin != null)
            {
                TimeSpan duracionTimeSpan = ReparacionSeleccionada.HoraFin.Value - ReparacionSeleccionada.HoraInicio;
                double duracionHoras = duracionTimeSpan.TotalHours;
                DuracionEnHoras = (float)Math.Round(duracionHoras, 2);
            }
            else
            {
                DuracionEnHoras = null;
            }
        }



        private async Task ModificarReparacion()
        {
            try
            {
                // Validar que la hora de fin no sea menor que la hora de inicio
                if (ReparacionSeleccionada.HoraFin.HasValue)
                {
                    if (ReparacionSeleccionada.HoraFin < ReparacionSeleccionada.HoraInicio)
                    {
                        MessageBox.Show("La hora de fin no puede ser anterior a la hora de inicio.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Validar que la fecha de fin no sea del día siguiente
                    if (ReparacionSeleccionada.HoraFin.Value.Date > ReparacionSeleccionada.HoraInicio.Date)
                    {
                        MessageBox.Show("La hora de fin no puede ser de un día diferente al de inicio.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // Realizar la solicitud PUT a la URL para modificar la reparación
                bool exito = await servicioReparacionAPI.ModificarReparacion(ReparacionSeleccionada);

                if (exito)
                {
                    MessageBox.Show("Reparación modificada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                    BuscarCamionReparacionPorMatricula();
                }
                else
                {
                    MessageBox.Show("Error al modificar la reparación.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task EliminarReparacion()
        {
            try
            {
                if (ReparacionSeleccionada != null)
                {
                    // Mostrar un MessageBox de confirmación
                    MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea eliminar esta reparación?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    // Verificar la respuesta del usuario
                    if (result == MessageBoxResult.Yes)
                    {
                        // El usuario confirmó que quiere eliminar la reparación
                        bool exito = await servicioReparacionAPI.EliminarReparacion(ReparacionSeleccionada.Id);

                        if (exito)
                        {
                            MessageBox.Show("Reparación eliminada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            if (MatriculaBusqueda != null)
                            {
                                BuscarCamionReparacionPorMatricula();
                            }
                            else
                            {
                                await CargarReparaciones();
                            }

                        }
                        else
                        {
                            MessageBox.Show("Error al eliminar la reparación.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione una reparación para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AbrirVentanaResumen()
        {
            if (reparacionSeleccionada.Matricula == null)
            {
                MessageBox.Show("No se puede sacar un resumen de una reparación sin matrícula", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                servicioNavegacion.AbrirVentanaResumen();
            }

        }

        public void AbrirVentanaNuevoCliente()
        {
            servicioNavegacion.AbrirVentanaNuevoCliente();

        }

        public void AbrirVentanaNuevoCamion()
        {
            servicioNavegacion.AbrirVentanaNuevoCamion();

        }

        public void AbrirVentanaNuevaReparacion()
        {
            servicioNavegacion.AbrirVentanaNuevaReparacion();
            BuscarCamionReparacionPorMatricula();


        }


    }
}
