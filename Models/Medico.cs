namespace CitasMedicas.Api.Models;

public class Medico
{
    public int Id { get; }
    public string Nombre { get; }
    public string Especialidad { get; }
    public string Colegiatura { get; }

    public Medico(int id, string nombre, string especialidad, string colegiatura)
    {
        Id = id;
        Nombre = nombre;
        Especialidad = especialidad;
        Colegiatura = colegiatura;
    }
}
