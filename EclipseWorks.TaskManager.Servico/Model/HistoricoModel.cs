namespace EclipseWorks.TaskManager.Servico.Model;

public class HistoricoModel
{
    public int Id { get; set; }
    public int IdTarefa { get; set; }
    public int IdUsuario { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public DateTime DataModificacao { get; set; }
    public List<AlteracoesModel> Alteracoes { get; set; } = new();
}
