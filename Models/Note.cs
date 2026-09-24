namespace IsLabApp.Models;

// Сущность "Заметка"
public class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// DTO для создания заметки (то, что приходит от клиента в POST /api/notes)
public class CreateNoteDto
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
