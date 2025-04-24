using EclipseWorks.TaskManager.Servico.Model;

namespace EclipseWorks.TaskManager.Servico.Servico;

public class TarefaServico
{
    BaseDB baseDB;
    public TarefaServico(BaseDB baseDB)
    {
        this.baseDB = baseDB;
    }
    public List<TarefaModel> Listar(int idProjeto)
    {
        string query = "SELECT * FROM Tarefas where idProjeto = @IdProjeto";
        var retorno = baseDB.ExecuteQuery<TarefaModel>(query, new Dictionary<string, object>
        {
            { "@IdProjeto", idProjeto }
        });

        var historicoServico = new HistoricoServico(baseDB);

        //lista o histórico da tarefa
        foreach (var tarefa in retorno)
        {
            tarefa.Historico = historicoServico.Listar(tarefa.Id);
        }

        return retorno;
    }
    public List<TarefaModel> ListarPendentes(int idProjeto)
    {
        string query = "SELECT * FROM tarefas where idProjeto = @IdProjeto and status = @Status";
        var retorno = baseDB.ExecuteQuery<TarefaModel>(query, new Dictionary<string, object>
        {
            { "@IdProjeto", idProjeto },
            { "@Status", TarefaModel.StatusTarefa.Pendente }
        });
        return retorno;
    }
    public TarefaModel? ObterTarefa(int id)
    {
        string query = "SELECT * FROM tarefas WHERE id = @Id";
        var retorno = baseDB.ExecuteQuery<TarefaModel>(query, new Dictionary<string, object>
        {
            { "@Id", id }
        });

        return retorno.FirstOrDefault();
    }
    public Result<TarefaModel> Criar(TarefaInserirModel tarefa)
    {
        var camposVazios = BaseDB.ValidaCampos(tarefa);

        if (camposVazios.Count > 0)
        {
            return Result<TarefaModel>.ValidationError(camposVazios);
        }

        string query = @"INSERT INTO tarefas (titulo, descricao, idProjeto, idUsuario, dataVencimento, prioridade, status) 
                                    VALUES (@Titulo, @Descricao, @IdProjeto, @IdUsuario, @DataVencimento, @Prioridade, @Status) returning id";
        var parametros = new Dictionary<string, object>
        {
            { "@IdProjeto", tarefa.IdProjeto },
            { "@IdUsuario", tarefa.IdUsuario },
            { "@Titulo", tarefa.Titulo },
            { "@Descricao", tarefa.Descricao },
            { "@DataVencimento", tarefa.DataVencimento },
            { "@Prioridade", tarefa.Prioridade! },
            { "@Status", tarefa.Status! }
        };
        var retorno = baseDB.ExecuteScalar(query, parametros);

        if (retorno != null)
        {
            return ObterTarefa((int)retorno)!;
        }
        else
        {
            return Result<TarefaModel>.Error("Erro ao criar tarefa.");
        }
    }
    public Result<TarefaModel> Alterar(TarefaAlterarModel tarefa)
    {
        var camposVazios = BaseDB.ValidaCampos(tarefa);

        if (camposVazios.Count > 0)
        {
            return Result<TarefaModel>.ValidationError(camposVazios);
        }

        var tarefaAnterior = ObterTarefa(tarefa.Id);

        if (tarefaAnterior == null)
        {
            return Result<TarefaModel>.Error("Tarefa não encontrada.");
        }

        new HistoricoServico(baseDB).InserirHistorico(tarefaAnterior, tarefa);

        string query = @"UPDATE tarefas SET 
                        idUsuario = @IdUsuario, 
                        titulo = @Titulo, 
                        descricao = @Descricao, 
                        dataVencimento = @DataVencimento, 
                        status = @Status 
                        WHERE id = @Id";
        var parametros = new Dictionary<string, object>
        {
            { "@Id", tarefa.Id },
            { "@IdUsuario", tarefa.IdUsuario },
            { "@Titulo", tarefa.Titulo },
            { "@Descricao", tarefa.Descricao },
            { "@DataVencimento", tarefa.DataVencimento },
            { "@Status", tarefa.Status! }
        };
        baseDB.ExecuteQuery(query, parametros);

        return ObterTarefa(tarefa.Id)!;
    }


    public List<RelatorioModel> TarefasConcluidasPorUsuario()
    {
        string query = @"SELECT IdUsuario, COUNT(id) as TarefasConcluidas
                        FROM tarefas
                        WHERE status = @Status
                        AND DataVencimento >= @Data
                        GROUP BY IdUsuario";
        var parametros = new Dictionary<string, object>
        {
            { "@Status", TarefaModel.StatusTarefa.Concluida },
            { "@Data", DateTime.Now.AddDays(-30) }
        };
        var retorno = baseDB.ExecuteQuery<RelatorioModel>(query, parametros);
        return retorno;
    }

    public void Remover(int id)
    {
        var historicoServico = new HistoricoServico(baseDB);
        historicoServico.Remover(id);

        string query = "DELETE FROM tarefas WHERE id = @Id";
        var parametros = new Dictionary<string, object>
        {
            { "@Id", id }
        };
        baseDB.ExecuteQuery(query, parametros);
    }
}
