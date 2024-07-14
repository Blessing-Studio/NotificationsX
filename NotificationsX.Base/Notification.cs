namespace NotificationsX;

public class Notification {
    public Notification() {
        BodyImageAltText = "Image";
        Buttons = [];
    }

    public string Body { get; set; }
    public string Title { get; set; }
    public string BodyImagePath { get; set; }
    public string BodyImageAltText { get; set; }
    public List<(string Title, string ActionId)> Buttons { get; }
}