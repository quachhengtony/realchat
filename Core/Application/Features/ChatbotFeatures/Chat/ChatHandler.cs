using System.Text;
using System.Text.Json;
using AutoMapper;
using MediatR;
using Realchat.Application.Common.Handlers;
using Realchat.Application.LLM;
using Realchat.Application.Repositories;
using Realchat.Application.Storage;

namespace Realchat.Application.Features.ChatbotFeatures.Chat;

public sealed class ChatHandler : BaseHandler, IRequestHandler<ChatRequest, ChatResponse>
{
    private readonly IMilvusAdapter _milvusAdapter;
    private readonly IInformationChunkRepository _informationChunkRepository;
    private readonly ILLamaAdapter _lLamaAdapter;
    private readonly IScriptRepository _scriptRepository;

    public ChatHandler(IUnitOfWork unitOfWork, IMapper mapper, IMilvusAdapter milvusAdapter, IInformationChunkRepository informationChunkRepository, IScriptRepository scriptRepository, ILLamaAdapter lLamaAdapter) : base(unitOfWork, mapper)
    {
        _milvusAdapter = milvusAdapter;
        _informationChunkRepository = informationChunkRepository;
        _lLamaAdapter = lLamaAdapter;
        _scriptRepository = scriptRepository;
    }

    public async Task<ChatResponse> Handle(ChatRequest request, CancellationToken cancellationToken)
    {
        if (request.Mode == "Script")
        {
            var scriptId = await _milvusAdapter.SearchScript(request.ChatbotId, request.Text);

            // var script = await _scriptRepository.GetByTriggerTextAndChatbotId(request.Text, Guid.Parse(request.ChatbotId));
            var script = await _scriptRepository.GetById(scriptId, cancellationToken);
            if (script == null)
            {
                return new ChatResponse()
                {
                    Text = "Xin lỗi, tôi không tìm thấy kịch bản phù hợp để trả lời cho câu truy vấn của bạn. Vui lòng thử lại với câu truy vấn khác.ORIGIN:"
                };
            }
            var action = JsonSerializer.Deserialize<string[]>(script.Action);
            if (action == null)
            {
                return new ChatResponse()
                {
                    Text = "Xin lỗi, tôi không tìm thấy kịch bản phù hợp để trả lời cho câu truy vấn của bạn. Vui lòng thử lại với câu truy vấn khác.ORIGIN:"
                };
            }

            string response = string.Empty;
            foreach (var item in action)
            {
                response += item + "<ITEM>";
            }

            return new ChatResponse()
            {
                Text = response
            };
        }
        else
        {
            (Guid knowledgeBaseId, int chunkNumber) = await _milvusAdapter.Search(request.ChatbotId, request.Text);
            if (knowledgeBaseId == Guid.Empty || chunkNumber == 0)
            {
                return new ChatResponse()
                {
                    Text = $"Xin lỗi, tôi không tìm thấy tài liệu phù hợp để trả lời cho câu truy vấn của bạn. Vui lòng thử lại với câu truy vấn khác.ORIGIN:"
                };
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Retrieving information chunks from knowledge base with id {knowledgeBaseId} and chunk number {chunkNumber}.");
            Console.ForegroundColor = ConsoleColor.White;

            StringBuilder informationChunks = new();
            if (chunkNumber <= 1)
            {
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber))?.Content + " ");
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 1))?.Content + " ");
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 2))?.Content + " ");
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 3))?.Content + " ");
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 4))?.Content + " ");
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 5))?.Content + " ");
                // informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 6))?.Content);
                // informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 7))?.Content + " ");
                // informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 8))?.Content + " ");
                // informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 9))?.Content + " ");

            }
            else
            {
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber - 1))?.Content + " ");
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber))?.Content + " ");
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 1))?.Content + " ");
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 2))?.Content + " ");
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 3))?.Content + " ");
                informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 4))?.Content + " ");
                // informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 5))?.Content);
                // informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 6))?.Content + " ");
                // informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 7))?.Content + " ");
                // informationChunks.Append((await _informationChunkRepository.GetByKnowledgeBaseIdAndChunkNumber(knowledgeBaseId, chunkNumber + 8))?.Content + " ");
            }
            var length = informationChunks.ToString().Split(" ").Length;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Information chunks retrieved (length: {length}):");
            Console.WriteLine(informationChunks.ToString());
            Console.ForegroundColor = ConsoleColor.White;
            var response = await _lLamaAdapter.GenerateResponse(informationChunks.ToString(), request.Text);
            if (response == "")
            {
                response = await _lLamaAdapter.GetResponse();
            }
            return new ChatResponse()
            {
                Text = $"{response}ORIGIN:\"... {informationChunks} ...\"."
            };
        }
    }
}