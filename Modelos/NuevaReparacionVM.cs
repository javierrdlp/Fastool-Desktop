using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Diagnosis.Clases;
using Diagnosis.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Diagnosis.Modelos
{
    internal class NuevaReparacionVM : ObservableObject
    {
       
        private ServicioReparacionAPI servicioReparacionAPI;

        private Camion camionEncontrado;
        public Camion CamionEncontrado
        {
            get { return camionEncontrado; }
            set { SetProperty(ref camionEncontrado, value); }
        }
        private string? horaInicio;
        public string? HoraInicio
        {
            get { return horaInicio; }
            set { SetProperty(ref horaInicio, value); }
        }
        private string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set { SetProperty(ref descripcion, value); }
        }


        public RelayCommand CrearReparacionCommand { get; }
        public NuevaReparacionVM() 
        {
            servicioReparacionAPI = new ServicioReparacionAPI();
            CamionEncontrado = WeakReferenceMessenger.Default.Send<CamionEncontradoMessage>();
            CrearReparacionCommand = new RelayCommand(CrearReparacion);
        }

        public async void CrearReparacion()
        {
            try
            {            

                // Crear la reparación con la descripción proporcionada por el usuario
                bool exito = await servicioReparacionAPI.CrearReparacion(CamionEncontrado, Descripcion, HoraInicio);

                if (exito)
                {
                    MessageBox.Show("Reparación creada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                    window?.Close();
                }
                else
                {
                    MessageBox.Show("Error al crear la reparación, compruebe los datos introducidos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
