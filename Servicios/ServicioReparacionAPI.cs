using Diagnosis.Clases;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diagnosis.Servicios
{
    public class ServicioReparacionAPI
    {
        private readonly string baseUrl = "http://localhost:8080/fasttool/datos/reparaciones";

       public async Task<ObservableCollection<Reparacion>> ObtenerReparaciones()
        {
            ObservableCollection<Reparacion> listaReparaciones = new ObservableCollection<Reparacion>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(baseUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var reparacionesApi = JsonConvert.DeserializeObject<List<ReparacionDTO>>(jsonString);
                        foreach (var rep in reparacionesApi)
                        {
                            // Definir el formato de fecha y hora esperado
                            string formatoFechaHora = "yyyy-MM-ddTHH:mm:ssZ'['UTC']'";

                            // Convertir la cadena de fecha y hora al objeto DateTime en UTC
                            DateTime horaInicioUtc = DateTime.ParseExact(rep.HoraInicio, formatoFechaHora, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                            DateTime? horaFinUtc = string.IsNullOrEmpty(rep.HoraFin) ? null : DateTime.ParseExact(rep.HoraFin, formatoFechaHora, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

                            // Crear el objeto Reparacion con las fechas y horas en UTC
                            var reparacion = new Reparacion
                            {
                                Id = rep.Id,
                                Matricula = rep.Matricula,
                                Descripcion = rep.Descripcion,
                                HoraInicio = horaInicioUtc,
                                HoraFin = horaFinUtc
                            };

                            listaReparaciones.Add(reparacion);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Error al obtener las reparaciones: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error: " + ex.Message);
            }

            return listaReparaciones;
        }


        public async Task<ObservableCollection<Reparacion>> ReparacionesPorMatricula(string matricula)
        {
            ObservableCollection<Reparacion> reparacionesPorMatricula = new ObservableCollection<Reparacion>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync($"{baseUrl}/matriculas/{matricula}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        var reparacionesDTO = JsonConvert.DeserializeObject<List<ReparacionDTO>>(jsonString);
                        foreach (var reparacionDTO in reparacionesDTO)
                        {
                            string formatoFechaHora = "yyyy-MM-ddTHH:mm:ssZ'['UTC']'";                           

                            // Convertir la cadena de fecha y hora al objeto DateTime en UTC
                            DateTime horaInicioUtc = DateTime.ParseExact(reparacionDTO.HoraInicio, formatoFechaHora, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                            DateTime? horaFinUtc = string.IsNullOrEmpty(reparacionDTO.HoraFin) ? null : DateTime.ParseExact(reparacionDTO.HoraFin, formatoFechaHora, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

                            var reparacion = new Reparacion
                            {
                                Id = reparacionDTO.Id,
                                Matricula = reparacionDTO.Matricula,
                                Descripcion = reparacionDTO.Descripcion,
                                HoraInicio = horaInicioUtc,
                                HoraFin = horaFinUtc
                            };

                            reparacionesPorMatricula.Add(reparacion);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Error al obtener las reparaciones por matrícula: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error: " + ex.Message);
            }

            return reparacionesPorMatricula;
        }

        public async Task<bool> CrearReparacion(Camion camionEncontrado, string descripcion, string horaInicio = null)
        {
            try
            {
                DateTime horaInicioReparacion;
                if (!string.IsNullOrEmpty(horaInicio))
                {
                    if (DateTime.TryParseExact(horaInicio, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedHoraInicio))
                    {
                        
                        horaInicioReparacion = parsedHoraInicio;
                    }
                    else
                    {
                        throw new ArgumentException("El formato de hora de inicio es inválido.", nameof(horaInicio));
                    }
                }
                else
                {
                    horaInicioReparacion = DateTime.Now;
                }



                var json = JsonConvert.SerializeObject(new
                {
                    descripcion = descripcion,
                    horaInicio = horaInicioReparacion.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    id = 0, // 0 si es una nueva reparación
                    matricula = new
                    {
                        id = camionEncontrado.Id,
                        marca = camionEncontrado.Marca,
                        matricula = camionEncontrado.Matricula,
                        modelo = camionEncontrado.Modelo
                    }
                });

                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(baseUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return true; // La reparación se creó correctamente
                    }
                    else
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        Trace.WriteLine($"Error al crear la reparación: {response.StatusCode} - {responseBody}");
                        return false; // Hubo un error al crear la reparación
                    }
                }
            }
            catch (ArgumentException argEx)
            {
                Trace.WriteLine($"Error de argumento: {argEx.Message}");
                return false;
            }
            catch (HttpRequestException httpEx)
            {
                Trace.WriteLine($"Error de solicitud HTTP: {httpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error inesperado: {ex.Message}");
                return false; // Hubo un error de excepción al crear la reparación
            }
        }



        public async Task<bool> ModificarReparacion(Reparacion reparacion)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var servicioCamionAPI = new ServicioCamionAPI();
                    // Obtener los detalles del camión basado en la matrícula
                    var camion = await servicioCamionAPI.BuscarCamionPorMatricula(reparacion.Matricula.Matricula);

                    // Verificar si se encontró un camión con la matrícula especificada
                    if (camion != null)
                    {
                                         

                        // Construir el objeto JSON con la estructura deseada
                        var json = JsonConvert.SerializeObject(new
                        {
                            descripcion = reparacion.Descripcion,
                            horaInicio = reparacion.HoraInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                            horaFin = reparacion.HoraFin.HasValue ? reparacion.HoraFin.Value.ToString("yyyy-MM-ddTHH:mm:ss") : null, // La hora fin puede ser nula
                            id = reparacion.Id,
                            matricula = new
                            {
                                id = camion.Id,
                                matricula = camion.Matricula,
                                marca = camion.Marca,
                                modelo = camion.Modelo,
                                clienteId = new
                                {
                                    id = camion.IdCliente.Id,
                                    nombre = camion.IdCliente.Nombre,
                                    cif = camion.IdCliente.Cif,
                                    email = camion.IdCliente.Email,
                                    password = camion.IdCliente.Password
                                }
                            }
                        });
                        Trace.WriteLine(json);
                        // Crear una solicitud HTTP PUT con el JSON como contenido
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await httpClient.PutAsync(baseUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            return true; // La reparación se modificó correctamente
                        }
                        else
                        {
                            Trace.WriteLine("Error al modificar la reparación: " + response.StatusCode);
                            return false; // Hubo un error al modificar la reparación
                        }
                    }
                    else
                    {
                        // Manejar el caso en el que no se encontró un camión con la matrícula especificada
                        Trace.WriteLine("No se encontró ningún camión con la matrícula especificada.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error: " + ex.Message);
                return false; // Hubo un error de excepción al modificar la reparación
            }
        }




        public async Task<bool> EliminarReparacion(int reparacionId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.DeleteAsync($"{baseUrl}/{reparacionId}");

                    if (response.IsSuccessStatusCode)
                    {
                        return true; 
                    }
                    else
                    {
                        Trace.WriteLine("Error al eliminar la reparación: " + response.StatusCode);
                        return false; 
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error: " + ex.Message);
                return false; 
            }
        }
    }

    public class ReparacionDTO
    {
        public int Id { get; set; }
        public Camion Matricula { get; set; }
        public string Descripcion { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
    }
    
}




