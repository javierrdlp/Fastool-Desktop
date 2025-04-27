using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnosis.Clases
{
    public class DatosPDF : ObservableObject
    {
        private string ruta;
        private string? observaciones;
        private Reparacion reparacion;

        public string Ruta
        {
            get { return ruta; }
            set
            {
                if (ruta != value)
                {
                    SetProperty(ref ruta, value);
                }
            }
        }

        public string? Observaciones
        {
            get { return observaciones; }
            set
            {
                if (observaciones != value)
                {
                    SetProperty(ref observaciones, value);
                }
            }
        }

        public Reparacion Reparacion
        {
            get { return reparacion; }
            set
            {
                if (reparacion != value)
                {
                    SetProperty(ref reparacion, value);
                }
            }
        }
        public DatosPDF() { }

        public DatosPDF(string ruta, string? observaciones, Reparacion reparacion)
        {
            this.ruta = ruta;
            this.observaciones = observaciones;
            this.reparacion = reparacion;
    }
    }
}
