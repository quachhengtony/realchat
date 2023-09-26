namespace Realchat.Application.LLM;

public interface ILLamaAdapter
{
    public void StartLLamaConnection();
    public Task<string> GenerateResponse(string context, string inquiry);
    public Task<string> GetResponse();
    public void PrintLLamaConnectionStatus();
}