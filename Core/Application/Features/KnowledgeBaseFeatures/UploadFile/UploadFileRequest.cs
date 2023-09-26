using MediatR;
using Realchat.Application.Dto;

namespace Realchat.Application.Features.KnowledgeBaseFeatures.UploadFile;

public sealed record UploadFileRequest(Guid KnowledgeBaseId, string FileName, Stream FileStream) : IRequest<Response>;