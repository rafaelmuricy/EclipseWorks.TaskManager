namespace EclipseWorks.TaskManager.Servico.Model
{
    public class ProjetoModel
    {
        public int IdProjeto { get; set; }
        public string Nome { get; set; } = string.Empty;
        public List<TarefaModel>? Tarefas { get; set; }
    }
}
