namespace API.DTOs;

public class MessageDto
{
    public required string Id { get; set; }
    public required string Content { get; set; }
    public required string SenderId { get; set; }
    public required string SenderUserName { get; set; }
    public string? SenderImageUrl  { get; set; }
    public required string RecipientId { get; set; }
    public required string RecipientUserName { get; set; }
    public string? RecipientImageUrl  { get; set; }
    public DateTime? MessageRead { get; set; }
    public DateTime MessageSent { get; set; }
}
