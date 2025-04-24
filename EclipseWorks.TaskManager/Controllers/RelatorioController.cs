using EclipseWorks.TaskManager.Servico.Model;
using EclipseWorks.TaskManager.Servico.Servico;
using Microsoft.AspNetCore.Mvc;

namespace EclipseWorks.TaskManager.Controllers;
public class RelatorioController : ControllerBase
{
    BaseDB baseDB;
    public RelatorioController(BaseDB baseDB)
    {
        this.baseDB = baseDB;
    }

    [HttpGet]
    public List<RelatorioModel> TarefasPorUsuario()
    {
        var tarefaServico = new TarefaServico(baseDB);
        var retorno = tarefaServico.TarefasConcluidasPorUsuario();

        return retorno;
    }
}
