using System.Linq.Expressions;
using API.DTOs;
using API.Entities;

namespace API.Extensions;

public static class MessageExtensions
{
     public static MessageDto ToDto(this Message message)
     {
          return new MessageDto()
          {
               Id = message.Id,
               SenderId = message.SenderId,
               SenderUserName =  message.Sender.UserName,
               SenderImageUrl =  message.Sender.ImageUrl,
               RecipientId = message.RecipientId,
               RecipientUserName =  message.Recipient.UserName,
               RecipientImageUrl =  message.Recipient.ImageUrl,
               Content = message.Content,
               MessageRead =  message.MessageRead,
               MessageSent =   message.MessageSent
          };
     }

     public static Expression<Func<Message, MessageDto>> ToDtoProjection()
    {
        return message => new MessageDto
        {
            Id = message.Id,
               SenderId = message.SenderId,
               SenderUserName =  message.Sender.UserName,
               SenderImageUrl =  message.Sender.ImageUrl,
               RecipientId = message.RecipientId,
               RecipientUserName =  message.Recipient.UserName,
               RecipientImageUrl =  message.Recipient.ImageUrl,
               Content = message.Content,
               MessageRead =  message.MessageRead,
               MessageSent =   message.MessageSent
        };
    }
}
