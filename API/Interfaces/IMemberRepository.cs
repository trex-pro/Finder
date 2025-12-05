using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMemberRepository
{
    void Update(Member member);
    Task<bool> SaveAllAsync();
    Task<PaginatedResults<Member>> GetMembersAsync(MemberParams memberParams);
    Task<Member?> GetMemberByIdAsync(string id);
    Task<Member?> GetMemberForUpdate(string id);
    Task<IReadOnlyList<Photo>> GetPhotosByMemberIdAsync(string memberId);
}
