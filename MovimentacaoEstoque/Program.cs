using System.Text.Json;

public class Produto
{
    public int codigoProduto { get; set; }
    public string descricaoProduto { get; set; }
    public int estoque { get; set; }
}

public class EstoqueRoot
{
    public List<Produto> estoque { get; set; }
}

public class Movimentacao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int CodigoProduto { get; set; }
    public string Descricao { get; set; }
    public int Quantidade { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
}

class Program
{
    static void Main()
    {
        ///*----------> *json com as vendas em arvivo TXT* <----------------*/
        //string caminho = @"C:\Users\graeb\Downloads\Estoque.txt";/*adicione o caminho com seu json formatado*/
        //if (!File.Exists(caminho))
        //{
        //    Console.WriteLine("Arquivo não encontrado: " + caminho);
        //    return;
        //}
        ////Lê o JSON
        //string json = File.ReadAllText(caminho);

        // Lê o arquivo JSON na raiz do projeto
        string json = File.ReadAllText("estoque.json");/*se utilizar o codigo acima comente essa linha*/
        var dados = JsonSerializer.Deserialize<EstoqueRoot>(json);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("------> PRODUTOS DISPONÍVEIS <------\n");

            // Lista produtos
            foreach (var p in dados.estoque)
            {
                Console.WriteLine($"Código: {p.codigoProduto} | Produto: {p.descricaoProduto} | Estoque: {p.estoque}");
            }

            Console.WriteLine("\n----------------------\n");

            Console.Write("Informe o código do produto a movimentar (ou digite 0 para sair): ");
            if (!int.TryParse(Console.ReadLine(), out int codigo) || codigo == 0)
            {
                Console.WriteLine("\nEncerrando o sistema...");
                break;
            }

            var produto = dados.estoque.FirstOrDefault(x => x.codigoProduto == codigo);

            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado!");
                Console.ReadKey();
                continue;
            }

            Console.Write("Movimentação (E = Entrada | S = Saída): ");
            string tipo = Console.ReadLine().ToUpper();

            // 🔥 Validação para aceitar apenas E ou S
            while (tipo != "E" && tipo != "S")
            {
                Console.WriteLine("Opção inválida! Digite apenas 'E' para Entrada ou 'S' para Saída.");
                Console.Write("Movimentação (E = Entrada | S = Saída): ");
                tipo = Console.ReadLine().ToUpper();
            }

            int qtd = 0;
            bool quantidadeValida = false;

            while (!quantidadeValida)
            {
                Console.Write("Quantidade: ");

                // Valida entrada numérica
                if (!int.TryParse(Console.ReadLine(), out qtd) || qtd <= 0)
                {
                    Console.WriteLine("Valor inválido! Digite um número inteiro maior que zero.");
                    continue;
                }

                // Valida estoque negativo
                if (tipo == "S" && qtd > produto.estoque)
                {
                    Console.WriteLine($"A quantidade informada ({qtd}) maior que estoque disponível ({produto.estoque}).");
                    Console.WriteLine("Digite uma nova quantidade.");
                    continue;
                }

                quantidadeValida = true;
            }

            Console.Write("Descrição da movimentação: ");
            string descricaoMov = Console.ReadLine();

            // Movimenta
            var mov = new Movimentacao
            {
                CodigoProduto = produto.codigoProduto,
                Descricao = descricaoMov,
                Quantidade = tipo == "S" ? -qtd : qtd
            };

            // Aplica a movimentação no estoque
            produto.estoque += mov.Quantidade;

            Console.WriteLine("\n----> MOVIMENTAÇÃO REGISTRADA <-----");
            Console.WriteLine($"ID: {mov.Id}");
            Console.WriteLine($"Produto: {produto.descricaoProduto}");
            Console.WriteLine($"Tipo: {(mov.Quantidade > 0 ? "Entrada" : "Saída")}");
            Console.WriteLine($"Quantidade: {Math.Abs(mov.Quantidade)}");
            Console.WriteLine($"Descrição: {mov.Descricao}");
            Console.WriteLine($"Data: {mov.Data}");
            Console.WriteLine($"Estoque final: {produto.estoque}");
            Console.WriteLine("-----------------------------------");

            Console.Write("\nDeseja movimentar outro produto? (S = Sim | N = Não): ");
            string opc = Console.ReadLine().ToUpper();

            if (opc == "N")
            {
                Console.WriteLine("\nEncerrando o sistema...");
                break;
            }
        }
    }
}