using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Diagnosis.Clases 
{
    public class Camion : ObservableObject
{
        private int id;
        private string matricula;
        private string marca;
        private string modelo;
        private Cliente? idCliente;

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
        public string Matricula
        {
            get { return matricula; }
            set
            {
                if (matricula != value)
                {
                    SetProperty(ref matricula, value);
                }                
            }
        }
        [JsonProperty("marca")]
        public string Marca
        {
            get { return marca; }
            set
            {
                if (marca != value)
                {
                    SetProperty(ref marca, value);
                }
            }
        }
        [JsonProperty("modelo")]
        public string Modelo
        {
            get { return modelo; }
            set
            {
                if (modelo != value)
                {
                    SetProperty(ref modelo, value);
                }
            }
        }

        [JsonProperty("clienteId")]
        public Cliente? IdCliente
        {
            get { return idCliente; }
            set
            {
                if (idCliente != value)
                {
                    SetProperty(ref idCliente, value);
                }
            }
        }

        public Camion() { }

        public Camion(int id, string matricula, string marca, string modelo, Cliente idCliente)
        {
            this.id = id;
            this.matricula = matricula;
            this.marca = marca;
            this.modelo = modelo;
            this.idCliente = idCliente;

        }

        public override string ToString()
        {
            return $"ID: {Id}\nMatricula: {Matricula}\nMarca: {Marca}\nModelo: {Modelo}\nCliente: {IdCliente}";
        }


    }

}
