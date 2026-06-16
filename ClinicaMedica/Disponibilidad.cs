using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicaMedica
{
    public class Disponibilidad
    {
        public int Matricula { get; set; }
        public int IdEspecialidad { get; set; }
        public int DiaSemana { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }

        public Medico Medico { get; set; }
        public Especialidad Especialidad { get; set; }

        public string ToReadableString() {
		// hdps en C# cómo que si no arranca el día en domingo anda mal
		string[] dias = {"Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"};
        	string nombreDia = (DiaSemana >= 0 && DiaSemana < 7) ? dias[DiaSemana] : $"Día {DiaSemana}";
        	return $"{nombreDia} de {HoraInicio} a {HoraFin}";
        }
    }
}
