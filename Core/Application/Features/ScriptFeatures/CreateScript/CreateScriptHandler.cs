using System.Text;
using System.Text.Json;
using AutoMapper;
using MediatR;
using Realchat.Application.Common.Handlers;
using Realchat.Application.Dto;
using Realchat.Application.Repositories;
using Realchat.Application.Storage;
using Realchat.Domain.Entities;

namespace Realchat.Application.Features.ScriptFeatures.CreateScript;

public sealed class CreateScriptHandler : BaseHandler, IRequestHandler<CreateScriptRequest, Response>
{
    private readonly IChatbotRepository _chatbotRepository;
    private readonly IScriptRepository _scriptRepository;
    private readonly IMilvusAdapter _milvusAdapter;

    public CreateScriptHandler(IUnitOfWork unitOfWork, IChatbotRepository chatbotRepository, IMilvusAdapter milvusAdapter, IScriptRepository scriptRepository, IMapper mapper) : base(unitOfWork, mapper)
    {
        _chatbotRepository = chatbotRepository;
        _scriptRepository = scriptRepository;
        _milvusAdapter = milvusAdapter;
    }

    public async Task<Response> Handle(CreateScriptRequest request, CancellationToken cancellationToken)
    {
        var script = Mapper.Map<Script>(request);
        script.Id = Guid.NewGuid();
        script.CreatedTime = DateTime.UtcNow;

        var chatbot = await _chatbotRepository.GetById(script.ChatbotId, cancellationToken);

        if (chatbot == null) return new Response(400, "Chatbot with the provided id does not exist.");

        script.OrganizationId = chatbot.OrganizationId;
        script.ScriptTypeId = 1;

        _scriptRepository.Create(script);
        await UnitOfWork.Save(cancellationToken);

        var httpClient = new HttpClient();
        var payload = new
        {
            text = request.TriggerText.ToLower()
        };
        var contentData = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync("http://localhost:9089/vectors", contentData);

        if (response.IsSuccessStatusCode)
        {
            JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (json.RootElement.TryGetProperty("vector", out var value))
            {
                await _milvusAdapter.ImportScriptToMilvus(value.ToString(), script.OrganizationId, script.ChatbotId, script.Id);
            }
        }
        return new Response(200, "Script created successfully.");
    }
}