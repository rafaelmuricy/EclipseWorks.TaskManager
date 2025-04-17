using EclipseWorks.TaskManager.Servico.Model;

namespace EclipseWorks.TaskManager.Servico.Servico;

public class ProjetoServico
{
    BaseDB baseDB;
    public ProjetoServico(BaseDB baseDB)
    {
        this.baseDB = baseDB;
    }

    public List<ProjetoModel> Listar(int idUsuario)
    {
        string query = "SELECT * FROM projeto where IdUsuario = @IdUsuario";
        var parametros = new Dictionary<string, object>
        {
            { "@IdUsuario", idUsuario }
        };
        var retorno = baseDB.ExecuteQuery<ProjetoModel>(query, parametros);
        return retorno;
    }

    public Result<ProjetoModel> Criar(ProjetoModel projeto)
    {
        var camposVazios = BaseDB.ValidaCampos(projeto);

        if (camposVazios.Count > 0)
        {
            return Result<ProjetoModel>.ValidationError(camposVazios);
        }

        string query = "INSERT INTO projeto (nome) VALUES (@Nome) returning id";
        var parametros = new Dictionary<string, object>
        {
            { "@Nome", projeto.Nome }
        };
        var retorno = baseDB.ExecuteScalar(query, parametros);

        if (retorno != null)
        {
            projeto.Id = (int)retorno;
            return projeto;
        }
        else
        {
            return Result<ProjetoModel>.Error("Erro ao criar projeto.");
        }
    }

    public Result<bool> Remover(int id)
    {
        var tarefaServico = new TarefaServico(baseDB);
        var tarefasPendentes = tarefaServico.ListarPendentes(id);

        if (tarefasPendentes.Any())
        {
            return Result<bool>.Error("Existem tarefas pendentes associadas a este projeto.");
        }

        string query = "DELETE FROM projeto WHERE id = @Id";
        var parametros = new Dictionary<string, object>
        {
            { "@Id", id }
        };
        baseDB.ExecuteQuery(query, parametros);

        return true;
    }
}
