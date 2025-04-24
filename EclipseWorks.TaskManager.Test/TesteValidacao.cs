using EclipseWorks.TaskManager.Servico.Model;
using EclipseWorks.TaskManager.Servico.Servico;

namespace EclipseWorks.TaskManager.Test;

[TestClass]
public sealed class TesteValidacao
{
    [TestMethod]
    public void ValidacaoProjeto()
    {
        var projeto = new ProjetoModel();
        var resultadoValidacao = BaseDB.ValidaCampos(projeto);
        Assert.Equals(resultadoValidacao.Count, 2);
    }

    [TestMethod]
    public void ValidacaoTarefa()
    {
        var tarefa = new TarefaModel();
        var resultadoValidacao = BaseDB.ValidaCampos(tarefa);
        Assert.Equals(resultadoValidacao.Count, 7);
    }
}
