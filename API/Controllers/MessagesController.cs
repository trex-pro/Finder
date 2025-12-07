using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MessagesController(IMessageRepository messageRepository,
        IMemberRepository memberRepository) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var sender = await memberRepository.GetMemberByIdAsync(User.GetMemberId());
        var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);
        if (recipient == null || sender == null || sender.Id == recipient.Id)
            return BadRequest("Cannot Send Message.");

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content,
        };
        
        messageRepository.AddMessage(message);

        if (await messageRepository.SaveAllAsync()) return message.ToDto();
        return BadRequest("Failed to Send Message.");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResults<MessageDto>>> GetMessageByContainer([FromQuery] MessageParams messageParams)
    {
        messageParams.MemberId = User.GetMemberId();
        return await messageRepository.GetMessageForMember(messageParams);
    }

    [HttpGet("thread/{recipientId}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageByThread(string recipientId)
    {
        return Ok(await messageRepository.GetMessageThread( User.GetMemberId(), recipientId)); 
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(string id)
    {
        var memberId = User.GetMemberId();
        var message = await messageRepository.GetMessage(id);

        if (message == null)
            return BadRequest("Cannot Send Message.");
        if (message.SenderId != memberId && message.RecipientId != memberId)
            return BadRequest("Cannot Send Message.");

        if (message.SenderId == memberId) message.SenderDeleted = true;
        if (message.RecipientId == memberId) message.RecipientDeleted = true;

        if (message is {SenderDeleted: true, RecipientDeleted: true })
        {
            messageRepository.DeleteMessage(message);     
        }
        if (await memberRepository.SaveAllAsync()) return Ok();
        return BadRequest("Something Went Wrong.");
    }

}