namespace EclipseWorks.TaskManager.Servico.Model
{
    public class TarefaModel
    {
        public int Id { get; set; }
        public int IdProjeto { get; set; }
        public int IdUsuario { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataVencimento { get; set; }

        public PrioridadeTarefa Prioridade { get; set; }
        public StatusTarefa Status { get; set; }

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

    public class TarefaComentario
    {
        public int IdTarefa { get; set; }
        public int IdUsuario { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }
}
