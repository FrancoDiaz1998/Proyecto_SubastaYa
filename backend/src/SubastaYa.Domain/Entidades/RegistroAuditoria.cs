namespace SubastaYa.Domain.Entidades;

public class RegistroAuditoria
{
    public long Id { get; set; }
    public Guid? UsuarioActorId { get; set; }
    public required string TipoEntidad { get; set; }
    public required string EntidadId { get; set; }
    public required string Accion { get; set; }
    public required string DetallesJson { get; set; }
    public DateTimeOffset Fecha { get; set; }
}
