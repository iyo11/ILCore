namespace ILCore.OAuth.RedirectUri;

public enum AuthStatus
{
    Success,
    Error,
    Interrupt
}

public class RedirectMessage
{
    public RedirectMessage(string titleSuccess = "Success", string titleError = "Error", string contentSuccess = "Success", string contentError = "Error", string titleInterrupt = "Interrupt", string contentInterrupt = "Interrupt")
    {
        TitleSuccess = titleSuccess;
        TitleError = titleError;
        ContentSuccess = contentSuccess;
        ContentError = contentError;
        TitleInterrupt = titleInterrupt;
        ContentInterrupt = contentInterrupt;
    }
    
    public string TitleSuccess { get; set; }
    public string TitleError { get; set; }
    public string TitleInterrupt { get; set; }
    public string ContentSuccess { get; set; }
    public string ContentError { get; set; }

    public string ContentInterrupt { get; set; }
}