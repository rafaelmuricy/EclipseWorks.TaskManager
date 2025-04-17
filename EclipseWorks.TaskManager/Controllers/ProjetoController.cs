using EclipseWorks.TaskManager.Servico.Model;
using EclipseWorks.TaskManager.Servico.Servico;
using Microsoft.AspNetCore.Mvc;

namespace EclipseWorks.TaskManager.Controllers;
public class ProjetoController : ControllerBase
{
    BaseDB baseDB;

    public ProjetoController(BaseDB baseDB)
    {
        this.baseDB = baseDB;
    }

    [HttpGet("{idUsuario}")]
    public List<ProjetoModel> Listar(int idUsuario)
    {
        var projetoServico = new ProjetoServico(baseDB);
        return projetoServico.Listar(idUsuario);
    }


    [HttpPost]
    public Result<ProjetoModel> Criar([FromBody] ProjetoModel projeto)
    {
        var projetoServico = new ProjetoServico(baseDB);
        var retorno = projetoServico.Criar(projeto);
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

    [HttpDelete("{id}")]
    public Result<bool> Remover(int id)
    {
        var projetoServico = new ProjetoServico(baseDB);
        var retorno = projetoServico.Remover(id);
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


}
