using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnosis.Servicios
{
    class ServicioNavegacion
    {

        public void AbrirMain()
        {
            MainWindow nuevaVentana = new MainWindow();

            nuevaVentana.ShowDialog();

        }
        public void AbrirVentanaNuevoCliente()
        {
            NuevoCliente nuevaVentana = new NuevoCliente();

            nuevaVentana.Show();

        }

        public void AbrirVentanaNuevoCamion()
        {
            NuevoCamion nuevaVentana = new NuevoCamion();

            nuevaVentana.Show();

        }

        public void AbrirVentanaNuevaReparacion()
        {
            NuevaReparacion nuevaVentana = new NuevaReparacion();

            nuevaVentana.ShowDialog();

        }

        public void AbrirVentanaModificarCliente()
        {
            ModificarCliente nuevaVentana = new ModificarCliente();

            nuevaVentana.ShowDialog();

        }

        public void AbrirVentanaModificarCamion()
        {
            ModificarCamion nuevaVentana = new ModificarCamion();

            nuevaVentana.ShowDialog();

        }

        public void AbrirVentanaResumen()
        {
            CrearPDF nuevaVentana = new CrearPDF();
            nuevaVentana.ShowDialog();
        }

    }
}
