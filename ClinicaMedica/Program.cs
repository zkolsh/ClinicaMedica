using Microsoft.EntityFrameworkCore;

namespace ClinicaMedica {
    internal class Program {
        static void Main(string[] args) {
            var context = new ClinicaContext();

            Console.Write("Ingrese su DNI: ");
            int dni = int.Parse(Console.ReadLine()!);

            var paciente = context.Pacientes.Find(dni) ?? RegistrarPaciente(context, dni);
            Console.WriteLine($"Hola, {paciente.Nombre} {paciente.Apellido}.");

            var commands = new List<(string Description, Action Callback)>{
                    ("Registrar turno", () => RegistrarTurno(context, dni)),
                    ("Ver turnos", () => VerTurnos(context, dni)),
                    ("Cancelar turno reservado", () => CancelarTurnoInteractive(context, dni)),
            };

            Menu.RunCommand("Clinica médica - menú principal", commands);

            // Por si faltó algo.
            context.SaveChanges();
            Console.WriteLine("Datos guardados en la base de datos");
        }

        // Toma el DNI porque ya lo deberíamos tener.
        static Paciente RegistrarPaciente(ClinicaContext context, int dni) {
                Paciente p = new Paciente();

                Console.WriteLine("Registrando nuevo paciente.");
                Console.Write("Ingrese su nombre: ");
                p.Nombre = Console.ReadLine()!;
                Console.Write("Ingrese su apellido: ");
                p.Apellido = Console.ReadLine()!;
                Console.Write("Ingrese su teléfono: ");
                p.Telefono = Console.ReadLine()!;
                Console.Write("Ingrese su e-mail: ");
                p.Email = Console.ReadLine()!;
                Console.Write("Ingrese su fecha de nacimiento: ");
                p.FechaNacimiento = Console.ReadLine()!;

                context.Pacientes.Add(p);
                Console.WriteLine("Nuevo paciente registrado.");
                return p;
        }

        static void VerTurnos(ClinicaContext context, int dni) {
            var turnos = context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .Include(t => t.Especialidad)
                .Include(t => t.Estado)
                .Where(t => t.Dni == dni)
                .OrderBy(t => t.Fecha)
                .ThenBy(t => t.Hora)
                .ToList();

            if (turnos.Count == 0) {
                    Console.WriteLine("No tienes turnos registrados.");
                    return;
            }

            Console.WriteLine($"Tienes {turnos.Count} turnos registrados.");
            foreach (var t in turnos) {
                    Console.WriteLine(t.MostrarDatos("  "));
            }
        }

        static void CancelarTurnoInteractive(ClinicaContext context, int dni) {
            var turnosReservados = context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .Include(t => t.Especialidad)
                .Include(t => t.Estado)
                .Where(t => t.Dni == dni && t.Estado.Descripcion == "reservado")
                .OrderBy(t => t.Fecha)
                .ThenBy(t => t.Hora)
                .ToList();

            if (turnosReservados.Count == 0) {
                    Console.WriteLine("No tienes turnos reservados.");
                    return;
            }

            Console.WriteLine($"Tienes {turnosReservados.Count} turnos reservados.");
            Turno t = Menu.Choose("Selecciona un turno para cancelar", turnosReservados,
                t => t.MostrarDatos("\t"));

            int idCancelado = context.Estados.Where(e => e.Descripcion == "cancelado").Select(e => e.IdEstado).FirstOrDefault();
            if (idCancelado != 0) {
                    t.IdEstado = idCancelado;
                    context.SaveChanges();
                    Console.WriteLine("Turno cancelado exitosamente");
            } else {
                    Console.WriteLine("!! No se pudo modificar el turno.");
            }
        }

        static void RegistrarTurno(ClinicaContext context, int dni) {
                Console.WriteLine("Registrando un nuevo turno.");

                var especialidades = context.Especialidades.ToList();
                if (!especialidades.Any()) {
                        Console.WriteLine("No hay médicos disponibles.");
                        return;
                }

                Especialidad e = Menu.Choose("Selección de especialidad", especialidades, e => e.Nombre);

                var idMedicos = context.Disponibilidades
                        .Where(d => d.IdEspecialidad == e.IdEspecialidad)
                        .Select(d => d.Matricula)
                        .Distinct()
                        .ToList();

                var medicos = context.Medicos
                        .Where(m => idMedicos.Contains(m.Matricula) && m.Activo != 0)
                        .ToList();
                if (!medicos.Any()) {
                        Console.WriteLine("No hay médicos activos disponibles para esta especialidad.");
                        return;
                }

                Medico m = Menu.Choose("Selección de médico", medicos, m => m.Nombre + " " + m.Apellido);
                var disponibilidades = context.Disponibilidades
                        .Where(d => d.Matricula == m.Matricula && d.IdEspecialidad == e.IdEspecialidad)
                        .ToList();

                Console.WriteLine($"Horarios disponibles para {m.Apellido}:");
                disponibilidades.ForEach(d => Console.WriteLine($" * {d.ToReadableString()}"));

                string fecha = Menu.ReadFutureDate("Ingrese la fecha del turno", disponibilidades);
                string hora = Menu.ReadValidTime("Ingrese la hora del turno", fecha, disponibilidades);

                Estado estado = context.Estados.Where(e => e.Descripcion.ToLower() == "reservado").FirstOrDefault()!;
                Turno t = new Turno {
                        Dni = dni,
                        Matricula = m.Matricula,
                        IdEspecialidad = e.IdEspecialidad,
                        Fecha = fecha,
                        Hora = hora,
                        IdEstado = estado.IdEstado,
                        Observaciones = null,

                        Medico = m,
                        Especialidad = e,
                        Estado = estado
                };

                Console.WriteLine("Configuración final del turno:");
                Console.WriteLine(t.MostrarDatos(">>> "));

                if (Menu.ConfirmPositive($"¿Confirmar turno el {fecha} a las {hora}?")) {
                        context.Turnos.Add(t);
                        context.SaveChanges();
                        Console.WriteLine("Turno registrado exitosamente.");
                } else {
                        Console.WriteLine("Operación cancelada.");
                }
        }
    }
}
