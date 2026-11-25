namespace CitasMedicas.Api.Models;

public class Paciente
{
    public int Id { get; }
    public string Nombre { get; }
    public string Dni { get; }
    public int Edad { get; }

    public Paciente(int id, string nombre, string dni, int edad)
    {
        Id = id;
        Nombre = nombre;
        Dni = dni;
        Edad = edad;
    }
}
