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
