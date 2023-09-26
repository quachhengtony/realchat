namespace Realchat.Services.LLamaService.Dto;

public sealed record InferenceRequest(string Context, string Inquiry);