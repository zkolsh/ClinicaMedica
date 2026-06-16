using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaMedica
{
    public class Turno
    {
        public int Dni { get; set; }
        public int Matricula { get; set; }
        public int IdEspecialidad { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int IdEstado { get; set; }
        public string? Observaciones { get; set; }

        public Paciente Paciente { get; set; }
        public Medico Medico { get; set; }
        public Especialidad Especialidad { get; set; }
        public Estado Estado { get; set; }

        public string MostrarDatos(string prefix = "") {
        	string especialidad = Especialidad?.Nombre ?? "(none)";
        	string estado = Estado?.Descripcion ?? "(none)";
        	string nombreMedico = Medico?.Nombre ?? "<desconocido>";
        	string apellidoMedico = Medico?.Apellido ?? "<desconocido>";
        	
        	return $"{prefix}Turno [{estado}]{Environment.NewLine}"
        	     + $"{prefix} - Con [{especialidad}] [{nombreMedico} {apellidoMedico}]{Environment.NewLine}"
        	     + $"{prefix} - Horario {Fecha} {Hora}{Environment.NewLine}"
        	     + (string.IsNullOrWhiteSpace(Observaciones)
        	     ? "" : $"{prefix} - Observaciones: {Observaciones}{Environment.NewLine}");
        }
    }
}
