using AutoMapper;
using MediatR;
using Realchat.Application.Repositories;

namespace Realchat.Application.Common.Handlers;

public class BaseHandler
{
    public readonly IUnitOfWork UnitOfWork = null!;
    public readonly IMapper Mapper = null!;

    public BaseHandler(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    public BaseHandler(IMapper mapper)
    {
        Mapper = mapper;
    }

    public BaseHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        UnitOfWork = unitOfWork;
        Mapper = mapper;
    }
}