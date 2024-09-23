using Microsoft.AspNetCore.Mvc;
using DsiVendas.Models;

namespace DsiVendas.Controllers;

public class FornecedoresController(ApplicationDbContext context) : Controller
{
  public IActionResult Index()
  {
    var listaFornecedores = context.Fornecedores.ToList();
    return View(listaFornecedores);
  }

  public List<Fornecedor> Api()
  {
    var listaFornecedores = context.Fornecedores.ToList();
    return listaFornecedores;
  }

  public IActionResult Criar()
  {
    return View();
  }

  [HttpPost]
  public IActionResult Criar(Fornecedor fornecedor)
  {
    context.Fornecedores.Add(fornecedor);
    context.SaveChanges();  
    
    return RedirectToAction("Index");
  }

  public IActionResult Editar(int id)
  {
    var fornecedor = context.Fornecedores.Find(id);
    if (fornecedor == null)
    {
      return NotFound();
    }
    return View(fornecedor);
  }

  [HttpPost]
  public IActionResult Editar(Fornecedor fornecedor)
  {
      var fornecedorExistente = context.Fornecedores.Find(fornecedor.Id); 
      if (fornecedorExistente == null)
      {
        return NotFound();
      }
      fornecedorExistente.Nome = fornecedor.Nome;
      fornecedorExistente.Cidade = fornecedor.Cidade;
      fornecedorExistente.Telefone = fornecedor.Telefone;
      context.Fornecedores.Update(fornecedorExistente); 
      context.SaveChanges(); 
      return RedirectToAction("Index");

  }
  public IActionResult Remover(int id)
  {
    var fornecedor = context.Fornecedores.Find(id);
    if (fornecedor == null)
    {
      return NotFound();
    }
    return View(fornecedor);
  }
  [HttpPost]
  public IActionResult Remover(Fornecedor fornecedor)
  {
      var fornecedorExistente = context.Fornecedores.Find(fornecedor.Id); 
      if (fornecedorExistente == null)
      {
        return NotFound();
      }
      
      context.Fornecedores.Remove(fornecedorExistente); 
      context.SaveChanges(); 
      return RedirectToAction("Index");

  }
}
