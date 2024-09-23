using DsiVendas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DsiVendas.Controllers;
    public class VendasController(ApplicationDbContext context) : Controller
    {
    public IActionResult Index()
    {
        var listaVendas = Api();
        return View(listaVendas);
    }
    public List<Venda> Api()
    {
    var listaVendas = context.Vendas
    .Include(v => v.Cliente)
    .Include(v => v.ItensVenda)
        .ThenInclude(i => i.Produto) // Inclui os produtos nos itens
    .ToList();
    return listaVendas;
    }
        // GET: Criação de Venda
        public IActionResult Criar()
        {
            var ListaFormaPagamento = new List<string>();
            ListaFormaPagamento.Add("Cartão de Débito");
            ListaFormaPagamento.Add("Cartão de Crédito");
            ListaFormaPagamento.Add("Boleto");
            ListaFormaPagamento.Add("PIX");
            ViewBag.Clientes = new SelectList(context.Clientes, "Id", "Nome");
            ViewBag.Produtos = new SelectList(context.Produtos, "Id", "Nome");
            ViewBag.FormaPagamentos = new SelectList(ListaFormaPagamento);
            return View();
        }

        [HttpGet]
        public JsonResult GetPrecoProduto(int idProduto)
        {
            var produto = context.Produtos.FirstOrDefault(p => p.Id == idProduto);
            if (produto != null)
            {
                return Json(produto.Preco);
            }
            return Json(0);
        }

        // POST: Salvar a Venda e seus itens
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(Venda venda, List<ItemVenda> itensVenda)
        {
            context.Add(venda);
            await context.SaveChangesAsync();
            foreach (var item in itensVenda)
            {
                item.VendaId = venda.Id;
                item.PrecoUnitario = context.Produtos.Find(item.ProdutoId).Preco;
                context.ItemsVenda.Add(item);
            }
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


            ViewBag.Clientes = new SelectList(context.Clientes, "Id", "Nome", venda.Id);
            ViewBag.Produtos = new SelectList(context.Produtos, "Id", "Nome");
            return View(venda);
        }
        
        //get de remover
        public IActionResult Remover(int id)
        {
            var venda = context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.ItensVenda)
                .ThenInclude(i => i.Produto) 
            .FirstOrDefault(v => v.Id == id);
            if (venda == null)
            {
            return NotFound();
            }
            return View(venda);
        }
        //post de remover
        [HttpPost]
        public IActionResult Remover(Venda venda)
        {
            var vendaExistente = context.Vendas.Find(venda.Id); 
            if (vendaExistente == null)
            {
                return NotFound();
            }
            
            context.Vendas.Remove(vendaExistente); 
            context.SaveChanges(); 
            return RedirectToAction("Index");

        }
        // GET: Edição de Venda
        public IActionResult Editar(int id)
        {
            var venda = context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.ItensVenda)
                    .ThenInclude(i => i.Produto) 
                .FirstOrDefault(v => v.Id == id);

            if (venda == null)
            {
                return NotFound();
            }

            // Passando dados necessários para a View
            ViewBag.Produtos = new SelectList(context.Produtos, "Id", "Nome", venda.ItensVenda);
            ViewBag.Clientes = new SelectList(context.Clientes, "Id", "Nome", venda.ClienteId);
            ViewBag.FormaPagamentos = new SelectList(new List<string>
            {
                "Cartão de Débito",
                "Cartão de Crédito",
                "Boleto",
                "PIX"
            }, venda.FormaPagamento);
            
            return View(venda);
        }

        // POST: Salvar a edição da Venda
       [HttpPost]
    public IActionResult Editar(Venda venda, List<ItemVenda> itensVenda)
    {
        var vendaExistente = context.Vendas
        .Include(v => v.Cliente) // Inclui o cliente para garantir que estamos trabalhando com a entidade correta
        .Include(v => v.ItensVenda) // Inclui os itens da venda
        .FirstOrDefault(v => v.Id == venda.Id);        
        if (vendaExistente == null)
        {
            return NotFound();
        }
        vendaExistente.ClienteId = venda.ClienteId;
        vendaExistente.DataVenda = venda.DataVenda;
        vendaExistente.FormaPagamento = venda.FormaPagamento;
        var itensExistentes = vendaExistente.ItensVenda.ToList(); 
        context.ItemsVenda.RemoveRange(itensExistentes.Where(i => !itensVenda.Any(iv => iv.Id == i.Id)));
         // Atualiza ou adiciona os itens
    foreach (var item in itensVenda)
    {
        var itemExistente = itensExistentes.FirstOrDefault(i => i.Id == item.Id);
        if (itemExistente != null)
        {
            itemExistente.Quantidade = item.Quantidade;
            itemExistente.PrecoUnitario = context.Produtos.Find(item.ProdutoId)?.Preco ?? itemExistente.PrecoUnitario;
            context.ItemsVenda.Update(itemExistente);
        }
        else
        {
            item.VendaId = vendaExistente.Id;
            item.PrecoUnitario = context.Produtos.Find(item.ProdutoId)?.Preco ?? item.PrecoUnitario;
            context.ItemsVenda.Add(item);
        }
    }
        context.Vendas.Update(vendaExistente); 
        context.SaveChanges(); 
        return RedirectToAction("Index");

    }
}