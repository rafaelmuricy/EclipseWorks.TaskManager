namespace EclipseWorks.TaskManager.Servico.Model;

public class ProjetoModel
{
    [IgnorarValidacao]
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
}
