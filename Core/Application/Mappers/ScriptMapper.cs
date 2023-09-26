using AutoMapper;
using Realchat.Application.Features.ScriptFeatures.CreateScript;
using Realchat.Domain.Entities;

namespace Realchat.Application.Mappers;

public sealed class ScriptMapper : Profile
{
    public ScriptMapper()
    {
        CreateMap<CreateScriptRequest, Script>()
            .ForMember(dest => dest.Action, src => src.MapFrom(source => source.Action.SerializedSay));
    }
}