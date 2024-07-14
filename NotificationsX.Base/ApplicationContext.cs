namespace NotificationsX;

public record ApplicationContext {
    public ApplicationContext(string name) {
        Name = name;
    }

    public string Name { get; }
}