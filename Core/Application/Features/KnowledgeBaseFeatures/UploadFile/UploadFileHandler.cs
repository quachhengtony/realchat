using System.Xml;
using AutoMapper;
using DocumentFormat.OpenXml.Packaging;
using MediatR;
using Realchat.Application.Common.Handlers;
using Realchat.Application.Dto;
using Realchat.Application.Repositories;
using Realchat.Application.Storage;
using Realchat.Domain.Entities;

namespace Realchat.Application.Features.KnowledgeBaseFeatures.UploadFile;

public sealed class UploadFileHandler : BaseHandler, IRequestHandler<UploadFileRequest, Response>
{
    private readonly IMinioAdapter _minioAdapter;
    private readonly IKnowledgeBaseRepository _knowledgeBaseRepository;
    private readonly IChatbotRepository _chatbotRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IInformationChunkRepository _informationChunkRepository;

    public UploadFileHandler(IUnitOfWork unitOfWork, IMapper mapper, IMinioAdapter minioAdapter, IKnowledgeBaseRepository knowledgeBaseRepository, IChatbotRepository chatbotRepository, IOrganizationRepository organizationRepository, IInformationChunkRepository informationChunkRepository) : base(unitOfWork, mapper)
    {
        _minioAdapter = minioAdapter;
        _knowledgeBaseRepository = knowledgeBaseRepository;
        _chatbotRepository = chatbotRepository;
        _organizationRepository = organizationRepository;
        _informationChunkRepository = informationChunkRepository;
    }

    public async Task<Response> Handle(UploadFileRequest request, CancellationToken cancellationToken)
    {
        var knowledgeBase = await _knowledgeBaseRepository.GetById(request.KnowledgeBaseId, cancellationToken);
        if (knowledgeBase == null)
        {
            return new Response(400, "Failed to find a knowledge base with the given id.");
        }

        var chatbot = await _chatbotRepository.GetById(knowledgeBase.ChatbotId, cancellationToken);
        var organization = await _organizationRepository.GetById(chatbot.OrganizationId, cancellationToken);

        await _minioAdapter.UploadAndPreprocessFile($"knowledge_bases/{knowledgeBase.Id}/{Guid.NewGuid()}_{request.FileName}", request.FileStream, organization.Id, chatbot.Id, knowledgeBase.Id);

        List<string> informationChunkList = await PreprocessFile(request.FileStream);
        for (int i = 0; i < informationChunkList.Count; i++)
        {
            Console.WriteLine(informationChunkList[i]);
            var informationChunk = new InformationChunk
            {
                Id = Guid.NewGuid(),
                CreatedTime = DateTime.UtcNow,
                KnowledgeBaseId = knowledgeBase.Id,
                ChunkNumber = i + 1,
                Content = informationChunkList[i]
            };

            _informationChunkRepository.Create(informationChunk);
        }
        await UnitOfWork.Save(cancellationToken);
        return new Response(200, "Upload file successfully.");
    }

    private async Task<List<string>> PreprocessFile(Stream fileStream)
    {
        List<string> informationChunks = new();
        using HttpClient httpClient = new();

        Console.WriteLine("Preprocessing file.");
        WordprocessingDocument doc = WordprocessingDocument.Open(fileStream, false);

        XmlDocument xmlDocument = new();
        xmlDocument.Load(doc.MainDocumentPart.GetStream());

        var words = GetWordsFromXmlDocument(xmlDocument);

        int chunkSize = 100;
        int chunkCount = (int)Math.Ceiling((double)words.Count / chunkSize);

        for (int i = 0; i < chunkCount; i++)
        {
            int startIndex = i * chunkSize;
            int endIndex = Math.Min(startIndex + chunkSize, words.Count);
            List<string> chunk = words.GetRange(startIndex, endIndex - startIndex);

            string chunkText = string.Join(" ", chunk); // Join words with spaces
            informationChunks.Add(chunkText);
        }
        return informationChunks;
    }

    private List<string> GetWordsFromXmlDocument(XmlDocument xmlDocument)
    {
        List<string> words = new();

        // Recursively traverse the XML tree and extract text from XmlText nodes
        void ExtractWords(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Text)
            {
                string[] rawWords = node.Value.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                words.AddRange(rawWords);
            }
            else
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    ExtractWords(childNode);
                }
            }
        }

        ExtractWords(xmlDocument);

        return words;
    }
}