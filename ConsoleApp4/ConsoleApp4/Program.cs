using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }

    public Transaction(int id, decimal amount)
    {
        Id = id;
        Amount = amount;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Amount: {Amount}";
    }
}

public class TransactionFileReader
{
    private readonly string _filePath;

    public TransactionFileReader(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public List<Transaction> ReadTransactions()
    {
        var transactions = new List<Transaction>();

        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"Файл {_filePath} не найден");
        }

        try
        {
            using (var reader = new StreamReader(_filePath))
            {
                string line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var parts = line.Split('|');
                        if (parts.Length != 2)
                            throw new FormatException("Неверный формат строки");

                        int id = int.Parse(parts[0]);
                        decimal amount = decimal.Parse(parts[1]);

                        transactions.Add(new Transaction(id, amount));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке строки {lineNumber}: {ex.Message}");
                    }
                }
            }

            return transactions;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            throw;
        }
    }
}

public class TransactionProcessor
{
    private List<Transaction> _transactions;

    public TransactionProcessor(List<Transaction> transactions)
    {
        _transactions = transactions ?? throw new ArgumentNullException(nameof(transactions));
    }

    public List<Transaction> FilterByAmount(decimal minAmount)
    {
        return _transactions
            .Where(t => t.Amount >= minAmount)
            .ToList();
    }

    public List<Transaction> SortByAmount()
    {
        return _transactions
            .OrderBy(t => t.Amount)
            .ToList();
    }

    public List<Transaction> SortByAmountDescending()
    {
        return _transactions
            .OrderByDescending(t => t.Amount)
            .ToList();
    }

    public Transaction FindById(int id)
    {
        return _transactions.FirstOrDefault(t => t.Id == id);
    }

    public List<Transaction> GetAllTransactions()
    {
        return new List<Transaction>(_transactions);
    }
}

class Program
{
    static void Main(string[] args)
    {
     
        string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "transactions.data");

        try
        {
            var reader = new TransactionFileReader(filePath);
            var transactions = reader.ReadTransactions();

            Console.WriteLine($"Прочитано {transactions.Count} транзакций:");
            foreach (var t in transactions)
            {
                Console.WriteLine(t);
            }

            var processor = new TransactionProcessor(transactions);

            var filtered = processor.FilterByAmount(100);
            Console.WriteLine("\nТранзакции с суммой >= 100:");
            foreach (var t in filtered)
            {
                Console.WriteLine(t);
            }
            
            var sorted = processor.SortByAmount();
            Console.WriteLine("\nТранзакции отсортированные по сумме (возрастание):");
            foreach (var t in sorted)
            {
                Console.WriteLine(t);
            }

            Console.WriteLine("\nВведите ID транзакции для поиска:");
            if (int.TryParse(Console.ReadLine(), out int searchId))
            {
                var found = processor.FindById(searchId);
                Console.WriteLine(found != null
                    ? $"Найдена транзакция: {found}"
                    : "Транзакция не найдена");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}
