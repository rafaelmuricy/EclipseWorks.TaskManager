namespace EclipseWorks.TaskManager.Servico.Model;

public class ProjetoModel
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public List<TarefaModel>? Tarefas { get; set; }
}
