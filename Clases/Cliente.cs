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
    public class Cliente : ObservableObject
    {
        private int id;
        private string nombre;
        private string cif;
        private string email;
        //No se va a utilizar en la aplicación en esta primera versión.
        private string password;


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

        [JsonProperty("nombre")]
        public string Nombre
        {
            get { return nombre; }
            set
            {
                if (nombre != value)
                {
                    SetProperty(ref nombre, value);
                }
            }
        }

        [JsonProperty("cif")]
        public string Cif
        {
            get { return cif; }
            set
            {
                if (cif != value)
                {
                    SetProperty(ref cif, value);
                }
            }
        }

        [JsonProperty("email")]
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    SetProperty(ref email, value);
                }
            }
        }

        [JsonProperty("password")]
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    SetProperty(ref password, value);
                }
            }
        }


        public Cliente() { }

        
        public Cliente(int id, string nombre, string cif, string email, string password)
        {
            this.id = id;
            this.nombre = nombre;
            this.cif = cif;
            this.email = email;
            this.password = password;
        }

        public override string ToString()
        {
            return $" ID Cliente: {Id}\n Nombre: {Nombre}\n CIF: {Cif}\n Email: {Email}\n";

        }


    }
}
