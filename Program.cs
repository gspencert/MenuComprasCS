using System;
using System.IO;
using System.Collections.Generic;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public string Categoria { get; set; }
}

public class ItemCarrinho
{
    public Produto Produto { get; set; }
    public int Quantidade { get; set; }
}

class Program
{
    static void Main()
    {
        string caminhoCsv = "produtos.csv";
        List<Produto> produtos = CarregarProdutos(caminhoCsv);
        List<ItemCarrinho> carrinho = new List<ItemCarrinho>();

        Menu(caminhoCsv, produtos, carrinho);
    }

    static List<Produto> CarregarProdutos(string caminho)
    {
        var produtos = new List<Produto>();

        if (!File.Exists(caminho))
        {
            File.Create(caminho).Close();
            return produtos;
        }

        foreach (var linha in File.ReadAllLines(caminho))
        {
            var campos = linha.Split(',');

            if (campos.Length != 4) continue;

            produtos.Add(new Produto
            {
                Id = int.Parse(campos[0]),
                Nome = campos[1],
                Preco = decimal.Parse(campos[2]),
                Categoria = campos[3]
            });
        }

        return produtos;
    }

    static void SalvarProdutos(string caminho, List<Produto> produtos)
    {
        using var writer = new StreamWriter(caminho);

        foreach (var p in produtos)
        {
            writer.WriteLine($"{p.Id},{p.Nome},{p.Preco},{p.Categoria}");
        }
    }

    static void ListarProdutos(List<Produto> produtos)
    {
        Console.Clear();
        Console.WriteLine("=== PRODUTOS ===");

        if (produtos.Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado.");
        }
        else
        {
            foreach (var p in produtos)
            {
                Console.WriteLine($"ID: {p.Id} | {p.Nome} | R$ {p.Preco} | {p.Categoria}");
            }
        }

        Pause();
    }

    static void AdicionarProduto(List<Produto> produtos)
    {
        Console.Clear();
        Console.WriteLine("=== ADICIONAR PRODUTO ===");

        Produto prod = new Produto();

        prod.Id = produtos.Count == 0 ? 1 : produtos[^1].Id + 1;

        Console.Write("Nome: ");
        prod.Nome = Console.ReadLine()!;

        Console.Write("Preço: ");
        prod.Preco = decimal.Parse(Console.ReadLine()!);

        Console.Write("Categoria: ");
        prod.Categoria = Console.ReadLine()!;

        produtos.Add(prod);

        Console.WriteLine("Produto adicionado!");
        Pause();
    }

    static void RemoverProduto(List<Produto> produtos)
    {
        Console.Clear();
        Console.WriteLine("=== REMOVER PRODUTO ===");

        Console.Write("ID do produto: ");
        int id = int.Parse(Console.ReadLine()!);

        var produto = produtos.Find(x => x.Id == id);

        if (produto == null)
        {
            Console.WriteLine("Produto não encontrado.");
        }
        else
        {
            produtos.Remove(produto);
            Console.WriteLine("Produto removido.");
        }

        Pause();
    }

    static void AdicionarAoCarrinho(List<Produto> produtos, List<ItemCarrinho> carrinho)
    {
        Console.Clear();
        Console.WriteLine("=== ADICIONAR AO CARRINHO ===");

        Console.Write("ID do produto: ");
        int id = int.Parse(Console.ReadLine()!);

        var produto = produtos.Find(x => x.Id == id);

        if (produto == null)
        {
            Console.WriteLine("Produto não encontrado.");
            Pause();
            return;
        }

        Console.Write("Quantidade: ");
        int quantidade = int.Parse(Console.ReadLine()!);

        var item = carrinho.Find(i => i.Produto.Id == id);

        if (item == null)
        {
            carrinho.Add(new ItemCarrinho { Produto = produto, Quantidade = quantidade });
        }
        else
        {
            item.Quantidade += quantidade;
        }

        Console.WriteLine("Adicionado ao carrinho!");
        Pause();
    }

    static void VerCarrinho(List<ItemCarrinho> carrinho)
    {
        Console.Clear();
        Console.WriteLine("=== CARRINHO ===");

        if (carrinho.Count == 0)
        {
            Console.WriteLine("Carrinho vazio.");
            Pause();
            return;
        }

        decimal total = 0;

        foreach (var item in carrinho)
        {
            decimal subtotal = item.Produto.Preco * item.Quantidade;
            total += subtotal;
            Console.WriteLine($"{item.Produto.Nome} | Qtd: {item.Quantidade} | Subtotal: R$ {subtotal}");
        }

        Console.WriteLine($"TOTAL: R$ {total}");
        Pause();
    }

    static void FinalizarCompra(List<ItemCarrinho> carrinho)
    {
        Console.Clear();
        Console.WriteLine("=== FINALIZAR ===");

        if (carrinho.Count == 0)
        {
            Console.WriteLine("Carrinho vazio.");
            Pause();
            return;
        }

        decimal total = 0;

        foreach (var item in carrinho)
            total += item.Produto.Preco * item.Quantidade;

        Console.WriteLine($"Total da compra: R$ {total}");

        Console.Write("Informe o valor pago: R$ ");
        decimal.TryParse(Console.ReadLine(), out decimal pago);

        if (pago < total)
        {
            decimal faltando = total - pago;
            Console.WriteLine($"Dinheiro insuficiente. Faltam R$ {faltando}");
            Pause();
            return;
        }

        if (pago == total)
        {
            Console.WriteLine("Pagamento exato! Obrigado pela compra.");
        }
        else
        {
            decimal troco = pago - total;
            Console.WriteLine($"Pagamento efetuado! Seu troco é R$ {troco}");
        }

        carrinho.Clear();
        Pause();
    }

    static void Menu(string caminho, List<Produto> produtos, List<ItemCarrinho> carrinho)
    {
        bool executando = true;

        do
        {
            Console.Clear();
            Console.WriteLine("=== MERCADINHO ===");
            Console.WriteLine("1 - Listar produtos");
            Console.WriteLine("2 - Adicionar produto");
            Console.WriteLine("3 - Remover produto");
            Console.WriteLine("4 - Adicionar ao carrinho");
            Console.WriteLine("5 - Ver carrinho");
            Console.WriteLine("6 - Finalizar compra");
            Console.WriteLine("7 - Sair");
            Console.Write("Opção: ");

            int.TryParse(Console.ReadLine(), out int opcao);

            switch (opcao)
            {
                case 1: ListarProdutos(produtos); break;
                case 2: AdicionarProduto(produtos); break;
                case 3: RemoverProduto(produtos); break;
                case 4: AdicionarAoCarrinho(produtos, carrinho); break;
                case 5: VerCarrinho(carrinho); break;
                case 6: FinalizarCompra(carrinho); break;

                case 7:
                    // antes de sair tu salva o CSV
                    SalvarProdutos(caminho, produtos);
                    executando = false;
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    Pause();
                    break;
            }

        } while (executando);
    }

    static void Pause()
    {
        Console.WriteLine("\nPressione Enter...");
        Console.ReadLine();
    }
}
