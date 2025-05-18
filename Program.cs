using System;
using System.Collections.Generic;
using System.Data.SQLite;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public bool IsIssued { get; set; }
}

public class LibraryDatabase
{
    private const string connectionString = "Data Source=library.db";

    public static void Initialize()
    {
        using var conn = new SQLiteConnection(connectionString);
        conn.Open();
        string sql = "CREATE TABLE IF NOT EXISTS Books (Id INTEGER PRIMARY KEY AUTOINCREMENT, Title TEXT, Author TEXT, IsIssued INTEGER)";
        new SQLiteCommand(sql, conn).ExecuteNonQuery();
    }

    public static void AddBook(Book book)
    {
        using var conn = new SQLiteConnection(connectionString);
        conn.Open();
        var cmd = new SQLiteCommand("INSERT INTO Books (Title, Author, IsIssued) VALUES (@Title, @Author, 0)", conn);
        cmd.Parameters.AddWithValue("@Title", book.Title);
        cmd.Parameters.AddWithValue("@Author", book.Author);
        cmd.ExecuteNonQuery();
    }

    public static void IssueBook(int bookId)
    {
        using var conn = new SQLiteConnection(connectionString);
        conn.Open();
        var cmd = new SQLiteCommand("UPDATE Books SET IsIssued = 1 WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", bookId);
        cmd.ExecuteNonQuery();
    }

    public static List<Book> GetAllBooks()
    {
        var books = new List<Book>();
        using var conn = new SQLiteConnection(connectionString);
        conn.Open();
        var cmd = new SQLiteCommand("SELECT * FROM Books", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            books.Add(new Book
            {
                Id = Convert.ToInt32(reader["Id"]),
                Title = reader["Title"].ToString(),
                Author = reader["Author"].ToString(),
                IsIssued = Convert.ToInt32(reader["IsIssued"]) == 1
            });
        }
        return books;
    }
}

public class Program
{
    public static void Main()
    {
        LibraryDatabase.Initialize();

        LibraryDatabase.AddBook(new Book { Title = "C# Basics", Author = "John Doe" });
        LibraryDatabase.IssueBook(1);

        var books = LibraryDatabase.GetAllBooks();
        foreach (var book in books)
        {
            Console.WriteLine($"ID: {book.Id}, Title: {book.Title}, Author: {book.Author}, Issued: {book.IsIssued}");
        }
    }
}
