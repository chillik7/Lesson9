using System;
using System.IO;
using System.Text;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }

    public Transaction(int id, decimal amount)
    {
        Id = id;
        Amount = amount;
    }

    public string ToFileString()
    {
        return $"{Id}|{Amount}";
    }

    public static Transaction FromFileString(string data)
    {
        var parts = data.Split('|');
        if (parts.Length != 2)
            throw new FormatException("Некорректный формат данных транзакции");

        return new Transaction(
            id: int.Parse(parts[0]),
            amount: decimal.Parse(parts[1]));
    }
}

public class TransactionFileWriter
{
    private readonly string _filePath;

    public TransactionFileWriter(string filePath)
    {
        _filePath = filePath;

        if (!File.Exists(_filePath))
        {
            File.Create(_filePath).Close();
        }
    }

    public void AppendTransaction(Transaction transaction)
    {
        try
        {
         
            using (var writer = new StreamWriter(_filePath, true, Encoding.UTF8))
            {
                writer.WriteLine(transaction.ToFileString());
            }

            Console.WriteLine($"Транзакция {transaction.Id} успешно добавлена в файл.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении транзакции: {ex.Message}");
        }
    }

    public void DisplayAllTransactions()
    {
        try
        {
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
            {
                Console.WriteLine("Файл транзакций пуст.");
                return;
            }

            Console.WriteLine("\nСписок всех транзакций:");
            using (var reader = new StreamReader(_filePath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        var transaction = Transaction.FromFileString(line);
                        Console.WriteLine($"ID: {transaction.Id}, Сумма: {transaction.Amount}");
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"Ошибка формата данных: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {

        string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "transactions.data");

        var writer = new TransactionFileWriter(filePath);

        writer.AppendTransaction(new Transaction(1, 100.50m));
        writer.AppendTransaction(new Transaction(2, 250.75m));
        writer.AppendTransaction(new Transaction(3, 99.99m));

        writer.DisplayAllTransactions();

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}
