using Microsoft.AspNetCore.Mvc;
using Realchat.Services.LLamaService.Dto;
using Realchat.Services.LLamaService.Services;

namespace Realchat.Services.LLamaService.Controllers;

[ApiController]
[Route("api/llama")]
public class LLamaController : ControllerBase
{
    private readonly ILogger<LLamaController> _logger;
    private readonly ILLamaService _llamaService;

    public LLamaController(ILogger<LLamaController> logger, ILLamaService llamaService)
    {
        _logger = logger;
        _llamaService = llamaService;
    }

    [HttpGet("startup")]
    public ActionResult Startup()
    {
        _llamaService.Startup();
        return Ok();
    }

    [HttpPost("inference-async")]
    public async Task InferenceAsync([FromBody] InferenceRequest inferenceRequest, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        await foreach (var r in _llamaService.GenerateResponseAsync(inferenceRequest.Context, inferenceRequest.Inquiry))
        {
            await Response.WriteAsync(r, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }

        //string response = string.Empty;
        //await foreach (var r in _llamaService.GenerateResponse(inferenceRequest.Context, inferenceRequest.Inquiry))
        //{
        //    response += r.ToString();
        //}

        await Response.CompleteAsync();
        //var response = _llamaService.GenerateResponse(inferenceRequest.Context, inferenceRequest.Inquiry);
        //return Ok(new InferenceResponse(response));
    }


    [HttpPost("inference")]
    public ActionResult Inference([FromBody] InferenceRequest inferenceRequest, CancellationToken cancellationToken)
    {
        var response = _llamaService.GenerateResponse(inferenceRequest.Context, inferenceRequest.Inquiry);
        return Ok(new InferenceResponse(response));
    }
}
