using LLama;
using LLama.Common;

namespace Realchat.Services.LLamaService.Services;

public sealed class LLamaService : ILLamaService
{
    //private readonly string modelPath = "D:\\Data\\Models\\LLaMa\\7B\\llama-2-7b-guanaco-qlora.ggmlv3.q2_K.bin";
    private readonly string modelPath = ".\\Files\\llama-2-7b-guanaco-qlora.Q2_K.gguf";
    private ChatSession _chatSession;
    //private LLamaModel _llamaModel;
    private LLamaWeights _llamaModel;

    private InferenceParams _inferenceParams;
    private const string dialogPrompt = "Transcript of a dialog, where the User interacts with DMSBot. DMSBot is honest, helpful but not chatty, and always answer the User's questions truthfully based on Context and not prior knowledge.\r\n\r\n";
    private const string systemPrompt1 = "Context:INFORMATION STORAGE TIME The Member's personal data will be stored until there is a request to cancel or the Member can log in and perform the cancellation himself. Remaining in all cases, Members' personal information will be kept confidential on solutions.viettel.vn's server.\r\nUser:How long does Viettel store user's data?\r\nDMSBot:We will store this information for as long as you use the services provided by Viettel and until you request deletion of your account. We will keep your data until you ask us to delete it from our servers or you cancel your membership with us.\r\nContext:";
    private const string systemPrompt2 = "information related to the customer's access password and should not be shared with anyone else. - If using a computer with many people, the customer should log out, or exit all windows. Website window is open. - Customers can also access and edit their personal information according to the appropriate links (website's links) provided by the Management Board. - If customers have questions about the information Individuals can directly contact the customer care phone number provided by the website or send an email to the website's mailbox. This privacy policy takes effect from June 1, 2018.\r\nUser: What is the effective date of the privacy policy?\r\nDMSBot: The effective date of the privacy policy is June 1, 2018.\r\nContext:";
    private const string systemPrompt3 = "Web and associated data sources mentioned above. Disclaimer To the extent permitted by law, the information displayed on this website does not come with any warranties or undertakings of any kind, formal or implied, on the part of Viettel regarding the suitability of the product or service for the specific purpose chosen by the buyer. To the extent permitted by law, Viettel also disclaims any responsibility or warrants that the website will be free of operational errors, safe, uninterrupted or of any kind as to the accuracy, completeness and Timeliness of displayed information. To the extent permitted by law, when accessing this website, you implicitly agree that Viettel and its branches, officers, managers, employees and agents are not responsible for any any injury, loss, claim or damage, indirect, unforeseeable or consequential damage of any kind arising out of or in connection with: (1) use the information on this website; (2) connections from this website; (3) register as a member, sign up to receive emails or participate in Viettel's customer care program; (4) Whether Viettel will provide the service or not even in cases where Viettel has been warned about the possibility of damage.\r\nUser: What is the scope of Viettel's use of members' personal data?\r\nDMSBot: I am unable to answer the question due to lack of context.\r\nContext:";
    private const string systemPrompt4 = "WEBSITE TERMS OF USE Support Website Terms of Use This website is owned and managed by Viettel Business Solutions Corporation. The information displayed on this Website is for the purpose of providing information about Viettel, its products, services, promotional programs as well as customer care programs.\r\nUser: Who owns and manages this website?\r\nDMSBot: This website is owned and managed by Viettel Business Solutions.\r\nContext:";
    private const string systemPrompt5 = "The website management board uses various information security technologies such as international standards PCI, SSL, etc. to protect this information from unintended retrieval, use or disclosure. However, no data can be 100% secure. Therefore, the Management Board cannot guarantee absolute security and the Management Board cannot be held responsible in the event of unauthorized access to the customer's personal information such as the case where the customer voluntarily intention to share information with others.. - Management recommends that customers keep confidential information related to customer access passwords and should not share it with anyone else. - If using a shared computer with many people, customers should log out, or exit all open Website windows. - Customers can also access and edit their personal information according to the appropriate links (website's links) provided by the Management.\r\nUser: What information security technologies does the website management board use?\r\nDMSBot: The website management board uses various international standards such as PCI, SSL, etc., to protect this information from unintended retrieval, use or disclosure. However, no data can be 100% secure. Therefore, the Management Board cannot guarantee absolute security and the Management Board cannot be held responsible in the event of unauthorized access to the customer's personal information such as the case where the customer voluntarily intention to share information with others.\r\nContext:";

    private const string systemPrompt6 = "OVERVIEW OF DMS PRODUCTS by Military Industry and Telecommunications Group (Viettel Group) provides online sales distribution system management solutions for businesses. Helps businesses manage goods distribution activities to the market, to distribution channels and points of sale. Through the DMS system, distributors provide customer information, products, promotions, and carry out warehouse import and export for each sales staff providing goods to retailers such as supermarkets and convenience stores. , agents, grocery stores... to reach consumers under the management of sales supervisors. All developments on the distribution channel (orders, images and customer issues) are always updated in real time. This distribution model will help sales activities be faster, more accurate, more effective and easier to manage. Support point, instructions for use During the implementation process, for operational issues related to the product/application, users should contact the following contact point: Southern Solution Center - Solution Corporation Viettel France Phone: 19008098-branch 2 Email: cskh_gpdn@viettel.com.vn Other contents Fields * are required Date type: dd/mm/yyyy INTRODUCTION TO FUNCTIONS Functions in the Module CMS Viettel STT Function Description Target users 1 Company list Allow Search/ Add new/ Edit/ Activate/ Cancel/ Lock and Unlock company � System administration 2 Application version Allow Add new/ Edit/ Copy/ Delete/ Lock and Unlock application version � System administration 3 Account management Search account list � System administration 4 Declare visit steps Search/ Add new/ Edit edit � System Administration 5 Configuration declaration Declare system configuration group/unit configuration; Allows Search / Add new / Edit and View configuration value list � System Administration 6 Role declaration � Role declaration; Allows adding/editing/locking/unlocking and deleting roles. System administration 7 Function declaration. Subsystem declaration: Web/Mobile/... used for the system; Allows searching/ Adding new/ Editing/ Locking/ Unlocking various types of functions: menu / submenu / form / control � System administration Functions in Common Module STT Function Description Target users 1 Post enter Allows entering identification code / login / Select position / Forgot password Authorized users 2 Employee information View employee information / Change password / View and download documents User manual Authorized users decentralization Functions in the System Module No. Function Description User object 1 Parameter declaration 1. Product declaration 2. Order declaration 3. Sales order declaration 4. Article declaration Adjust inventory 5. Declare Inventory transfer Authorized users Functions in Module Category No. Function Description User 1 Product Product category management Product attribute management Product management Product Tax rate management Price management Delegated users 2 Units and employees Public management\r\nUser: DMS product overview?\r\nDMSBot: Viettel provides online goods distribution management system (DMS) version 4.0 for businesses to manage goods distribution activities to the market, distribution channels and points of sale. Through the DMS system, distributors provide customer information, products, promotions, and carry out warehouse import and export for each sales staff providing goods to retailers such as supermarkets and convenience stores. , agents, grocery stores... to reach consumers under the management of sales supervisors.\r\nContext:";

    public LLamaService()
    {
        //var modelParams = new ModelParams(modelPath, contextSize: 3999);
        //_llamaModel = new LLamaModel(modelParams);
        //_chatSession = new ChatSession(new InteractiveExecutor(_llamaModel));
        //_chatSession = new StatelessExecutor(_llamaModel);
        //_inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "Context:" } };

        //Console.ForegroundColor = ConsoleColor.Blue;
        //Console.WriteLine("Preprompting using system prompt #1.");
        //foreach (var text in _chatSession.Chat(systemPrompt1, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        //Console.WriteLine();

        //Console.WriteLine("Preprompting using system prompt #2.");
        //foreach (var text in _chatSession.Chat(" " + systemPrompt2, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        //Console.WriteLine();

        //Console.WriteLine("Preprompting using system prompt #3.");
        //foreach (var text in _chatSession.Chat(" " + systemPrompt3, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        //Console.WriteLine();

        //Console.WriteLine("Preprompting using system prompt #4.");
        //foreach (var text in _chatSession.Chat(" " + systemPrompt4, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        //Console.WriteLine();

        //Console.WriteLine("Preprompting using system prompt #5.");
        //foreach (var text in _chatSession.Chat(" " + systemPrompt5, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        //Console.WriteLine();
        //Console.ForegroundColor = ConsoleColor.White;
    }

    public void Startup()
    {
        //var modelParams = new ModelParams(modelPath, contextSize: 3999);
        //_llamaModel = new LLamaModel(modelParams);
        //_chatSession = new ChatSession(new InteractiveExecutor(_llamaModel));
        //_inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "Context:" } };

        var modelParams = new ModelParams(modelPath)
        {
            ContextSize = 3000
            // Seed = 1337
            // GpuLayerCount = 5
        };
        _llamaModel = LLamaWeights.LoadFromFile(modelParams);

        var context = _llamaModel.CreateContext(modelParams);
        var ex = new InteractiveExecutor(context);
        _chatSession = new ChatSession(ex);
        _inferenceParams = new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "Context:" } };

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Preprompting using system prompt #1 and #2.");
        foreach (var text in _chatSession.Chat(dialogPrompt + systemPrompt1 + systemPrompt2, _inferenceParams))
        {
            Console.Write(text);
        }
        //Console.WriteLine();

        //Console.WriteLine("Preprompting using system prompt #3 and #4.");
        //foreach (var text in _chatSession.Chat(systemPrompt3 + systemPrompt4, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        Console.WriteLine("Finished preprompting.");

        //Console.WriteLine("Preprompting using system prompt #3.");
        //foreach (var text in _chatSession.Chat(" " + systemPrompt3, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        //Console.WriteLine();

        //Console.WriteLine("Preprompting using system prompt #4.");
        //foreach (var text in _chatSession.Chat(" " + systemPrompt4, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        //Console.WriteLine();

        //Console.WriteLine("Preprompting using system prompt #5.");
        //foreach (var text in _chatSession.Chat(" " + systemPrompt5, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        //Console.WriteLine();
        //Console.ForegroundColor = ConsoleColor.White;

        //Console.WriteLine("Preprompting using system prompt #6.");
        //foreach (var text in _chatSession.Chat(" " + systemPrompt6, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        //Console.WriteLine();

        //Console.WriteLine("Preprompting using system prompt.");
        //foreach (var text in _chatSession.Chat(dialogPrompt + systemPrompt1, _inferenceParams))
        //{
        //    Console.Write(text);
        //}
        //Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
    }

    public async IAsyncEnumerable<string> GenerateResponseAsync(string context, string inquiry)
    {
        string prompt = context + "\r\n" + "User:" + inquiry;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Context:" + prompt);
        var outputs = _chatSession.ChatAsync(dialogPrompt + prompt, _inferenceParams);
        await foreach (var output in outputs)
        {
            Console.Write(output);
            yield return output;
        }
        Console.ForegroundColor = ConsoleColor.White;
    }

    public string GenerateResponse(string context, string inquiry)
    {
        string result = string.Empty;
        string prompt = context + "\r\n" + "User:" + inquiry;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Context:" + prompt);
        foreach (var text in _chatSession.Chat(prompt, _inferenceParams))
        {
            result += text;
            // Console.Write(text);
        }
        Console.ForegroundColor = ConsoleColor.White;
        return result;
    }
}