using static EclipseWorks.TaskManager.Servico.Model.TarefaModel;

namespace EclipseWorks.TaskManager.Servico.Model
{
    public class TarefaInserirModel
    {
        public int IdProjeto { get; set; }
        public int IdUsuario { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataVencimento { get; set; }

        public PrioridadeTarefa? Prioridade { get; set; }
        public StatusTarefa? Status { get; set; }

        
    }
}
