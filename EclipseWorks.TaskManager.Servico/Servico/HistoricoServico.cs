using EclipseWorks.TaskManager.Servico.Model;

namespace EclipseWorks.TaskManager.Servico.Servico;

internal class HistoricoServico
{
    BaseDB baseDB;
    public HistoricoServico(BaseDB baseDB)
    {
        this.baseDB = baseDB;
    }

    public List<HistoricoModel> Listar(int idTarefa)
    {
        string query = "SELECT * FROM Historico WHERE IdTarefa = @IdTarefa";
        var retorno = baseDB.ExecuteQuery<HistoricoModel>(query, new Dictionary<string, object>
        {
            { "@IdTarefa", idTarefa }
        });
    
        //lista as alterações do histórico
        foreach (var item in retorno)
        {
            string queryAlteracoes = "SELECT * FROM Alteracoes WHERE IdHistorico = @IdHistorico";
            var alteracoes = baseDB.ExecuteQuery<AlteracoesModel>(queryAlteracoes, new Dictionary<string, object>
            {
                { "@IdHistorico", item.Id }
            });
            item.Alteracoes = alteracoes;
        }
        return retorno;
    }

    internal void InserirHistorico(TarefaModel tarefaAnterior, TarefaAlterarModel tarefaAtual)
    {
        string query = @"INSERT INTO Historico (IdTarefa, IdUsuario, Comentario, DataModificacao) 
                                    VALUES (@IdTarefa, @IdUsuario, @Comentario, @DataModificacao) returning Id";
        var parametros = new Dictionary<string, object>
        {
            { "@IdTarefa", tarefaAnterior.Id },
            { "@IdUsuario", tarefaAtual.IdUsuario },
            { "@Comentario", tarefaAtual.Comentario },
            { "@DataModificacao", DateTime.Now }
        };

        var idHistorico = baseDB.ExecuteScalar(query, parametros);

        var alteracoes = ListarAlteracoes(tarefaAnterior, tarefaAtual);

        foreach (var alteracao in alteracoes)
        {
            string queryAlteracao = @"INSERT INTO Alteracoes (IdHistorico, Campo, ValorAntigo, ValorNovo)
                                                VALUES (@IdHistorico, @Campo, @ValorAntigo, @ValorNovo)";
            var parametrosAlteracao = new Dictionary<string, object>
            {
                { "@IdHistorico", idHistorico! },
                { "@Campo", alteracao.Campo },
                { "@ValorAntigo", alteracao.ValorAntigo },
                { "@ValorNovo", alteracao.ValorNovo }
            };

            baseDB.ExecuteQuery(queryAlteracao, parametrosAlteracao);
        }
    }
    private List<AlteracoesModel> ListarAlteracoes(TarefaModel tarefaAnterior, TarefaAlterarModel tarefaAtual)
    {
        List<AlteracoesModel> alteracoes = new();
        if (tarefaAnterior.Titulo != tarefaAtual.Titulo)
        {
            alteracoes.Add(new AlteracoesModel { Campo = "Título", ValorAntigo = tarefaAnterior.Titulo, ValorNovo = tarefaAtual.Titulo });
        }
        if (tarefaAnterior.Descricao != tarefaAtual.Descricao)
        {
            alteracoes.Add(new AlteracoesModel { Campo = "Descrição", ValorAntigo = tarefaAnterior.Descricao, ValorNovo = tarefaAtual.Descricao });
        }
        if (tarefaAnterior.DataVencimento != tarefaAtual.DataVencimento)
        {
            alteracoes.Add(new AlteracoesModel { Campo = "Data de Vencimento", ValorAntigo = tarefaAnterior.DataVencimento.ToString(), ValorNovo = tarefaAtual.DataVencimento.ToString() });
        }
        if (tarefaAnterior.Prioridade != tarefaAtual.Prioridade)
        {
            alteracoes.Add(new AlteracoesModel { Campo = "Prioridade", ValorAntigo = tarefaAnterior.Prioridade.ToString()!, ValorNovo = tarefaAtual.Prioridade!.ToString()! });
        }
        if (tarefaAnterior.Status != tarefaAtual.Status)
        {
            alteracoes.Add(new AlteracoesModel { Campo = "Status", ValorAntigo = tarefaAnterior.Status.ToString()!, ValorNovo = tarefaAtual.Status.ToString()! });
        }
        return alteracoes;
    }
    public void Remover(int idTarefa)
    {
        var historicos = Listar(idTarefa);

        //remove as alteracoes
        foreach (var historico in historicos)
        {
            string queryAlteracoes = "DELETE FROM Alteracoes WHERE IdHistorico = @IdHistorico";
            baseDB.ExecuteNonQuery(queryAlteracoes, new Dictionary<string, object>
            {
                { "@IdHistorico", historico.Id }
            });
        }

        //remove os historicos
        string query = "DELETE FROM Historico WHERE IdTarefa = @IdTarefa";
        baseDB.ExecuteNonQuery(query, new Dictionary<string, object>
        {
            { "@IdTarefa", idTarefa }
        });
    }
    
}
