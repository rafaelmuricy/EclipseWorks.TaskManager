using EclipseWorks.TaskManager.Servico.Model;
using EclipseWorks.TaskManager.Servico.Servico;
using Microsoft.AspNetCore.Mvc;

namespace EclipseWorks.TaskManager.Controllers;
public class TarefaController : ControllerBase
{
    BaseDB baseDB;

    public TarefaController(BaseDB baseDB)
    {
        this.baseDB = baseDB;
    }


    [HttpGet("{idProjeto}")]
    public List<TarefaModel> Listar(int idProjeto)
    {
        var tarefaServico = new TarefaServico(baseDB);
        var retorno = tarefaServico.Listar(idProjeto);

        return retorno;
    }

    [HttpPost]
    public Result<TarefaModel> Criar([FromBody] TarefaModel tarefa)
    {
        var tarefaServico = new TarefaServico(baseDB);
        var retorno = tarefaServico.Criar(tarefa);
        if (retorno.ErrorMessage.IsNullOrEmpty())
        {
            Response.StatusCode = 201; // Created
        }
        else
        {
            Response.StatusCode = 400; // Bad Request
        }
        return retorno;
    }

    [HttpPut]
    public Result<TarefaModel> Alterar([FromBody] TarefaModel tarefa, [FromBody] string Comentario)
    {
        var tarefaServico = new TarefaServico(baseDB);
        var retorno = tarefaServico.Alterar(tarefa, Comentario);
        if (retorno.ErrorMessage.IsNullOrEmpty())
        {
            Response.StatusCode = 200; // OK
        }
        else
        {
            Response.StatusCode = 400; // Bad Request
        }
        return retorno;
    }

    [HttpDelete("{id}")]
    public Result<bool> Remover(int id)
    {
        var tarefaServico = new TarefaServico(baseDB);
        tarefaServico.Remover(id);
        Response.StatusCode = 200; // OK
        return true;
    }
}
