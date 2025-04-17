namespace EclipseWorks.TaskManager.Servico.Model
{
    public class TarefaModel
    {
        [IgnorarValidacao]
        public int Id { get; set; }
        public int IdProjeto { get; set; }
        public int IdUsuario { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataVencimento { get; set; }

        public PrioridadeTarefa? Prioridade { get; set; }
        public StatusTarefa? Status { get; set; }

        [IgnorarValidacao]
        public List<HistoricoModel> Historico { get; set; } = new();
        public enum PrioridadeTarefa
        {
            Baixa,
            Media,
            Alta
        }
        public enum StatusTarefa
        {
            Pendente,
            EmAndamento,
            Concluida
        }
    }
}
