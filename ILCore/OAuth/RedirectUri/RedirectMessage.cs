namespace ILCore.OAuth.RedirectUri;

public enum AuthStatus
{
    Success,
    Error,
    Interrupt
}

public class RedirectMessage(
    string titleSuccess = "Success",
    string titleError = "Error",
    string contentSuccess = "Success",
    string contentError = "Error",
    string titleInterrupt = "Interrupt",
    string contentInterrupt = "Interrupt")
{
    public string TitleSuccess { get; set; } = titleSuccess;
    public string TitleError { get; set; } = titleError;
    public string TitleInterrupt { get; set; } = titleInterrupt;
    public string ContentSuccess { get; set; } = contentSuccess;
    public string ContentError { get; set; } = contentError;

    public string ContentInterrupt { get; set; } = contentInterrupt;
}