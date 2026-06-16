namespace ClinicaMedica
{
    public class Paciente
    {
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string FechaNacimiento { get; set; }

	public void MostrarDatos() {
		Console.WriteLine($"{Nombre} {Apellido}");
		Console.WriteLine($"\tDNI: {Dni}");
		Console.WriteLine($"\tTeléfono: {Telefono}");
		Console.WriteLine($"\tE-mail: {Email}");
		Console.WriteLine($"\tFecha de nacimiento: {FechaNacimiento}");
	}
    }
}
