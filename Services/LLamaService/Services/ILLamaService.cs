using System.Collections.Generic;

namespace Realchat.Services.LLamaService.Services;

public interface ILLamaService
{
    public void Startup();
    public IAsyncEnumerable<string> GenerateResponseAsync(string context, string inquiry);
    public string GenerateResponse(string context, string inquiry);

}