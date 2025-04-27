using CommunityToolkit.Mvvm.ComponentModel;
using Diagnosis.Modelos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnosis.Clases
{
    public class Reparacion : ObservableObject
    {
        private int id;
        private Camion? matricula;
        private string descripcion;
        private DateTime horaInicio;
        private DateTime? horaFin;


        [JsonProperty("id")]
        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    SetProperty(ref id, value);                    
                }
            }
        }

        [JsonProperty("matricula")]
        public Camion? Matricula
        {
            get { return matricula; }
            set
            {
                if (matricula != value)
                {
                    SetProperty (ref matricula, value);
                }
            }
        }

        [JsonProperty("descripcion")]
        public string Descripcion
        {
            get { return descripcion; }
            set
            {
                if (descripcion != value)
                {
                    SetProperty(ref descripcion, value);
                }
            }
        }

        [JsonProperty("horaInicio")]
        public DateTime HoraInicio
        {
            get { return horaInicio; }
            set
            {
                if (horaInicio != value)
                {
                    SetProperty(ref horaInicio, value);
                }
            }
        }

        [JsonProperty("horaFin")]
        public DateTime? HoraFin
        {
            get { return horaFin; }
            set
            {
                if (horaFin != value)
                {
                    SetProperty(ref horaFin, value);
                }
            }
        }

        public Reparacion() { }
                
        public Reparacion(int id, Camion? matricula, string fecha, string descripcion, DateTime horaInicio, DateTime? horaFin)
        {
            this.id = id;
            this.matricula = matricula;           
            this.descripcion = descripcion;
            this.horaInicio = horaInicio;
            this.horaFin = horaFin;
           
        }

        public override string ToString()
        {
            // Calcular la duración si HoraFin no es nulo
            float duracionFloat = 0.0f;
            if (HoraFin.HasValue)
            {
                TimeSpan duracionTimeSpan = HoraFin.Value - HoraInicio;
                duracionFloat = (float)duracionTimeSpan.TotalHours;
            }

            // Redondear la duración a dos decimales
            string duracionString = Math.Round(duracionFloat, 2).ToString();

            string horaInicioString = HoraInicio.ToString("HH:mm");
            string horaFinString = HoraFin?.ToString("HH:mm") ?? "N/A";
            string fechaInicioString = HoraFin?.ToString("dd/MM/yyyy");

            return $" ID Reparación: {Id}                                                  Hora Inicio: {horaInicioString}\n" +
                   $" Matricula: {Matricula?.Matricula}                                             Hora Fin: {horaFinString}\n" +
                   $" Fecha: {fechaInicioString}                                              Duración: {duracionString} hrs.\n"+
                   $"\n Descripción: {Descripcion}\n";
        }
        
    }
}
