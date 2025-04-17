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
        string query = "SELECT * FROM tarefa where idProjeto = @IdProjeto";
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
        string query = "SELECT * FROM tarefa where idProjeto = @IdProjeto and status = @Status";
        var retorno = baseDB.ExecuteQuery<TarefaModel>(query, new Dictionary<string, object>
        {
            { "@IdProjeto", idProjeto },
            { "@Status", TarefaModel.StatusTarefa.Pendente }
        });
        return retorno;
    }
    public TarefaModel? ObterTarefa(int id)
    {
        string query = "SELECT * FROM tarefa WHERE id = @Id";
        var retorno = baseDB.ExecuteQuery<TarefaModel>(query, new Dictionary<string, object>
        {
            { "@Id", id }
        });

        return retorno.FirstOrDefault();
    }
    public Result<TarefaModel> Criar(TarefaModel tarefa)
    {
        var camposVazios = BaseDB.ValidaCampos(tarefa);

        if (camposVazios.Count > 0)
        {
            return Result<TarefaModel>.ValidationError(camposVazios);
        }

        string query = @"INSERT INTO tarefa (titulo, descricao, idProjeto, idUsuario, dataVencimento, prioridade, status) 
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
            tarefa.Id = (int)retorno;
            return tarefa;
        }
        else
        {
            return Result<TarefaModel>.Error("Erro ao criar tarefa.");
        }
    }
    public Result<TarefaModel> Alterar(TarefaModel tarefa, string comentario)
    {
        var camposVazios = BaseDB.ValidaCampos(tarefa);

        if (camposVazios.Count > 0)
        {
            return Result<TarefaModel>.ValidationError(camposVazios);
        }

        if (comentario != string.Empty)
        {
            return Result<TarefaModel>.Error("Insira um comentário");
        }

        var tarefaAnterior = ObterTarefa(tarefa.Id);

        if (tarefaAnterior == null)
        {
            return Result<TarefaModel>.Error("Tarefa não encontrada.");
        }

        InserirHistorico(tarefaAnterior, tarefa, comentario);

        string query = @"UPDATE tarefa SET 
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
            { "@Status", tarefa.Status }
        };
        baseDB.ExecuteQuery(query, parametros);

        return tarefa;
    }
    public void Remover(int id)
    {
        var historicoServico = new HistoricoServico(baseDB);
        historicoServico.Remover(id);

        string query = "DELETE FROM tarefa WHERE id = @Id";
        var parametros = new Dictionary<string, object>
        {
            { "@Id", id }
        };
        baseDB.ExecuteQuery(query, parametros);


    }
    private void InserirHistorico(TarefaModel tarefaAnterior, TarefaModel tarefaAtual, string comentario)
    {
        string query = @"INSERT INTO Historico (IdTarefa, IdUsuario, Comentario, DataModificacao) 
                                    VALUES (@IdTarefa, @IdUsuario, @Comentario, @DataModificacao) returning Id";
        var parametros = new Dictionary<string, object>
        {
            { "@IdTarefa", tarefaAnterior.Id },
            { "@IdUsuario", tarefaAtual.IdUsuario },
            { "@Comentario", comentario },
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
    private List<AlteracoesModel> ListarAlteracoes(TarefaModel tarefaAnterior, TarefaModel tarefaAtual)
    {
        List<AlteracoesModel> alteracoes = new();
        if (tarefaAnterior.Titulo != tarefaAtual.Titulo)
        {
            alteracoes.Add( new AlteracoesModel { Campo = "Título", ValorAntigo = tarefaAnterior.Titulo, ValorNovo = tarefaAtual.Titulo });
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

}
